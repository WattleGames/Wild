using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Wattle.Wild.Infrastructure.Conversation;
using Wattle.Wild.UI;

public class UISpeakerTwins : UIDialogueSpeaker
{
    [Header("Right Twin")]
    [SerializeField] private RectTransform upperMouthRight;
    [SerializeField] private RectTransform lowerMouthRight;

    [SerializeField] private bool moveUpperMouthRight = true;
    [SerializeField] private bool moveLowerMouthRight = true;

    [SerializeField] private AudioClip fallbackVoiceRight;

    public float rightUpperMouthMovementRange = 2;
    public float rightLowerMouthMovementRange = 2;

    private const string LEFT_TWIN = "LEFT TWIN";
    private const string RIGHT_TWIN = "RIGHT TWIN";

    private Tweener upperMouthTweenRight = null;
    private Tweener lowerMouthTweenRight = null;

    private string currentSpeaker;
    private int directionRight = -1;


    protected override void OnDialogueSpoken(float speed, string speaker)
    {
        if (speaker == LEFT_TWIN)
        {
            base.OnDialogueSpoken(speed, speaker);
        }
        else if (speaker == RIGHT_TWIN)
        {
            directionRight *= -1;

            if (upperMouthTweenRight != null)
                upperMouthTweenRight.Kill(true);

            if (moveUpperMouthRight)
            {
                upperMouthTweenRight = upperMouthRight.DOAnchorPosY(2, speed / 2.0f).OnComplete(() =>
                {
                    upperMouthTweenRight = upperMouthRight.DOAnchorPosY(-2, speed / 2.0f).OnComplete(() =>
                    {
                        upperMouthTweenRight.Kill(true);
                    });
                });
            }

            if (moveLowerMouthRight)
            {
                if (lowerMouthTweenRight != null)
                    lowerMouthTweenRight.Kill(true);

                lowerMouthTweenRight = lowerMouthRight.DOAnchorPosY(2 * directionRight, speed).OnComplete(() =>
                {
                    lowerMouthTweenRight.Kill(true);
                });
            }
        }
    }

    public override AudioClip GetFallbackVoice(string speaker)
    {
        if (speaker == LEFT_TWIN)
        {
            return base.GetFallbackVoice(speaker);
        }
        else
        {
            return fallbackVoiceRight;
        }
    }
}
