using DG.Tweening;
using System;
using UnityEngine;
using Wattle.Wild.Infrastructure;
using static Wattle.Wild.Gameplay.MapManager;

namespace Wattle.Wild.Gameplay.Player
{
    public class WorldPlayer : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Rigidbody2D rb;

        private Vector2 movement;
        private bool isInputEnabled = true;

        private (Vector2, MapSectionLocation) currentSectionDetails;

        void Update()
        {
            // Get input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

        }

        void FixedUpdate()
        {
            if (isInputEnabled)
            {
                // Apply movement
                rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
            }
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

            isInputEnabled = false;

            Tweener tween = rb.DOMove(newPosition, 0.35f).OnComplete(() =>
            {
                SaveSystem.Instance.SaveFile.playerLocation.Value = sectionDetails.location;

                onComplete?.Invoke();
                isInputEnabled = true;
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

