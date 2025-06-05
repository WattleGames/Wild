using System;
using UnityEngine;

namespace Wattle.Wild.Infrastructure.Conversation
{
    [CreateAssetMenu(fileName = "Message Details", menuName = "Scriptable Objects/Dialogue/Dialogue", order = 1)]
    public class Dialogue : ConversationStage
    {
        [Header("Messages")]
        public DialogueMessage[] dialogueMessages;

        public DialogueStyle dialogueStyle;
        public DialogueReply[] dialogueReplies;
    }

    [Serializable]
    public struct DialogueMessage
    {
        [Header("Speaker")]
        public UIDialogueSpeaker dialogueSpeakerPrefab;
        public AudioClip voiceLine;
        public bool isSpoken;

        public string speaker;

        [Header("Message")]
        [TextArea(2, 20)] public string dialogueText;
    }
}