using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Wattle.Utils;
using Wattle.Wild.Infrastructure;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private Button handleButton;
    [SerializeField] private RectTransform rectTransform;

    private bool inventoryOpened = false;
    private Tweener movingTween;
    private bool canOpenInventory = true;

    private void OnEnable()
    {
        Initialiser.OnGameStateChanged += OnGameStateChanged;

        WorldInteraction.OnWorldInteractionEntered += OnInteractionEntered;
        WorldInteraction.OnWorldInteractionExited += OnInteractionExited;

        handleButton.onClick.AddListener(HandleButtonClick);

        rectTransform.anchoredPosition = rectTransform.anchoredPosition.WithY(-118f);
    }

    private void OnDisable()
    {
        Initialiser.OnGameStateChanged -= OnGameStateChanged;

        WorldInteraction.OnWorldInteractionEntered -= OnInteractionEntered;
        WorldInteraction.OnWorldInteractionExited -= OnInteractionExited;

        handleButton.onClick.RemoveListener(HandleButtonClick);

        movingTween?.Kill();
    }

    private void OnGameStateChanged(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Conversation:
            case GameState.WorldTransition:
                Toggle(false);
                break;
        }
    }

    private void OnInteractionEntered()
    {
        canOpenInventory = false;
    }

    private void OnInteractionExited()
    {
        canOpenInventory = true;
    }

    private void HandleButtonClick()
    {
        if (canOpenInventory)
            Toggle(!inventoryOpened);
    }

    public void Toggle(bool enabled)
    {
        movingTween?.Kill();

        //open the inventory
        movingTween = rectTransform.DOAnchorPosY(enabled ? -220f : -118f, 1f).SetEase(Ease.OutElastic);
        inventoryOpened = enabled;
    }
}
