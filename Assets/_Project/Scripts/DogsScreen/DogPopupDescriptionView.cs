using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.GUI.DogsScreen
{
    public class DogPopupDescriptionView : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI header { get; private set; }
        [field: SerializeField] public TextMeshProUGUI description { get; private set; }
        [field: SerializeField] public Button closeBtn { get; private set; }
    }
}