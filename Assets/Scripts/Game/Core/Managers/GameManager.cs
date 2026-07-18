using UnityEngine;
using Zenject;

namespace Game.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        private GameEntryPointManager _gameEntryPoint;

        [Inject]
        public void Construct(GameEntryPointManager gameEntryPoint)
            => _gameEntryPoint = gameEntryPoint;

        private void Start()
            => _gameEntryPoint.Initialize();
    }
}