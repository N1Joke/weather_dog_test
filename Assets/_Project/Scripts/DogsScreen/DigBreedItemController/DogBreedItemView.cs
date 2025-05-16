using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.GUI.DogsScreen
{
    public class DogBreedItemView : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI lable { get; private set; }
        [field: SerializeField] public Button button { get; private set; }
        [field: SerializeField] public RectTransform loadingRect { get; private set; }
    }
}