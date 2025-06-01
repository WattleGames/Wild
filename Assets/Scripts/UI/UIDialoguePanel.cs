using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Wattle.Wild.Infrastructure.Conversation;

namespace Wattle.Wild.UI
{ 
    public struct PopupInfo
    {
        public PopupInfo(ConversationStage messageDetails, AudioClip textSound, float popupSpeed, Ease popupEase)
        {
            this.messageDetails = messageDetails;
            this.textSound = textSound;
            this.popupSpeed = popupSpeed;
            this.popupEase = popupEase;
        }

        public ConversationStage messageDetails;
        public AudioClip textSound;
        public float popupSpeed;
        public Ease popupEase;
    }

    public class UIDialoguePanel : MonoBehaviour
    {
        public event Action<Dialogue> onDialogueFinished;

        public event Action<float> onDialogueSpoken;
        public event Action onDialogueEnded;

        [SerializeField] private RectTransform messageBox;

        [SerializeField] private TextMeshProUGUI speakerName;

        [SerializeField] private TextMeshProUGUI draftMessageText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private AudioSource audioSource;

        private Coroutine messageCorutine = null;
        private Tweener moveTween;
        private Tweener scaleTween;

        private int messageIndex = 0;

        private bool isActive = false;
        private bool isTextAnimating = false;

        private Dialogue dialogue;

        private const float DEFAULT_TEXT_SPEED = 0.05f;

        private readonly Vector2 POPUP_START_POSITION = new(163, -93);
        private readonly Vector2 POPUP_END_POSITION = new(-125, 50);

        public void OpenDialogueWindow(Dialogue dialogue)
        {
            this.dialogue = dialogue;
            speakerName.text = dialogue.dialogueSpeakerPrefab.speakerName;
            messageText.text = string.Empty;

            if (!isActive)
            {
                messageBox.anchoredPosition = POPUP_START_POSITION;
                messageBox.localScale = new Vector3(0, 0, 0);
            }

            messageIndex = 0;
            isActive = true;

            CancelTextCorutine();

            ShowPopup(dialogue.dialogueStyle.popUpSpeed, dialogue.dialogueStyle.messageEase, () =>
            {
                DisplayDialogue(dialogue.dialogueText[messageIndex]);
            });
        }

        private void DisplayDialogue(string text)
        {
            messageText.text = string.Empty;
            draftMessageText.text = text;

            Canvas.ForceUpdateCanvases();

            messageText.fontSize = draftMessageText.fontSize;

            DisplayMessageText(text, dialogue.dialogueStyle.isTimed, () =>
            {

            });
        }

        private void ShowPopup(in float speed, Ease ease, Action onComplete)
        {
            if (speed == 0)
            {
                messageBox.anchoredPosition = new Vector3(POPUP_END_POSITION.x, POPUP_END_POSITION.y, messageBox.position.z);
                messageBox.localScale = new Vector3(1, 1, 1);
                onComplete?.Invoke();
            }
            else
            {
                TweenPopup(new Vector3(1, 1, 1), POPUP_END_POSITION, speed, ease, onComplete);
            }
        }

        public void CloseDialoguePanel(Action onComplete = null)
        {
            isActive = false;
            CancelTextCorutine();

            if (dialogue.dialogueStyle.popUpSpeed == 0)
            {
                messageBox.anchoredPosition = new Vector3(POPUP_END_POSITION.x, POPUP_END_POSITION.y, messageBox.position.z);
                messageBox.localScale = new Vector3(0, 0, 0);

                onComplete?.Invoke();
            }
            else
            {
                TweenPopup(Vector3.zero, POPUP_START_POSITION, dialogue.dialogueStyle.popUpSpeed, dialogue.dialogueStyle.messageEase, () =>
                {
                    isActive = false;
                    onComplete?.Invoke();
                });
            }
        }

        private void DisplayMessageText(string text, bool isTimed, Action onComplete)
        {
            messageText.text = string.Empty;

            if (isTimed)
                messageCorutine = StartCoroutine(DisplayMessageText(text, onComplete));
            else
            {
                messageText.text = text;
                onComplete?.Invoke();
            }
        }

        private void CancelTextCorutine()
        {
            if (messageCorutine != null)
            {
                StopCoroutine(messageCorutine);

                isTextAnimating = false;
                messageCorutine = null;
            }
        }

        private void TweenPopup(in Vector2 endScale, in Vector2 endPos, in float speed, Ease ease, Action onComplete)
        {
            if (moveTween != null)
            {
                moveTween.Kill();
                moveTween = null;
            }

            if (scaleTween != null)
            {
                scaleTween.Kill();
                scaleTween = null;
            }

            scaleTween = messageBox.DOScale(endScale, speed).SetAutoKill().SetEase(ease);
            moveTween = messageBox.DOAnchorPos(endPos, speed).SetAutoKill().SetEase(ease);

            scaleTween.onComplete += () =>
            {
                if (!moveTween.active)
                {
                    moveTween.Kill();
                    scaleTween.Kill();

                    onComplete?.Invoke();
                }
            };

            moveTween.onComplete += () =>
            {
                if (!scaleTween.active)
                {
                    moveTween.Kill();
                    scaleTween.Kill();

                    onComplete?.Invoke();
                }
            };
        }

        private IEnumerator DisplayMessageText(string text, Action onComplete)
        {
            isTextAnimating = true;

            bool isProcessing = false;
            int startIndex = -1;
            float speed = DEFAULT_TEXT_SPEED;

            for (int index = 0; index < text.Length; ++index)
            {
                char character = text[index];

                if (character == '{' || character == '[')
                {
                    isProcessing = true;
                    startIndex = index + 1;
                }
                else if (character == '}' || character == ']')
                {
                    if (character == '}')
                    {
                        speed = index - startIndex == 0 ? DEFAULT_TEXT_SPEED : float.Parse(text[startIndex..index]);
                    }
                    else
                    {
                        float waitTime = float.Parse(text[startIndex..index]);
                        yield return new WaitForSeconds(waitTime);
                    }

                    startIndex = -1;
                    isProcessing = false;
                    continue;
                }

                if (isProcessing)
                    continue;

                messageText.text = messageText.text.Insert(messageText.text.Length, character.ToString());
                onDialogueSpoken?.Invoke(speed);

                PlayTextCharacterAudio(character);
                yield return new WaitForSeconds(speed);
            }

            isTextAnimating = false;
            onComplete?.Invoke();
        }

        private void PlayTextCharacterAudio(char character)
        {
            if (!char.IsLetter(character))
                return;

            int charCode = (int)character;

            int maxValue = (int)'z'; // wont work for secial characters

            if (audioSource.isPlaying)
                audioSource.Stop();

            if (charCode <= maxValue)
            {
                float scalar = 1 - (0.2f * ((float)charCode / (float)maxValue));

                audioSource.pitch = 1 + scalar;

                audioSource.Play();
            }
        }

        private void Update()
        {
            if (!isActive)
                return;

            if (Input.GetKeyDown(KeyCode.Return))
            {

                if (!isTextAnimating && (!moveTween.active && !scaleTween.active))
                {
                    if (messageIndex == dialogue.dialogueText.Length - 1)
                    {
                        // we're done, signal to close the panel
                        onDialogueFinished?.Invoke(dialogue);
                    }
                    else
                    {
                        messageIndex += 1;
                        DisplayDialogue(dialogue.dialogueText[messageIndex]);
                    }
                }
                else
                {
                    // if playing, skip the voice line and text scrolling
                    CancelTextCorutine();
                    messageText.text = draftMessageText.text;
                }
            }
        }
    }
}