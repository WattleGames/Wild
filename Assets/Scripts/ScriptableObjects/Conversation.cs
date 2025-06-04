using UnityEngine;

namespace Wattle.Wild.Infrastructure.Conversation
{
    [CreateAssetMenu(fileName = "Message Conversation", menuName = "Scriptable Objects/Dialogue/Conversation", order = 1)]
    public class Conversaion : ScriptableObject
    {
        public ConversationStage startingStage;
        public AudioClip conversationMusic;
    }

    public abstract class ConversationStage : ScriptableObject
    {

    }
}

