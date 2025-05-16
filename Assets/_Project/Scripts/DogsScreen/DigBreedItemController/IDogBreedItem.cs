using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets._Project.Scripts.GUI.DogsScreen.DigBreedItemController
{
    public interface IDogBreedItem
    {
        public string Id { get; }
        void ToggleLoading(bool enable);
    }
}
