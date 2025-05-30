using UnityEngine;

namespace Wattle.Wild.Gameplay.Player
{
    public class MapSection : MonoBehaviour
    {
        [SerializeField] private SectionDoor[] sectionDoors;
        private MapManager mapManager;
        private int index;

        public void Init(MapManager mapManager, int index)
        {
            this.mapManager = mapManager;
            this.index = index;

            foreach (SectionDoor sectionDoor in sectionDoors)
            {
                sectionDoor.Init(this);
            }
        }

        public void EnterDoor(MapSection newSection)
        {
            mapManager.MoveSections(this, newSection);
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

