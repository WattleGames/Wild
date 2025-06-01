using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wattle.Wild.Infrastructure.Conversation;

namespace Wattle.Wild.UI
{
    public class UIDialogueReplyOption : MonoBehaviour
    {
        public static event Action<DialogueReply, bool> OnOptionSelected;

        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI optionText;

        private DialogueReply reply;
        private bool isLeave;

        public void Init(DialogueReply reply, bool isLeave)
        {
            this.reply = reply;
            this.isLeave = isLeave;

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
            OnOptionSelected?.Invoke(reply, isLeave);
        }
    }
}
