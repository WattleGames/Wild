using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Wattle.Wild.Audio;
using Wattle.Wild.Infrastructure;
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
        public event Action<DialogueMessage> onDialogueStarted;
        public event Action<Dialogue> onDialogueFinished;

        public event Action<float, string> onDialogueSpoken;
        public event Action onDialogueEnded;

        [SerializeField] private RectTransform messageBox;

        [SerializeField] private TextMeshProUGUI speakerName;

        [SerializeField] private TextMeshProUGUI draftMessageText;
        [SerializeField] private TextMeshProUGUI messageText;

        private Coroutine messageCorutine = null;
        private Tweener moveTween;
        private Tweener scaleTween;

        private int messageIndex = 0;

        private bool isActive = false;
        private bool isTextAnimating = false;

        private Dialogue dialogue;
        private AudioClip characterAudioBack;
        private AudioClip voiceline;

        private const float DEFAULT_TEXT_SPEED = 0.05f;

        private readonly Vector2 POPUP_START_POSITION = new(163, -93);
        private readonly Vector2 POPUP_END_POSITION = new(250, 50);

        private bool isVoicelinePlaying = false;
        private AudioInstance voicelineAudioInstance;

        public void OpenDialogueWindow(Dialogue dialogue)
        {
            messageIndex = 0;

            this.dialogue = dialogue;
            this.voiceline = dialogue.dialogueMessages[messageIndex].voiceLine;

            speakerName.text = dialogue.dialogueMessages[messageIndex].dialogueSpeakerPrefab.speakerName;
            messageText.text = string.Empty;

            if (!isActive)
            {
                messageBox.anchoredPosition = POPUP_START_POSITION;
                messageBox.localScale = new Vector3(0, 0, 0);
            }

            isActive = true;

            CancelSpeechCoroutine();

            onDialogueStarted?.Invoke(dialogue.dialogueMessages[messageIndex]);

            ShowPopup(dialogue.dialogueStyle.popUpSpeed, dialogue.dialogueStyle.messageEase, () =>
            {
                DisplayDialogue();
            });
        }

        private void DisplayDialogue()
        {
            DialogueMessage message = dialogue.dialogueMessages[messageIndex];

            messageText.text = string.Empty;
            draftMessageText.text = message.dialogueText;

            Canvas.ForceUpdateCanvases();

            messageText.fontSize = draftMessageText.fontSize;

            DisplayMessageText(message.dialogueText, dialogue.dialogueStyle.isTimed, () =>
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
            CancelSpeechCoroutine();

            if (dialogue != null && dialogue.dialogueStyle.popUpSpeed == 0)
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
                messageCorutine = StartCoroutine(AnimateTextAndPlayAudio(text, onComplete));
            else
            {
                messageText.text = text;
                onComplete?.Invoke();
            }
        }

        private void CancelSpeechCoroutine()
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

            scaleTween = messageBox.DOScale(endScale, speed).SetAutoKill().SetEase(ease).SetLink(this.gameObject);
            moveTween = messageBox.DOAnchorPos(endPos, speed).SetAutoKill().SetEase(ease).SetLink(this.gameObject);

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

        private IEnumerator AnimateTextAndPlayAudio(string text, Action onComplete)
        {
            isTextAnimating = true;

            bool isProcessing = false;
            int startIndex = -1;
            float speed = DEFAULT_TEXT_SPEED;
            float lettersInWord = 0f;

            for (int index = 0; index < text.Length; ++index)
            {
                char character = text[index];
                lettersInWord++;

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

                if (dialogue.dialogueMessages[messageIndex].isSpoken)
                {
                    if (character == ' ')
                    {
                        onDialogueSpoken?.Invoke(speed * lettersInWord, dialogue.dialogueMessages[messageIndex].speaker);
                        lettersInWord = 0;
                    }

                    if (dialogue.dialogueMessages[messageIndex].voiceLine == null)
                        PlayTextCharacterAudio(character);
                    else
                        PlayVoicelineAudio();
                }

                yield return new WaitForSeconds(speed);
            }

            isTextAnimating = false;
            onComplete?.Invoke();
        }

        private void PlayVoicelineAudio()
        {
            if (!isVoicelinePlaying)
            {
                isVoicelinePlaying = true;
                voicelineAudioInstance = AudioManager.Play(dialogue.dialogueMessages[messageIndex].voiceLine, Vector3.zero, Audio.AudioType.VOICE, () =>
                {
                    isVoicelinePlaying = false;
                });
            }
        }

        private void PlayTextCharacterAudio(char character)
        {
            if (!char.IsLetter(character))
                return;

            int charCode = (int)character;

            int maxValue = (int)'z'; // wont work for secial characters

            if (charCode <= maxValue)
            {
                float scalar = 1 - (0.2f * (charCode / (float)maxValue));

                AudioClip audioClip = dialogue.dialogueMessages[messageIndex].dialogueSpeakerPrefab.GetFallbackVoice(dialogue.dialogueMessages[messageIndex].speaker);
                AudioManager.Play(audioClip, Vector3.zero, Audio.AudioType.VOICE);
            }
        }

        private void Update()
        {
            if (!isActive)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                if (isVoicelinePlaying)
                {
                    StartCoroutine(voicelineAudioInstance.CleanUp(() =>
                    {
                        voicelineAudioInstance = null;
                        isVoicelinePlaying = false;
                    }));
                }

                if (!isTextAnimating && (!moveTween.active && !scaleTween.active))
                {
                    if (messageIndex == dialogue.dialogueMessages.Length - 1)
                    {
                        // we're done, signal to close the panel
                        onDialogueFinished?.Invoke(dialogue);
                        isActive = false;
                    }
                    else
                    {
                        messageIndex += 1;
                        onDialogueStarted?.Invoke(dialogue.dialogueMessages[messageIndex]);

                        DisplayDialogue();
                    }
                }
                else
                {
                    // if playing, skip the voice line and text scrolling
                    CancelSpeechCoroutine();
                    messageText.text = draftMessageText.text;
                }
            }
        }
    }
}