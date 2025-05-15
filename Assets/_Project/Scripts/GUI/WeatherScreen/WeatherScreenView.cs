using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.GUI.WeatherScreen
{
    public class WeatherScreenView : MonoBehaviour
    {
        [field: SerializeField] public Image icon { get; private set; }
        [field: SerializeField] public TextMeshProUGUI lable { get; private set; }
        [field: SerializeField] public RectTransform loadingImage { get; private set; }
    }
}