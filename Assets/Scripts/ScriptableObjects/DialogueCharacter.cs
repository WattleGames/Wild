using UnityEngine;

namespace Wattle.Wild.Infrastructure.Conversation
{
    [CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Dialogue/Character")]
    public class DialogueCharacter : ScriptableObject
    {
        public Sprite speakerPortrait;
        public Sprite speakerMouth;
        public Sprite speakerEyes;
    }
}