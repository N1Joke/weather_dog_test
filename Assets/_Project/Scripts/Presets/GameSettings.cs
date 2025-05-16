using UnityEngine;

namespace Presets
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
    public class GameSettings : ScriptableObject
    {
        [field: SerializeField] public float loadingImageRotationSpeed { get; private set; } = 10;
        [field: SerializeField] public float weatherRefreshTimer { get; private set; } = 5;
        [field: SerializeField] public float serverDelaySimulationBreedInfoDescription { get; private set; } = 0f;
    }
}