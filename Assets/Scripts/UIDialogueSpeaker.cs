using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Wattle.Wild.Infrastructure.Conversation;
using Wattle.Wild.UI;

public class UIDialogueSpeaker : MonoBehaviour
{
    public string speakerName;

    [SerializeField] private Image characterPortrait;
    [SerializeField] private AudioClip fallbackVoice;

    [SerializeField] private RectTransform upperMouth;
    [SerializeField] private RectTransform lowerMouth;

    [SerializeField] private bool moveUpperMouth = true;
    [SerializeField] private bool moveLowerMouth = true;

    public float upperMouthMovementRange = 2;
    public float lowerMouthMovementRange = 2;

    private bool isSpeaking = false;

    protected UIDialoguePanel dialoguePanel;

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

    public virtual AudioClip GetFallbackVoice(string speaker)
    {
        return fallbackVoice;
    }

    protected virtual void OnDialogueSpoken(float speed, string speaker)
    {
        direction *= - 1;

        if (moveUpperMouth)
        {
            if (upperMouthTween != null)
                upperMouthTween.Kill(true);

            upperMouthTween = upperMouth.DOAnchorPosY(2 * -direction, speed / 2.0f).OnComplete(() =>
            {
                upperMouthTween.Kill(true);
            }
            ).SetLink(this.gameObject);
        }

        if (moveLowerMouth)
        {
            if (lowerMothTween != null)
                lowerMothTween.Kill(true);

            lowerMothTween = lowerMouth.DOAnchorPosY(2 * direction, speed).OnComplete(() =>
            {
                lowerMothTween.Kill(true);
            }
            ).SetLink(this.gameObject);
        }
    }
}
