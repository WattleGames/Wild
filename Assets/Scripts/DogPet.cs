using UnityEngine;
using UnityEngine.UI;
using Wattle.Wild.Logging;

public class DogPet : MonoBehaviour
{
    [SerializeField] private Button dougButton;

    private void OnEnable()
    {
        dougButton.onClick.AddListener(DougButton_OnClick);
    }

    private void OnDisable()
    {
        dougButton.onClick.RemoveListener(DougButton_OnClick);
    }

    private void DougButton_OnClick()
    {
        LOG.Log("Pet");
    }
}
