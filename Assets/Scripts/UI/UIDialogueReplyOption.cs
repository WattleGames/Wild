using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wattle.Utils;
using Wattle.Wild.Infrastructure.Conversation;

namespace Wattle.Wild.UI
{
    public class UIDialogueReplyOption : MonoBehaviour
    {
        public static event Action<DialogueReply, bool> OnOptionSelected;

        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI optionText;
        [SerializeField] private RectTransform content;
        [SerializeField] private RectTransform container;

        [SerializeField] private Image backgroundImage;

        [SerializeField] private Sprite leaveSprite;
        [SerializeField] private Sprite standardSprite;

        private float xMin = 5f;
        private float xMax = 10f;
        private float yMin = 5f;
        private float yMax = 10f;

        public float durationMin = 1.5f;
        public float durationMax = 3.5f;

        private DialogueReply reply;
        private bool isLeave;

        private Vector2 endingPosition;
        private Vector2 targetPosition;

        public void Init(DialogueReply reply, bool isLeave)
        {
            this.reply = reply;
            this.isLeave = isLeave;

            backgroundImage.sprite = isLeave ? leaveSprite : standardSprite;
            button.targetGraphic = backgroundImage;

            optionText.text = reply.replyText;

            endingPosition = this.content.rect.position;
            content.anchoredPosition = content.anchoredPosition.WithX(-300);
        }

        public void StartFloat()
        {
            // Randomize target offsets and durations
            float xOffset = UnityEngine.Random.Range(xMin, xMax) * (UnityEngine.Random.value > 0.5f ? 1 : -1);
            float yOffset = UnityEngine.Random.Range(yMin, yMax) * (UnityEngine.Random.value > 0.5f ? 1 : -1);
            float xDuration = UnityEngine.Random.Range(durationMin, durationMax);
            float yDuration = UnityEngine.Random.Range(durationMin, durationMax);

            // Horizontal float
            container.DOAnchorPosX(container.anchoredPosition.x + xOffset, xDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine).SetLink(this.gameObject);

            // Vertical float
            container.DOAnchorPosY(container.anchoredPosition.y + yOffset, yDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine).SetLink(this.gameObject);
        }

        public void PlayEnterAnimation()
        {
            content.DOAnchorPosX(0, 1f)
                .SetEase(Ease.OutCubic).SetLink(this.gameObject);
        }

        public Tweener PlayExitAnimation()
        {
            return content.DOAnchorPosX(-300, 0.5f)
                .SetEase(Ease.OutCubic).SetLink(this.gameObject);
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
