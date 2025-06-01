using System;
using System.Collections.Generic;
using UnityEngine;
using Wattle.Wild.Infrastructure.Conversation;

namespace Wattle.Wild.UI
{
    public class UIDialogueReplyPanel : MonoBehaviour
    {
        public event Action<DialogueReply, bool> OnReplySelected;

        [SerializeField] private UIDialogueReplyOption dialogueOptionPrefab;
        [SerializeField] private RectTransform optionContainer;
        [SerializeField] private DialogueReply leaveReply;

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
                option.Init(reply, false);
                dialogueOptions.Add(option);
            }

            UIDialogueReplyOption leaveOption = Instantiate(dialogueOptionPrefab, optionContainer);
            leaveOption.Init(leaveReply, true);
            dialogueOptions.Add(leaveOption);
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

        private void OnOptionSelected(DialogueReply reply, bool isLeave)
        {
            // player audio here?
            OnReplySelected?.Invoke(reply, isLeave);
        }
    }
}


