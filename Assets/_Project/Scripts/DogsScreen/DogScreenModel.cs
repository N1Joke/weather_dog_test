using Assets._Project.Scripts.GUI.DogsScreen.DigBreedItemController;
using System.Collections.Generic;

namespace Assets._Project.Scripts.GUI.DogsScreen
{
    public class DogScreenModel
    {
        private List<DogBreedItemController> _items = new List<DogBreedItemController>(11);

        public void AddItem(DogBreedItemController item)
        {
            _items.Add(item);
        }

        public void DisposeItems()    
        {
            for (int i = 0; i < _items.Count; i++)            
                _items[i]?.Dispose();

            _items.Clear();
        }
    }
}
