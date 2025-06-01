using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
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

            Canvas.ForceUpdateCanvases();

            StartCoroutine(AnimateReplies());

            IEnumerator AnimateReplies()
            {
                foreach (UIDialogueReplyOption option in dialogueOptions)
                {
                    option.PlayEnterAnimation();
                    option.StartFloat();

                    yield return new WaitForSeconds(0.2f);
                }
                yield return null;
            }
        }

        public void CloseReplyPanel(Action onComplete = null) // TODO: handle exit animations
        {
            StartCoroutine(RemoveReplies(() =>
            {
                foreach (UIDialogueReplyOption option in dialogueOptions)
                {
                    Destroy(option.gameObject);
                }

                dialogueOptions.Clear();
                onComplete?.Invoke();
            }));

            IEnumerator RemoveReplies(Action onComplete)
            {
                for (int i = 0; i < dialogueOptions.Count; i++)
                {
                    UIDialogueReplyOption option = dialogueOptions[i];
                    Tweener tween = option.PlayExitAnimation();

                    yield return new WaitForSeconds(0.1f);

                    if (i == dialogueOptions.Count - 1)
                    {
                        tween.OnComplete(() =>
                        {
                            onComplete.Invoke();
                        });
                    }
                }
            }
        }

        private void OnOptionSelected(DialogueReply reply, bool isLeave)
        {
            // player audio here?
            OnReplySelected?.Invoke(reply, isLeave);
        }
    }
}


