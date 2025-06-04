using System;
using UnityEngine;

namespace Wattle.Wild.Gameplay.Player
{
    [Serializable]
    public class MapSection : MonoBehaviour
    {
        [SerializeField] private SectionDoor[] sectionDoors;
        [SerializeField] private SpriteRenderer playerRefernece;
        [SerializeField] private Transform spawnLocation = null;

        private MapManager mapManager;

        public Transform GetSpawnLocation()
        {
            return spawnLocation;
        }

        public Bounds? GetPlayerReferenceBounds()
        {
            if (playerRefernece == null)
                return null;
            else
                return playerRefernece.bounds;
        }

        public void Init(MapManager mapManager, int index)
        {
            this.mapManager = mapManager;

            foreach (SectionDoor sectionDoor in sectionDoors)
            {
                sectionDoor.Init(this);
            }
        }

        public void EnterDoor(MapSection newSection)
        {
            mapManager.MoveSections(newSection);
        }

        public void ToggleDoors(bool enabled)
        {
            foreach (SectionDoor sectionDoor in sectionDoors)
            {
                sectionDoor.ToggleCollider(enabled);
            }
        }
    }
}

