using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Wattle.Wild.Infrastructure.Conversation;
using Wattle.Wild.UI;

public class UIDialogueSpeaker : MonoBehaviour
{
    public string speakerName;
    public AudioClip FallbackVoice => fallbackVoice;

    [SerializeField] private Image characterPortrait;

    [SerializeField] private RectTransform upperMouth;
    [SerializeField] private RectTransform lowerMouth;
    [SerializeField] AudioClip fallbackVoice;

    public float upperMouthMovementRange = 2;
    public float lowerMouthMovementRange = 2;

    private bool isSpeaking = false;

    private float blinkRate = 0.0f;
    private UIDialoguePanel dialoguePanel;

    private Tweener upperMouthTween = null;
    private Tweener lowerMothTween = null;

    private int direction = -1;

    public void InitSpeaker(UIDialoguePanel dialoguePanel)
    {
        this.dialoguePanel = dialoguePanel;
        this.dialoguePanel.onDialogueSpoken += OnDialogueSpoken;
    }

    public void CleanUp()
    {
        dialoguePanel.onDialogueSpoken -= OnDialogueSpoken;
    }

    private void OnDialogueSpoken(float speed)
    {
        direction *= - 1;

        if (upperMouthTween != null)
            upperMouthTween.Kill(true);

        upperMouthTween = upperMouth.DOAnchorPosY(2, speed / 2.0f).OnComplete(() =>
        {
            upperMouthTween = upperMouth.DOAnchorPosY(-2, speed / 2.0f).OnComplete(() =>
            {
                upperMouthTween.Kill(true);
            });
        });

        if (lowerMothTween != null)
            lowerMothTween.Kill(true);

        lowerMothTween = lowerMouth.DOAnchorPosY(2 * direction, speed).OnComplete(() =>
        {
            lowerMothTween.Kill(true);
        });
    }
}
