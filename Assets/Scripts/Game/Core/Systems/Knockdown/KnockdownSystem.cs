using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.Data;
using Game.Core.Extension;
using Game.Core.GameEvents;
using Zenject;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Systems
{
    public sealed class KnockdownSystem : IKnockdownSystem, ILateTickable
    {
        private readonly IGameEventsBus        _events;
        private readonly ICharacterContext     _characters;
        private readonly ICharacterViewContext _views;
        private readonly Dictionary<EntityId, KnockdownState> _states = new(16);

        public KnockdownSystem(
            IGameEventsBus        events,
            ICharacterContext     characters,
            ICharacterViewContext views
        )
        {
            _events     = events;
            _characters = characters;
            _views      = views;

            _events.Subscribe<OnHitResolvedEvent>(OnHitResolved);
            _characters.OnCharacterAdded   += Register;
            _characters.OnCharacterRemoved += Unregister;

            var existingCharacters = _characters.AllCharacters;
            for (int i = 0; i < existingCharacters.Count; i++)
            {
                Register(existingCharacters[i].CharacterID);
            }
        }

        private void OnHitResolved(OnHitResolvedEvent evt)
        {
            if (evt.Hit.TargetType != HitObjectType.Character ||
                evt.Hit.AttackType != AttackType.Power ||
                !_states.TryGetValue(evt.Hit.TargetID, out KnockdownState state) ||
                state.IsActive)
            {
                return;
            }

            ICharacterModel model = _characters.GetModel(evt.Hit.TargetID);
            if (model == null || !model.GetState<ICharacterActivityState>().Enabled)
                return;

            state.Phase = KnockdownPhase.AwaitingFallBack;
            model.GetState<ICharacterMovementState>().SetControlLock(
                CharacterControlLock.Knockdown, true);

            _events.Publish(new OnKnockdownEvent(evt.Hit.TargetID));
        }

        public void LateTick()
        {
            foreach (KeyValuePair<EntityId, KnockdownState> pair in _states)
            {
                KnockdownState state = pair.Value;
                if (!state.IsActive)
                    continue;

                ICharacterModel model = _characters.GetModel(pair.Key);
                if (model == null || !model.GetState<ICharacterActivityState>().Enabled)
                {
                    ReleaseControl(model, state);
                    continue;
                }

                ICharacterView view = _views.GetView(pair.Key);
                if (view == null)
                    continue;

                UpdatePhase(model, view.KnockdownAnimatorStateHash, state);
            }
        }

        private static void UpdatePhase(ICharacterModel model, int animatorStateHash, KnockdownState state)
        {
            switch (state.Phase)
            {
                case KnockdownPhase.AwaitingFallBack:
                    if (animatorStateHash == AnimationData.FallBackState)
                    {
                        state.Phase = KnockdownPhase.Falling;
                    }
                    break;

                case KnockdownPhase.Falling:
                    if (animatorStateHash == AnimationData.GetUpState)
                    {
                        state.Phase = KnockdownPhase.GettingUp;
                    }
                    break;

                case KnockdownPhase.GettingUp:
                    if (animatorStateHash == AnimationData.FallBackState)
                    {
                        state.Phase = KnockdownPhase.Falling;
                    }
                    else if (animatorStateHash != AnimationData.GetUpState)
                    {
                        ReleaseControl(model, state);
                    }
                    break;
            }
        }

        private static void ReleaseControl(ICharacterModel model, KnockdownState state)
        {
            state.Phase = KnockdownPhase.None;
            model?.GetState<ICharacterMovementState>().SetControlLock(
                CharacterControlLock.Knockdown, false);
        }

        private void Register(EntityId characterId)
        {
            if (!_states.ContainsKey(characterId))
                _states.Add(characterId, new KnockdownState());
        }

        private void Unregister(EntityId characterId)
        {
            if (_states.TryGetValue(characterId, out KnockdownState state))
            {
                ReleaseControl(_characters.GetModel(characterId), state);
                _states.Remove(characterId);
            }
        }

        public void Dispose()
        {
            _events.Unsubscribe<OnHitResolvedEvent>(OnHitResolved);
            _characters.OnCharacterAdded   -= Register;
            _characters.OnCharacterRemoved -= Unregister;

            foreach (KeyValuePair<EntityId, KnockdownState> pair in _states)
            {
                ReleaseControl(_characters.GetModel(pair.Key), pair.Value);
            }

            _states.Clear();
        }
    }
}
