using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Wattle.Wild.Infrastructure;
using Wattle.Wild.Infrastructure.Conversation;
using Wattle.Wild.UI;

namespace Wattle.Wild.Gameplay.Conversation
{
    public class ConversationManager : MonoBehaviour
    {
        public event Action<string> onItemEarned;

        [Header("Panels")]
        [SerializeField] private UIDialoguePanel dialoguePanel;
        [SerializeField] private UIDialogueReplyPanel dialogueReplyPanel;

        [Header("Speaker")]
        [SerializeField] private RectTransform speakerContainer;

        [Header("Test")]
        [SerializeField] private Conversaion testConversation;

        private UIDialogueSpeaker dialogueSpeaker;

        private Tweener speakerMovementTween = null;
        private Tweener speakerBobTween = null;

        private bool dialoguePanelShowing = false;
        private bool dialogueReplyPanelShowing = false;

        private float speakerStartingXPosition;
        private float speakerEndingXPosition;

        public void StartConversation(Conversaion conversation)
        {
            dialoguePanel.onDialogueStarted += OnDialogueStarted;
            dialoguePanel.onDialogueFinished += OnDialogueFinished;

            dialogueReplyPanel.OnReplySelected += OnReplySelected;

            speakerStartingXPosition = speakerContainer.anchoredPosition.x + speakerContainer.rect.width;
            speakerEndingXPosition = speakerContainer.anchoredPosition.x;

            speakerBobTween?.Kill();

            if (conversation.startingStage is Dialogue dialogue)
            {
                ShowDialogue(dialogue);
            }
            else if (conversation.startingStage is DialogueReply reply)
            {
                ShowReplies(new DialogueReply[] { reply });

                if (dialogueSpeaker != null)
                {
                    RemoveSpeaker(() =>
                    {
                        dialogueSpeaker = Instantiate(reply.nextMessage.dialogueMessages[0].dialogueSpeakerPrefab, speakerContainer);
                        dialogueSpeaker.InitSpeaker(dialoguePanel);

                        AnimateSpeaker();
                    });
                }
                else
                {
                    dialogueSpeaker = Instantiate(reply.nextMessage.dialogueMessages[0].dialogueSpeakerPrefab, speakerContainer);
                    dialogueSpeaker.InitSpeaker(dialoguePanel);

                    AnimateSpeaker();
                }
            }

            Initialiser.ChangeGamestate(GameState.Conversation);
        }

        private void OnDestroy()
        {
            dialoguePanel.onDialogueStarted -= OnDialogueStarted;
            dialoguePanel.onDialogueFinished -= OnDialogueFinished;

            dialogueReplyPanel.OnReplySelected -= OnReplySelected;
        }

        public void EndConversation(Action onComplete = null)
        {
            dialoguePanel.onDialogueStarted -= OnDialogueStarted;
            dialoguePanel.onDialogueFinished -= OnDialogueFinished;

            dialogueReplyPanel.OnReplySelected -= OnReplySelected;

            StartCoroutine(CloseConversation(() =>
            {
                dialogueSpeaker = null;
                Initialiser.ChangeGamestate(GameState.World);
                onComplete?.Invoke();
            }));
        }

        private void ShowDialogue(Dialogue dialogue)
        {
            if (dialogueReplyPanelShowing)
            {
                dialogueReplyPanel.CloseReplyPanel(() =>
                {
                    dialogueReplyPanelShowing = false;

                    dialoguePanelShowing = true;
                    dialoguePanel.OpenDialogueWindow(dialogue);
                });
            }
            else
            {
                dialoguePanelShowing = true;
                dialoguePanel.OpenDialogueWindow(dialogue);
            }
        }

        private void ShowReplies(DialogueReply[] replies)
        {
            if (dialoguePanelShowing)
            {
                dialoguePanel.CloseDialoguePanel(() =>
                {
                    dialoguePanelShowing = false;

                    dialogueReplyPanelShowing = true;
                    dialogueReplyPanel.OpenReplyWindow(replies);
                });
            }
            else
            {
                dialogueReplyPanelShowing = true;
                dialogueReplyPanel.OpenReplyWindow(replies);
            }
        }

        private void OnDialogueStarted(DialogueMessage dialogueMessage)
        {
            if (dialogueSpeaker == null || dialogueMessage.dialogueSpeakerPrefab.speakerName != dialogueSpeaker.speakerName)
            {
                if (dialogueSpeaker != null)
                {
                    RemoveSpeaker(() =>
                    {
                        dialogueSpeaker = Instantiate(dialogueMessage.dialogueSpeakerPrefab, speakerContainer);
                        dialogueSpeaker.InitSpeaker(dialoguePanel);

                        AnimateSpeaker();
                    });
                }
                else
                {
                    dialogueSpeaker = Instantiate(dialogueMessage.dialogueSpeakerPrefab, speakerContainer);
                    dialogueSpeaker.InitSpeaker(dialoguePanel);

                    AnimateSpeaker();
                }
            }
        }

        private void OnDialogueFinished(Dialogue dialouge)
        {
            if (dialouge.dialogueReplies != null && dialouge.dialogueReplies.Length > 0)
            {
                ShowReplies(dialouge.dialogueReplies);
            }
            else
            {
                EndConversation();
            }
        }

        private void OnReplySelected(DialogueReply reply, bool isLeave)
        {
            if (!string.IsNullOrEmpty(reply.replyAction) && !string.IsNullOrEmpty(reply.actionParams))
            {
                if (reply.replyAction == "ITEM")
                {
                    onItemEarned?.Invoke(reply.actionParams);
                }
            }

            if (reply.replyAction == "END GAME")
            {
                EndConversation(() =>
                {
                    Initialiser.Instance.EndGame();
                });
                return;
            }

            if (isLeave)
            {
                EndConversation();
            }
            else
            {
                if (reply != null && reply.nextMessage != null)
                {
                    ShowDialogue(reply.nextMessage);
                }
                else
                {
                    EndConversation();
                }
            }
        }

        private void AnimateSpeaker()
        {
            if (speakerMovementTween != null)
                speakerMovementTween.Kill();

            if (speakerBobTween != null)
                speakerBobTween.Kill();

            speakerContainer.anchoredPosition = new Vector3(speakerStartingXPosition, 8);

            Canvas.ForceUpdateCanvases();

            speakerMovementTween = speakerContainer.DOAnchorPosX(speakerEndingXPosition, 1f).SetEase(Ease.OutQuint).OnComplete(() =>
            {
                speakerBobTween = speakerContainer.DOAnchorPosY(-8f, 2f).SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad)
                .SetLink(this.gameObject);
            });
        }

        private IEnumerator CloseConversation(Action onComplete)
        {
            int total = 3;
            int completed = 0;

            RemoveSpeaker(() =>
            {
                dialogueSpeaker = null;
                completed++;
            });

            dialoguePanel.CloseDialoguePanel(() =>
            {
                completed++;
            });

            dialogueReplyPanel.CloseReplyPanel(() =>
            {
                completed++;
            });

            yield return new WaitUntil(() => completed >= total);

            Initialiser.ChangeGamestate(GameState.World);

            onComplete?.Invoke();
        }

        private void RemoveSpeaker(Action onComplete = null)
        {
            if (speakerMovementTween != null)
                speakerBobTween.Kill();

            speakerMovementTween = speakerContainer.DOAnchorPosX(speakerStartingXPosition, 1f).SetEase(Ease.OutQuint).OnComplete(() =>
            {
                speakerBobTween?.Kill();

                speakerMovementTween?.Kill();

                dialogueSpeaker.CleanUp();
                Destroy(dialogueSpeaker.gameObject);

                speakerContainer.anchoredPosition = new Vector3(speakerEndingXPosition, 8);

                onComplete?.Invoke();
            });
        }
    }
}