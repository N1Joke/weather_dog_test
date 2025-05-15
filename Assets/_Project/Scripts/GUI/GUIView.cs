using Assets._Project.Scripts.GUI.BotTabs;
using Assets._Project.Scripts.GUI.DogsScreen;
using Assets._Project.Scripts.GUI.WeatherScreen;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    public class GUIView : MonoBehaviour
    {
        [field: SerializeField] public BotTabsView botTabsView { get; private set; }
        [field: SerializeField] public WeatherScreenView weatherScreenView { get; private set; }
        [field: SerializeField] public DogsScreenView dogsScreenView { get; private set; }
    }
}