using UnityEngine;

namespace Game.Common.AssetsManager
{
    [CreateAssetMenu(fileName = "ViewPrefab", menuName = "Game/Assets/View Prefab")]
    public sealed class ViewPrefabAsset : ScriptableObject
    {
        [SerializeField] private MonoBehaviour _viewPrefab;

        public MonoBehaviour ViewPrefab => _viewPrefab;
    }
}
