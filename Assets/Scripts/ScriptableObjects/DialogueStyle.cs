
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Message Style", menuName = "Scriptable Objects/Dialogue/Dialogue Style", order = 1)]
public class DialogueStyle : ScriptableObject
{
    [Header("Timed Details")]
    public bool isTimed;

    [Header("Style Details")]
    public bool hasPrompts;
    public Ease messageEase;
    public float popUpSpeed;
}