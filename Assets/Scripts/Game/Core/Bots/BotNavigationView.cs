using Game.Core.Data;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Core.Bots
{
    public sealed class BotNavigationView : MonoBehaviour, IBotNavigationAgent
    {
        private const float MinimumDirectionSqrMagnitude = 0.0001f;
        private const float DestinationChangeSqrDistance = 0.0625f;
        private const float DropProbeOutwardDistance = 1f;
        private const float DropProbeHeight = 2f;
        private const float DropProbeDepth = 6f;

        [SerializeField] private NavMeshAgent _agent;

        private float _sampleDistance;
        private float _arrivalDistance;
        private Vector3 _linkEndPosition;
        private Vector3 _destination;
        private bool _hasDestination;
        private bool _isTraversingLink;

        private void Awake()
        {
            _agent.updatePosition = false;
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.autoTraverseOffMeshLink = false;
            _agent.enabled = false;
        }

        private void OnDisable()
        {
            _isTraversingLink = false;
            _hasDestination = false;
            if (_agent.enabled && _agent.isOnNavMesh)
            {
                _agent.ResetPath();
            }

            _agent.enabled = false;
        }

        public void Activate(
            Vector3 position,
            IBotBehaviorSettings settings)
        {
            _sampleDistance = settings.NavigationSampleDistance;
            _arrivalDistance = settings.ArrivalDistance;
            _isTraversingLink = false;
            _hasDestination = false;
            _agent.enabled = true;

            if (NavMesh.SamplePosition(
                    position,
                    out NavMeshHit hit,
                    _sampleDistance,
                    _agent.areaMask))
            {
                _agent.Warp(hit.position);
            }
        }

        public bool TryGetMovement(
            Vector3 position,
            Vector3 destination,
            bool isGrounded,
            out Vector3 direction,
            out bool shouldJump)
        {
            direction = Vector3.zero;
            shouldJump = false;

            if (!Synchronize(position) ||
                !NavMesh.SamplePosition(
                    destination,
                    out NavMeshHit destinationHit,
                    _sampleDistance,
                    _agent.areaMask))
            {
                return false;
            }

            if (_isTraversingLink)
            {
                Vector3 linkDirection = _linkEndPosition - position;
                linkDirection.y = 0f;
                if (isGrounded &&
                    linkDirection.sqrMagnitude <=
                    _arrivalDistance * _arrivalDistance)
                {
                    if (_agent.isOnOffMeshLink)
                    {
                        _agent.CompleteOffMeshLink();
                    }

                    _isTraversingLink = false;
                }
                else
                {
                    direction = Normalize(linkDirection);
                    shouldJump = isGrounded;
                    return direction != Vector3.zero;
                }
            }

            _agent.isStopped = false;
            if (!_hasDestination ||
                (_destination - destinationHit.position).sqrMagnitude >
                DestinationChangeSqrDistance)
            {
                _destination = destinationHit.position;
                _hasDestination = _agent.SetDestination(_destination);
                if (!_hasDestination)
                {
                    return false;
                }
            }

            if (_agent.pathPending)
            {
                return false;
            }

            if (_agent.isOnOffMeshLink)
            {
                _linkEndPosition = _agent.currentOffMeshLinkData.endPos;
                _isTraversingLink = true;
                direction = Normalize(_linkEndPosition - position);
                shouldJump = isGrounded;
                return direction != Vector3.zero;
            }

            if (_agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                _hasDestination = false;
                return false;
            }

            direction = _agent.steeringTarget - position;
            direction.y = 0f;
            direction = Normalize(direction);
            return direction != Vector3.zero;
        }

        public bool TryGetDistanceToDropEdge(
            Vector3 position,
            out float distance)
        {
            distance = 0f;
            if (!_agent.enabled ||
                !NavMesh.FindClosestEdge(
                    position,
                    out NavMeshHit hit,
                    _agent.areaMask))
            {
                return false;
            }

            Vector3 toEdge = hit.position - position;
            toEdge.y = 0f;
            Vector3 probeOrigin = hit.position -
                                  hit.normal * DropProbeOutwardDistance;
            probeOrigin.y += DropProbeHeight;
            int groundLayerMask = 1 << LayerData.Ground;
            if (Physics.Raycast(
                    probeOrigin,
                    Vector3.down,
                    DropProbeHeight + DropProbeDepth,
                    groundLayerMask,
                    QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            distance = toEdge.magnitude;
            return true;
        }

        public bool CanStandAt(Vector3 position, float edgeClearance)
        {
            if (!_agent.enabled ||
                !NavMesh.SamplePosition(
                    position,
                    out NavMeshHit hit,
                    _arrivalDistance,
                    _agent.areaMask))
            {
                return false;
            }

            Vector3 sampleOffset = hit.position - position;
            sampleOffset.y = 0f;
            if (sampleOffset.sqrMagnitude > _arrivalDistance * _arrivalDistance)
            {
                return false;
            }

            return !TryGetDistanceToDropEdge(hit.position, out float distance) ||
                   distance >= edgeClearance;
        }

        public void Stop()
        {
            if (!_agent.enabled || !_agent.isOnNavMesh)
            {
                return;
            }

            _agent.isStopped = true;
            _agent.ResetPath();
            _hasDestination = false;
        }

        public void Deactivate()
        {
            _isTraversingLink = false;
            _hasDestination = false;
            if (_agent == null)
            {
                return;
            }

            if (_agent.enabled && _agent.isOnNavMesh)
            {
                _agent.ResetPath();
            }

            _agent.enabled = false;
        }

        private bool Synchronize(Vector3 position)
        {
            if (!_agent.enabled)
            {
                return false;
            }

            if (!_agent.isOnNavMesh)
            {
                if (!NavMesh.SamplePosition(
                        position,
                        out NavMeshHit hit,
                        _sampleDistance,
                        _agent.areaMask))
                {
                    return false;
                }

                return _agent.Warp(hit.position);
            }

            _agent.nextPosition = position;
            return true;
        }

        private static Vector3 Normalize(Vector3 direction)
        {
            direction.y = 0f;
            return direction.sqrMagnitude < MinimumDirectionSqrMagnitude
                ? Vector3.zero
                : direction.normalized;
        }
    }
}
