using Unity.VisualScripting;
using UnityEngine;
using Wattle.Wild.Gameplay.Conversation;
using Wattle.Wild.Infrastructure.Conversation;

public class TestConversation : MonoBehaviour
{
    [SerializeField] ConversationManager convo;
    [SerializeField] Conversaion conversation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            convo.StartConversation(conversation);
        }
    }
}
