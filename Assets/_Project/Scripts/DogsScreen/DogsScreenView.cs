using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.GUI.DogsScreen
{
    public class DogsScreenView : BaseScreenView
    {
        [field: SerializeField] public RectTransform contentParent { get; private set; }       
        [field: SerializeField] public DogPopupDescriptionView popupView { get; private set; }       
    }
}