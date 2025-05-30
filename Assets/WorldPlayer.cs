using DG.Tweening;
using System;
using UnityEngine;

namespace Wattle.Wild.Gameplay.Player
{
    public class WorldPlayer : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Rigidbody2D rb;

        private Vector2 movement;
        private bool isInputEnabled = true;

        void Update()
        {
            // Get input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Space))
            {

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

        public void Nudge(Action onComplete = null)
        {

            Vector2 oldPosition = new Vector2(this.transform.position.x, this.transform.position.y);
            Vector2 newPosition = oldPosition + new Vector2(movement.x * 2, movement.y * 2);

            isInputEnabled = false;

            Tweener tween = rb.DOMove(newPosition, 0.35f).OnComplete(() =>
            {
                onComplete?.Invoke();
                isInputEnabled = true;
            });

        }
    }
}

