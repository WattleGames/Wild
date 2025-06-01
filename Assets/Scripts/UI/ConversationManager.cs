using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Wattle.Wild.Infrastructure.Conversation;
using Wattle.Wild.Logging;
using Wattle.Wild.UI;

namespace Wattle.Wild.Gameplay.Conversation
{
    public class ConversationManager : MonoBehaviour
    {
        public event Action OnConversationStarted;
        public event Action OnConversationEnded;

        [Header("Panels")]
        [SerializeField] private UIDialoguePanel dialoguePanel;
        [SerializeField] private UIDialogueReplyPanel dialogueReplyPanel;

        [Header("Speaker")]
        [SerializeField] private Image speakerImage;
        [SerializeField] private RectTransform speakerContainer;

        [SerializeField] private Conversaion testConversation;

        private Tweener speakerMovementTween = null;
        private Tweener speakerBobTween = null;

        private bool dialoguePanelShowing = false;
        private bool dialogueReplyPanelShowing = false;

        private float speakerStartingXPosition;
        private float speakerEndingXPosition;

        private void OnEnable()
        {
            speakerImage.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartConversation(testConversation);

            }
        }


        public void StartConversation(Conversaion conversation)
        {
            dialoguePanel.onDialogueFinished += OnDialogueFinished;
            dialogueReplyPanel.OnReplySelected += OnReplySelected;

            speakerStartingXPosition = speakerContainer.anchoredPosition.x + speakerContainer.rect.width;
            speakerEndingXPosition = speakerContainer.anchoredPosition.x;

            if (speakerBobTween != null)
                speakerBobTween.Kill();

            if (conversation.startingStage is Dialogue dialogue)
            {
                ShowDialogue(dialogue);
            }
            else if (conversation.startingStage is DialogueReply reply)
            {
                ShowReplies(new DialogueReply[] { reply });
            }

            OnConversationStarted?.Invoke();
        }

        public void EndConversation()
        {
            dialoguePanel.onDialogueFinished -= OnDialogueFinished;
            dialogueReplyPanel.OnReplySelected -= OnReplySelected;

            StartCoroutine(RemoveSpeaker(() =>
            {
                LOG.Log("Conversation ended!");
                OnConversationEnded?.Invoke();
            }));
        }

        private void ShowDialogue(Dialogue dialogue)
        {
            if (speakerImage.sprite == null || dialogue.speakerPortrait.name != speakerImage.sprite.name)
            {
                // cancel speaker animations / tweens here
                // transition speakers here?
                speakerImage.sprite = dialogue.speakerPortrait;
                speakerImage.gameObject.SetActive(true);

                AnimateSpeaker();

            }

            if (dialogueReplyPanelShowing)
            {
                dialogueReplyPanel.CloseReplyPanel(() =>
                {
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
            if (isLeave)
            {
                EndConversation();
            }
            else if (reply.nextMessage != null)
            {
                ShowDialogue(reply.nextMessage);
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
                .SetEase(Ease.InOutQuad);
            });
        }

        private IEnumerator RemoveSpeaker(Action onComplete = null)
        {
            int total = 3;
            int completed = 0;

            if (speakerMovementTween != null)
                speakerBobTween.Kill();

            speakerMovementTween = speakerContainer.DOAnchorPosX(speakerStartingXPosition, 1f).SetEase(Ease.OutQuint).OnComplete(() =>
            {
                if (speakerBobTween != null)
                    speakerBobTween.Kill();

                if (speakerMovementTween != null)
                    speakerMovementTween.Kill();

                speakerImage.sprite = null;
                speakerImage.gameObject.SetActive(false);

                speakerContainer.anchoredPosition = new Vector3(speakerEndingXPosition, 8);

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

            onComplete?.Invoke();
        }
    }
}