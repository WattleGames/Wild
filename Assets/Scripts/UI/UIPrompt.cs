using TMPro;
using UnityEngine;

namespace Neighbors.UI
{
    public class UIPrompt : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI promptText;

        public void Toggle(in bool enabled)
        {
            this.gameObject.SetActive(enabled);
        }

        public void SetText(string text)
        {
           promptText.text = text;
        }
    }
}

