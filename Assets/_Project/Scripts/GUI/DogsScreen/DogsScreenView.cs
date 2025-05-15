using UnityEngine;

namespace Assets._Project.Scripts.GUI.DogsScreen
{
    public class DogsScreenView : MonoBehaviour
    {
        [field: SerializeField] public RectTransform contentParent { get; private set; }
        [field: SerializeField] public DogBreedItemView breedItemViewPrefab { get; private set; }
    }
}