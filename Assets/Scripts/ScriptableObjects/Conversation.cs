using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Wattle.Wild.Infrastructure.Conversation
{
    [CreateAssetMenu(fileName = "Message Conversation", menuName = "Scriptable Objects/Dialogue/Conversation", order = 1)]
    public class Conversaion : ScriptableObject
    {
        public ConversationStage startingStage;
        public AudioClip audioClip;
    }

    public abstract class ConversationStage : ScriptableObject
    {
        [Header("Speaker")]
        public UIDialogueSpeaker dialogueSpeakerPrefab;
        public AudioClip voiceLine;
    }
}

