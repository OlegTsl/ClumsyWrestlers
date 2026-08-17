using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.Extension;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Systems
{
    public sealed class HitReactionSystem : IHitReactionSystem, ILateTickable
    {
        private const float PeakTimeNormalized = 0.25f;
        private const float MinimumDirectionSqrMagnitude = 0.0001f;

        private readonly IGameEventsBus _events;
        private readonly ICharacterContext _models;
        private readonly ICharacterViewContext _views;
        private readonly Dictionary<EntityId, HitReactionState> _states = new(16);

        public HitReactionSystem(
            IGameEventsBus events,
            ICharacterContext models,
            ICharacterViewContext views
        )
        {
            _events = events;
            _models = models;
            _views = views;
            _events.Subscribe<OnHitResolvedEvent>(OnHitResolved);
            _models.OnCharacterAdded += Register;
            _models.OnCharacterRemoved += Unregister;

            var characters = _models.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                Register(characters[i].CharacterID);
            }
        }

        private void OnHitResolved(OnHitResolvedEvent evt)
        {
            if (evt.Hit.TargetType != HitObjectType.Character)
            {
                return;
            }

            ICharacterModel target = _models.GetModel(evt.Hit.TargetID);
            if (target == null || !_states.TryGetValue(evt.Hit.TargetID, out HitReactionState state))
                return;

            ICharacterCombatRuntimeState combat = target.GetState<ICharacterCombatRuntimeState>();
            if (!combat.Enabled)
                return;

            HitReactionSettings settings = target.Data.Combat.HitReaction;
            if (settings.LeanAngle <= 0f)
            {
                Reset(target.CharacterID, state);
                return;
            }

            Vector3 direction = evt.Hit.Force;
            direction.y = 0f;
            
            if (direction.sqrMagnitude <= MinimumDirectionSqrMagnitude)
                return;

            direction.Normalize();
            
            Vector3 worldAxis = Vector3.Cross(Vector3.up, direction);
            Vector3 localAxis = combat.InverseTransformDirection(worldAxis);
            
            state.Active         = true;
            state.Elapsed        = 0f;
            state.Duration       = settings.LeanDuration;
            state.StartRotation  = state.CurrentRotation;
            state.TargetRotation = Quaternion.AngleAxis(settings.LeanAngle, localAxis);
        }

        public void LateTick()
        {
            foreach (KeyValuePair<EntityId, HitReactionState> pair in _states)
            {
                HitReactionState state = pair.Value;
                if (!state.Active)
                    continue;

                ICharacterModel target = _models.GetModel(pair.Key);
                if (target == null || !target.GetState<ICharacterActivityState>().Enabled)
                {
                    Reset(pair.Key, state);
                    continue;
                }

                state.Elapsed += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(state.Elapsed / state.Duration);
                if (normalizedTime < PeakTimeNormalized)
                {
                    float progress = Mathf.SmoothStep(
                        0f, 1f, normalizedTime / PeakTimeNormalized);
                    state.CurrentRotation = Quaternion.SlerpUnclamped(
                        state.StartRotation, state.TargetRotation, progress);
                }
                else
                {
                    float progress = Mathf.SmoothStep(
                        0f, 1f, (normalizedTime - PeakTimeNormalized) / (1f - PeakTimeNormalized));
                    state.CurrentRotation = Quaternion.SlerpUnclamped(
                        state.TargetRotation, Quaternion.identity, progress);
                }

                _views.GetView(pair.Key)?.SetVisualLean(state.CurrentRotation);
                
                if (normalizedTime >= 1f)
                    Reset(pair.Key, state);
            }
        }

        private void Register(EntityId characterId)
        {
            if (!_states.ContainsKey(characterId))
            {
                _states.Add(characterId, new HitReactionState());
            }
        }

        private void Unregister(EntityId characterId)
        {
            if (_states.TryGetValue(characterId, out HitReactionState state))
            {
                Reset(characterId, state);
            }

            _states.Remove(characterId);
        }

        private void Reset(EntityId characterId, HitReactionState state)
        {
            state.Active = false;
            state.Elapsed = 0f;
            state.CurrentRotation = Quaternion.identity;
            state.StartRotation = Quaternion.identity;
            state.TargetRotation = Quaternion.identity;
            _views.GetView(characterId)?.SetVisualLean(Quaternion.identity);
        }

        public void Dispose()
        {
            _events.Unsubscribe<OnHitResolvedEvent>(OnHitResolved);
            _models.OnCharacterAdded -= Register;
            _models.OnCharacterRemoved -= Unregister;
        }
    }
}
