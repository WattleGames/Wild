using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using Wattle.Utils;
using Wattle.Wild.Gameplay.Conversation;
using Wattle.Wild.Infrastructure;
using static Wattle.Wild.Gameplay.MapManager;

namespace Wattle.Wild.Gameplay.Player
{
    public class WorldPlayer : MonoBehaviour
    {
        public static event Action<string> OnPlayerItemCollected;

        public UIInventory InventoryUI => inventoryUI;

        [Header("World Systems")]
        [SerializeField] private ConversationManager conversationManager;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Rigidbody2D rb;

        [Header("Animation")]
        [SerializeField] private Animator playerAnimator;

        [Header("Inventory")]
        [SerializeField] private UIInventory inventoryUI;
        [SerializeField] private PlayerInventory inventory;

        private Vector2 movement;
        private bool isInputEnabled = true;

        private (Vector2, MapSectionLocation) currentSectionDetails;

        private string isMovingParameter = "isMoving";

        private Dictionary<string, bool> items = new Dictionary<string, bool>();

        private void OnEnable()
        {
            if (conversationManager != null)
            {
                conversationManager.onItemEarned += HandleItemEarned;
            }

            WorldInteraction.onItemCollected += HandleItemEarned;
            Initialiser.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            if (conversationManager != null)
            {
                conversationManager.onItemEarned -= HandleItemEarned;
            }

            WorldInteraction.onItemCollected -= HandleItemEarned;
            Initialiser.OnGameStateChanged -= OnGameStateChanged;
        }

        private void Update()
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

        private void FixedUpdate()
        {
            if (isInputEnabled)
            {
                // Apply movement
                rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
            }
        }

        private void OnGameStateChanged(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Conversation:
                case GameState.Paused:
                case GameState.WorldTransition:
                    ToggleMovement(false);
                    break;
                case GameState.World:
                    ToggleMovement(true);
                    break;
                default:
                    break;
            }
        }

        private void HandleItemEarned(string item)
        {
            TryAddItemToPlayer(item);
        }

        public void TryAddItemToPlayer(string itemName)
        {
            if (!items.ContainsKey(itemName))
            {
                items.Add(itemName, true);
            }
        }

        public bool CheckIfPlayerHasItem(string itemName)
        {
            if (items.ContainsKey(itemName))
            {
                return true;
            }

            return false;
        }

        public void ToggleMovement(bool enabled)
        {
            isInputEnabled = enabled;
            playerAnimator.SetBool(isMovingParameter, enabled);
        }

        public void MoveToNewSection(MapSectionDetails sectionDetails, bool isLocation)
        {
            this.currentSectionDetails = (new Vector2(sectionDetails.mapSection.transform.position.x, sectionDetails.mapSection.transform.position.y), sectionDetails.location);

            if (isLocation)
            {
                Vector3 position = sectionDetails.mapSection.GetSpawnLocation().position;
                rb.transform.position = new Vector3(position.x, position.y, this.transform.position.z);

                if (sectionDetails.location != MapSectionLocation.STARTING_AREA && sectionDetails.location != MapSectionLocation.CAVE)
                {
                    rb.transform.localScale = rb.transform.localScale.WithXY(2, 2);
                }
            }
            else
            {
                rb.transform.localScale = rb.transform.localScale.WithXY(1, 1);
            }
        }

        public void MoveIntoNewSection(MapSectionDetails sectionDetails, Action onComplete = null)
        {
            this.currentSectionDetails = (new Vector2(sectionDetails.mapSection.transform.position.x, sectionDetails.mapSection.transform.position.y), sectionDetails.location);

            Vector2 oldPosition = new Vector2(this.transform.position.x, this.transform.position.y);
            Vector2 newPosition = oldPosition + new Vector2(movement.x * 2, movement.y * 2);

            Tweener tween = rb.DOMove(newPosition, 0.35f).OnComplete(() =>
            {
                onComplete?.Invoke();
            }
            ).SetLink(this.gameObject);
        }
    }
}

