using UnityEngine;
using Wattle.Wild.Gameplay.Player;
using Wattle.Wild.Logging;

namespace Wattle.Wild.Gameplay
{
    public class SectionDoor : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D colliderDoor;
        [SerializeField] private MapSection destinationSection;

        private MapSection parentSection = null;

        public void Init(MapSection parentSection)
        {
            this.parentSection = parentSection;
        }

        public void ToggleCollider(bool enabled)
        {
            colliderDoor.enabled = enabled;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out WorldPlayer worlPlayer))
            {
                if (destinationSection != null)
                {
                    parentSection.EnterDoor(destinationSection);
                }
            }
        }
    }
}

