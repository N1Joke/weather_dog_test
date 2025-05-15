using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.GUI.BotTabs
{
    public class BotTabsView : MonoBehaviour
    {
        [field: SerializeField] public Button WeatherTabBtn { get; private set; }
        [field: SerializeField] public Button DogTabBtn { get; private set; }
    }
}