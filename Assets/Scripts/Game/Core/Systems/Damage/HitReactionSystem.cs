using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class HitReactionSystem : IHitReactionSystem, ILateTickable
    {
        private const float PeakTimeNormalized = 0.25f;
        private const float MinDirectionSqrMagnitude = 0.0001f;

        private readonly GameEventsBus     _events;
        private readonly ICharacterContext _context;
        private readonly Dictionary<Guid, HitReactionState> _states = new();

        public HitReactionSystem(
            GameEventsBus     events,
            ICharacterContext context
        )
        {
            _events  = events;
            _context = context;

            _events.Subscribe<OnHitResolvedEvent>(OnHitResolved);

            _context.OnCharacterAdded   += Register;
            _context.OnCharacterRemoved += Unregister;

            RegisterExistingCharacters();
        }

        private void RegisterExistingCharacters()
        {
            var characters = _context.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                Register(characters[i].CharacterID);
            }
        }

        private void Register(Guid characterId)
            => _states[characterId] = new HitReactionState();

        private void Unregister(Guid characterId)
        {
            if (_states.TryGetValue(characterId, out HitReactionState state))
            {
                ICharacterModel target = _context.GetModel(characterId);
                if (target != null)
                    Reset(target, state);
            }

            _states.Remove(characterId);
        }

        private void OnHitResolved(OnHitResolvedEvent evt)
        {
            ICharacterModel target = _context.GetModel(evt.Hit.TargetID);
            if (target == null || !target.Enabled ||
                !_states.TryGetValue(evt.Hit.TargetID, out HitReactionState state))
            {
                return;
            }

            HitReactionSettings settings = target.Data.Combat.HitReaction;
            if (settings == null || settings.LeanAngle <= 0f)
            {
                Reset(target, state);
                return;
            }

            Vector3 direction = evt.Hit.Direction;
            direction.y = 0f;
            if (direction.sqrMagnitude <= MinDirectionSqrMagnitude)
            {
                return;
            }

            direction.Normalize();
            Vector3 worldAxis = Vector3.Cross(Vector3.up, direction);
            Vector3 localAxis = target.InverseTransformDirection(worldAxis);

            state.Active = true;
            state.Elapsed = 0f;
            state.Duration = settings.LeanDuration;
            state.StartRotation = state.CurrentRotation;
            state.TargetRotation = Quaternion.AngleAxis(
                settings.LeanAngle,
                localAxis);
        }

        public void LateTick()
        {
            foreach (var pair in _states)
            {
                HitReactionState state = pair.Value;
                if (!state.Active)
                {
                    continue;
                }

                ICharacterModel target = _context.GetModel(pair.Key);
                if (target == null)
                {
                    continue;
                }

                if (!target.Enabled)
                {
                    Reset(target, state);
                    continue;
                }

                state.Elapsed += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(
                    state.Elapsed / state.Duration);

                if (normalizedTime < PeakTimeNormalized)
                {
                    float leanProgress = Mathf.SmoothStep(
                        0f,
                        1f,
                        normalizedTime / PeakTimeNormalized);
                    state.CurrentRotation = Quaternion.SlerpUnclamped(
                        state.StartRotation,
                        state.TargetRotation,
                        leanProgress);
                }
                else
                {
                    float recoverProgress = Mathf.SmoothStep(
                        0f,
                        1f,
                        (normalizedTime - PeakTimeNormalized) /
                        (1f - PeakTimeNormalized));
                    state.CurrentRotation = Quaternion.SlerpUnclamped(
                        state.TargetRotation,
                        Quaternion.identity,
                        recoverProgress);
                }

                target.SetVisualLean(state.CurrentRotation);

                if (normalizedTime >= 1f)
                {
                    Reset(target, state);
                }
            }
        }

        private static void Reset(
            IVisualLean target,
            HitReactionState state
        )
        {
            state.Active = false;
            state.Elapsed = 0f;
            state.CurrentRotation = Quaternion.identity;
            state.StartRotation = Quaternion.identity;
            state.TargetRotation = Quaternion.identity;
            target.SetVisualLean(Quaternion.identity);
        }

        public void Dispose()
        {
            _events.Unsubscribe<OnHitResolvedEvent>(OnHitResolved);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}
