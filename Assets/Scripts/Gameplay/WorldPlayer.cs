using DG.Tweening;
using System;
using UnityEngine;
using Wattle.Wild.Gameplay.Conversation;
using Wattle.Wild.Infrastructure;
using static Wattle.Wild.Gameplay.MapManager;

namespace Wattle.Wild.Gameplay.Player
{
    public class WorldPlayer : MonoBehaviour
    {
        [Header("World Systems")]
        [SerializeField] private ConversationManager conversationManager;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Rigidbody2D rb;

        [Header("Animation")]
        [SerializeField] private Animator playerAnimator;

        private Vector2 movement;
        private bool isInputEnabled = true;

        private (Vector2, MapSectionLocation) currentSectionDetails;

        private string isMovingParameter = "isMoving";

        private void OnEnable()
        {
            if (conversationManager != null)
            {
                conversationManager.OnConversationStarted += OnConversationStarted;
                conversationManager.OnConversationEnded += OnConversationEnded;
            }
        }

        private void OnDisable()
        {
            if (conversationManager != null)
            {
                conversationManager.OnConversationStarted += OnConversationStarted;
                conversationManager.OnConversationEnded += OnConversationEnded;
            }
        }

        void Update()
        {
            if (isInputEnabled)
            {
                // Get input
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");

                Vector2.ClampMagnitude(movement, 1);

                bool moving = movement != Vector2.zero;

                playerAnimator.SetBool(isMovingParameter, moving);
            }
        }

        void FixedUpdate()
        {
            if (isInputEnabled)
            {
                // Apply movement
                rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
            }
        }

        private void OnConversationStarted()
        {
            ToggleMovement(false);
        }

        private void OnConversationEnded()
        {
            ToggleMovement(true);
        }

        public void ToggleMovement(bool enabled)
        {
            isInputEnabled = enabled;
            playerAnimator.SetBool(isMovingParameter, enabled);
        }

        public void MoveToNewSection(MapSectionDetails sectionDetails, Vector2? playerPosition)
        {
            this.currentSectionDetails = (new Vector2(sectionDetails.mapSection.transform.position.x, sectionDetails.mapSection.transform.position.y), sectionDetails.location);

            Vector2 position =
                playerPosition == null ? sectionDetails.mapSection.transform.position
                : new Vector2(sectionDetails.mapSection.transform.position.x + playerPosition.Value.x, sectionDetails.mapSection.transform.position.y + playerPosition.Value.y);

            rb.transform.position = new Vector3(position.x, position.y, this.transform.position.z);

            if (SaveSystem.Instance.SaveFile.playerLocation.Value != sectionDetails.location)
            {
                SaveSystem.Instance.SaveFile.playerLocation.Value = sectionDetails.location;
            }
        }

        public void MoveIntoNewSection(MapSectionDetails sectionDetails, Action onComplete = null)
        {
            this.currentSectionDetails = (new Vector2(sectionDetails.mapSection.transform.position.x, sectionDetails.mapSection.transform.position.y), sectionDetails.location);

            Vector2 oldPosition = new Vector2(this.transform.position.x, this.transform.position.y);
            Vector2 newPosition = oldPosition + new Vector2(movement.x * 2, movement.y * 2);

            Tweener tween = rb.DOMove(newPosition, 0.35f).OnComplete(() =>
            {
                SaveSystem.Instance.SaveFile.playerLocation.Value = sectionDetails.location;

                onComplete?.Invoke();
            }
            ).SetLink(this.gameObject);
        }

        private void OnApplicationQuit()
        {
            SavePlayerData();
        }

        private void SavePlayerData()
        {
            Vector2 positionOffset = new Vector2(
                    this.transform.position.x - currentSectionDetails.Item1.x,
                    this.transform.position.y - currentSectionDetails.Item1.y);

            SaveSystem.Instance.SaveFile.positionOffset = positionOffset;
            SaveSystem.Instance.SaveFile.playerLocation.Value = currentSectionDetails.Item2;

            SaveSystem.Instance.SaveFile.Save();
        }
    }
}

