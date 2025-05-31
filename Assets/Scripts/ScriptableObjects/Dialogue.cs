using DG.Tweening;
using UnityEngine;
using Wattle.Wild.Infrastructure.Conversation;

[CreateAssetMenu(fileName = "Message Details", menuName = "Scriptable Objects/Dialogue/Dialogue", order = 1)]
public class Dialogue : ConversationStage
{
    [Header("Message Details")]
    [TextArea(2, 20)] public string[] dialogueText;

    // Message style
    public DialogueStyle dialogueStyle;
    public DialogueReply[] dialogueReplies;
}