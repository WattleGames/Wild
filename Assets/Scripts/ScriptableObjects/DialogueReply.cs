using UnityEngine;

namespace Wattle.Wild.Infrastructure.Conversation
{
    [CreateAssetMenu(fileName = "Message Conversation", menuName = "Scriptable Objects/Dialogue/Dialogue Reply", order = 1)]
    public class DialogueReply : ConversationStage
    {
        [Header("Message Details")]
        [TextArea(2, 20)] public string replyText;

        public Dialogue nextMessage;
        // requirements?

        public string replyAction;
        public string actionParams;
    }
}