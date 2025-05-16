using Assets._Project.Scripts.GUI.DogsScreen;
using UnityEngine;
using Zenject;

namespace Assets._Project.Scripts.DogsScreen.Factory
{
    public class DogBreedItemPool : MonoMemoryPool<Transform, DogBreedItemView>
    {
        protected override void Reinitialize(Transform parent, DogBreedItemView item)
        {
            item.transform.SetParent(parent, false);
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(DogBreedItemView item)
        {
            item.gameObject.SetActive(false);
            item.button.onClick.RemoveAllListeners();
        }
    }
}
