using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Wattle.Utils;
using Wattle.Wild.Gameplay.Player;
using Wattle.Wild.Logging;

public class WorldInteraction : MonoBehaviour
{
    public static event Action OnWorldInteractionEntered;
    public static event Action OnWorldInteractionExited;

    [SerializeField] private UnityEvent onInteractionStarted;
    [SerializeField] private Image interactionSprite;
    [SerializeField] private Transform notificationTransform;

    private Vector3 targetScale = new Vector3(0.04f, 0.025f, 1);
    private float targetPos = 0.03f;

    private Tweener spriteTweener;
    private Tweener moveTweener;
    private Tweener idleTweener;

    private float speed = 0.4f;
    private bool isActive = false;

    private void OnEnable()
    {
        notificationTransform.localScale = Vector3.zero;
        notificationTransform.localPosition = notificationTransform.localPosition.WithY(0);
    }

    private void Update()
    {
        if (isActive)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                onInteractionStarted?.Invoke();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LOG.Log("Collioion detected", LOG.Type.UI);

        if (collision.gameObject.TryGetComponent(out WorldPlayer worldPlayer))
        {
            OnWorldInteractionEntered?.Invoke();

            worldPlayer.InventoryUI.Toggle(false);

            isActive = true;

            if (spriteTweener != null)
            {
                spriteTweener.Kill();
                spriteTweener = null;
            }

            if (moveTweener != null)
            {
                moveTweener.Kill();
                moveTweener = null;
            }

            PlayAnimation(true, () =>
            {
                idleTweener = notificationTransform.DOLocalMoveY(0.035f, 1f).SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);
            });
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out WorldPlayer worldPlayer))
        {
            isActive = false;

            OnWorldInteractionExited?.Invoke();

            if (spriteTweener != null)
            {
                spriteTweener.Kill();
                spriteTweener = null;
            }

            if (moveTweener != null)
            {
                moveTweener.Kill();
                moveTweener = null;
            }

            if (idleTweener != null)
            {
                idleTweener.Kill();
                idleTweener = null;
            }

            PlayAnimation(false);
        }
    }

    private void PlayAnimation(bool isEntering, Action onComplete = null)
    {
        spriteTweener = notificationTransform.DOScale(isEntering ? targetScale : Vector3.zero, speed).SetAutoKill().SetEase(Ease.InQuart);
        moveTweener = notificationTransform.DOLocalMoveY(isEntering ? targetPos : 0, speed).SetAutoKill().SetEase(Ease.InQuart);

        spriteTweener.onComplete += () =>
        {
            if (!moveTweener.active)
            {
                moveTweener.Kill();
                spriteTweener.Kill();

                onComplete?.Invoke();
            }
        };

        moveTweener.onComplete += () =>
        {
            if (!spriteTweener.active)
            {
                moveTweener.Kill();
                spriteTweener.Kill();

                onComplete?.Invoke();
            }
        };
    }
}
