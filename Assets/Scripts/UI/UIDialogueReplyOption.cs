using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wattle.Wild.Infrastructure.Conversation;

namespace Wattle.Wild.UI
{
    public class UIDialogueReplyOption : MonoBehaviour
    {
        public static event Action<DialogueReply> OnOptionSelected;

        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI optionText;

        private DialogueReply reply;

        public void Init(DialogueReply reply)
        {
            this.reply = reply;
            optionText.text = reply.replyText;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OptionSelect_OnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OptionSelect_OnClick);
        }

        private void OptionSelect_OnClick()
        {
            OnOptionSelected?.Invoke(reply);
        }
    }
}
