using System;
using System.Collections.Generic;
using UnityEngine;
using Wattle.Wild.Infrastructure.Conversation;

namespace Wattle.Wild.UI
{
    public class UIDialogueReplyPanel : MonoBehaviour
    {
        public event Action<DialogueReply> OnReplySelected;

        [SerializeField] private UIDialogueReplyOption dialogueOptionPrefab;
        [SerializeField] private RectTransform optionContainer;

        private List<UIDialogueReplyOption> dialogueOptions;

        private void OnEnable()
        {
            UIDialogueReplyOption.OnOptionSelected += OnOptionSelected;
        }

        private void OnDisable()
        {
            UIDialogueReplyOption.OnOptionSelected -= OnOptionSelected;
        }

        public void OpenReplyWindow(DialogueReply[] replies) // TODO: will need to handle enter animations
        {
            dialogueOptions = new List<UIDialogueReplyOption>();

            foreach (DialogueReply reply in replies)
            {
                UIDialogueReplyOption option = Instantiate(dialogueOptionPrefab, optionContainer);
                option.Init(reply);
                dialogueOptions.Add(option);
            }
        }

        public void CloseReplyPanel(Action onComplete = null) // TODO: handle exit animations
        {
            foreach (UIDialogueReplyOption reply in dialogueOptions)
            {
                Destroy(reply.gameObject);
            }

            dialogueOptions.Clear();
            onComplete?.Invoke();
        }

        private void OnOptionSelected(DialogueReply reply)
        {
            // player audio here?
            OnReplySelected?.Invoke(reply);
        }
    }
}


