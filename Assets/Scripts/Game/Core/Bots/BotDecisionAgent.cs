using Game.Common.Input;
using Game.Core.Character;
using Game.Core.Commands;
using Game.Core.Entities;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Bots
{
    public sealed class BotDecisionAgent : IBotDecisionAgent
    {
        private const float MinimumDirectionSqrMagnitude = 0.0001f;

        private readonly ICharacterModel _character;
        private readonly IBotPerception _perception;
        private readonly IBotUtilityEvaluator _utility;
        private readonly IBotNavigationAgent _navigation;
        private readonly IBotBehaviorSettings _settings;
        private readonly uint _decisionIntervalTicks;
        private readonly uint _attackCooldownTicks;
        private readonly uint _powerHoldTicks;
        private readonly uint _jumpCooldownTicks;

        private uint _nextDecisionTick;
        private uint _nextAttackTick;
        private uint _nextJumpTick;
        private uint _powerReleaseTick;
        private bool _isSimpleAttackPressed;
        private bool _isPowerAttackPressed;
        private bool _isDisposed;

        public EntityId CharacterId => _character.CharacterID;

        public BotDecisionAgent(
            ICharacterModel character,
            IBotPerception perception,
            IBotUtilityEvaluator utility,
            IBotNavigationAgent navigation,
            IBotBehaviorSettings settings)
        {
            _character = character;
            _perception = perception;
            _utility = utility;
            _navigation = navigation;
            _settings = settings;
            _decisionIntervalTicks = SecondsToTicks(settings.DecisionInterval);
            _attackCooldownTicks = SecondsToTicks(settings.AttackCooldown);
            _powerHoldTicks = SecondsToTicks(settings.PowerAttackHoldDuration);
            _jumpCooldownTicks = SecondsToTicks(settings.JumpCooldown);

            Vector3 position =
                character.GetState<ICharacterTransformState>().Position;
            _navigation.Activate(position, settings);
        }

        public void CollectCommands(
            uint simulationTick,
            ICharacterCommandSink commandSink)
        {
            if (_isDisposed)
            {
                return;
            }

            ReleaseInputs(simulationTick, commandSink);
            ICharacterActivityState activity =
                _character.GetState<ICharacterActivityState>();
            if (!activity.Enabled || simulationTick < _nextDecisionTick)
            {
                return;
            }

            _nextDecisionTick = simulationTick + _decisionIntervalTicks;
            BotWorldState world = _perception.Sense();
            BotIntent intent = _utility.Evaluate(world, _character);
            ExecuteIntent(intent, world, simulationTick, commandSink);
        }

        private void ExecuteIntent(
            BotIntent intent,
            in BotWorldState world,
            uint simulationTick,
            ICharacterCommandSink commandSink)
        {
            switch (intent)
            {
                case BotIntent.Recover:
                    NavigateTo(
                        world.SafePosition,
                        simulationTick,
                        commandSink);
                    break;

                case BotIntent.ChaseEnemy:
                    NavigateTo(
                        world.EnemyPosition,
                        simulationTick,
                        commandSink);
                    break;

                case BotIntent.SimpleAttack:
                    AttackSimple(
                        world.EnemyPosition,
                        simulationTick,
                        commandSink);
                    break;

                case BotIntent.PowerAttack:
                    AttackPower(
                        world.EnemyPosition,
                        simulationTick,
                        commandSink);
                    break;

                case BotIntent.PushEnvironment:
                    PushEnvironment(world, simulationTick, commandSink);
                    break;

                default:
                    Stop(commandSink, simulationTick);
                    break;
            }
        }

        private void NavigateTo(
            Vector3 destination,
            uint simulationTick,
            ICharacterCommandSink commandSink)
            => NavigateTo(
                destination,
                _settings.ArrivalDistance,
                simulationTick,
                commandSink);

        private void NavigateTo(
            Vector3 destination,
            float arrivalDistance,
            uint simulationTick,
            ICharacterCommandSink commandSink)
        {
            ICharacterMovementRuntimeState movement =
                _character.GetState<ICharacterMovementRuntimeState>();
            Vector3 offset = destination - movement.Position;
            offset.y = 0f;
            if (offset.sqrMagnitude <=
                arrivalDistance * arrivalDistance)
            {
                Stop(commandSink, simulationTick);
                return;
            }

            if (!_navigation.TryGetMovement(
                    movement.Position,
                    destination,
                    movement.IsGrounded,
                    out Vector3 direction,
                    out bool shouldJump))
            {
                Hold(commandSink, simulationTick);
                return;
            }

            EnqueueMove(direction, simulationTick, commandSink);
            if (shouldJump && simulationTick >= _nextJumpTick)
            {
                EnqueueAction(
                    CharacterCommandType.Jump,
                    InputEventType.Pressed,
                    simulationTick,
                    commandSink);
                _nextJumpTick = simulationTick + _jumpCooldownTicks;
            }
        }

        private void AttackSimple(
            Vector3 targetPosition,
            uint simulationTick,
            ICharacterCommandSink commandSink)
        {
            if (!FaceTarget(targetPosition, simulationTick, commandSink) ||
                simulationTick < _nextAttackTick ||
                _isPowerAttackPressed)
            {
                return;
            }

            EnqueueAction(
                CharacterCommandType.SimpleAttack,
                InputEventType.Pressed,
                simulationTick,
                commandSink);
            _isSimpleAttackPressed = true;
            _nextAttackTick = simulationTick + _attackCooldownTicks;
        }

        private void AttackPower(
            Vector3 targetPosition,
            uint simulationTick,
            ICharacterCommandSink commandSink)
        {
            if (!FaceTarget(targetPosition, simulationTick, commandSink) ||
                simulationTick < _nextAttackTick ||
                _isSimpleAttackPressed ||
                _isPowerAttackPressed)
            {
                return;
            }

            EnqueueAction(
                CharacterCommandType.PowerAttack,
                InputEventType.Pressed,
                simulationTick,
                commandSink);
            _isPowerAttackPressed = true;
            _powerReleaseTick = simulationTick + _powerHoldTicks;
            _nextAttackTick = simulationTick + _attackCooldownTicks;
        }

        private void PushEnvironment(
            in BotWorldState world,
            uint simulationTick,
            ICharacterCommandSink commandSink)
        {
            ICharacterTransformState transform =
                _character.GetState<ICharacterTransformState>();
            Vector3 stagingOffset = world.PushStagingPosition - transform.Position;
            stagingOffset.y = 0f;
            float stagingArrivalDistance = Mathf.Min(
                0.2f,
                _settings.ArrivalDistance);
            if (stagingOffset.sqrMagnitude >
                stagingArrivalDistance * stagingArrivalDistance)
            {
                NavigateTo(
                    world.PushStagingPosition,
                    stagingArrivalDistance,
                    simulationTick,
                    commandSink);
                return;
            }

            AttackSimple(
                world.PushablePosition,
                simulationTick,
                commandSink);
        }

        private bool FaceTarget(
            Vector3 targetPosition,
            uint simulationTick,
            ICharacterCommandSink commandSink)
        {
            ICharacterTransformState transform =
                _character.GetState<ICharacterTransformState>();
            Vector3 direction = targetPosition - transform.Position;
            direction.y = 0f;
            if (direction.sqrMagnitude < MinimumDirectionSqrMagnitude)
            {
                Stop(commandSink, simulationTick);
                return true;
            }

            direction.Normalize();
            Vector3 forward = transform.Forward;
            forward.y = 0f;
            forward.Normalize();
            if (Vector3.Dot(forward, direction) >= _settings.FacingDotThreshold)
            {
                Stop(commandSink, simulationTick);
                return true;
            }

            EnqueueMove(
                direction * _settings.TurnInputMagnitude,
                simulationTick,
                commandSink);
            return false;
        }

        private void Stop(
            ICharacterCommandSink commandSink,
            uint simulationTick)
        {
            _navigation.Stop();
            Hold(commandSink, simulationTick);
        }

        private void Hold(
            ICharacterCommandSink commandSink,
            uint simulationTick)
            => EnqueueMove(Vector3.zero, simulationTick, commandSink);

        private void ReleaseInputs(
            uint simulationTick,
            ICharacterCommandSink commandSink)
        {
            if (_isSimpleAttackPressed)
            {
                EnqueueAction(
                    CharacterCommandType.SimpleAttack,
                    InputEventType.Released,
                    simulationTick,
                    commandSink);
                _isSimpleAttackPressed = false;
            }

            if (_isPowerAttackPressed && simulationTick >= _powerReleaseTick)
            {
                EnqueueAction(
                    CharacterCommandType.PowerAttack,
                    InputEventType.Released,
                    simulationTick,
                    commandSink);
                _isPowerAttackPressed = false;
            }
        }

        private void EnqueueMove(
            Vector3 direction,
            uint simulationTick,
            ICharacterCommandSink commandSink)
            => commandSink.TryEnqueue(new CharacterCommand(
                CharacterId,
                simulationTick,
                CharacterCommandType.Move,
                direction: direction));

        private void EnqueueAction(
            CharacterCommandType type,
            InputEventType inputEventType,
            uint simulationTick,
            ICharacterCommandSink commandSink)
            => commandSink.TryEnqueue(new CharacterCommand(
                CharacterId,
                simulationTick,
                type,
                inputEventType: inputEventType));

        private static uint SecondsToTicks(float seconds)
            => (uint)Mathf.Max(
                1,
                Mathf.CeilToInt(seconds / Time.fixedDeltaTime));

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _navigation.Deactivate();
        }
    }
}
