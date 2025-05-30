using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using Wattle.Utils;
using Wattle.Wild.Gameplay.Player;
using Wattle.Wild.Logging;

namespace Wattle.Wild.Gameplay
{
    public enum MapSectionLocation
    {
        TOP_LEFT,
        TOP_CENTER,
        TOP_RIGHT,
        CENTER_LEFT,
        CENTER,
        CENTER_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_CENTER,
        BOTTOM_RIGHT,
    }

    public class MapManager : MonoBehaviour
    {
        [Serializable]
        public struct MapSectionDetails
        {
            public MapSection mapSection;
            public MapSectionLocation location;
        }

        [SerializeField] private CinemachineCamera mapCamera; // at some point I might do the same thing for the camera as weve done with the player for moving
        [SerializeField] private MapSectionDetails[] mapSections;
        [SerializeField] private WorldPlayer worldPlayer;

        private Tweener cameraTween = null;

        public void LoadMap(MapSectionLocation startingLocation, Vector2? playerPosition)
        {
            // move the player to starting position
            for (int i = 0; i < mapSections.Length; ++i)
            {
                MapSectionDetails sectionDetails = mapSections[i];
                sectionDetails.mapSection.Init(this, i);
                sectionDetails.mapSection.ToggleDoors(false);
            }

            MapSectionDetails startSectionDetails = GetSectionDetailsFromLocation(startingLocation);

            mapCamera.transform.position = startSectionDetails.mapSection.transform.position.WithZ(-1);
            worldPlayer.MoveToNewSection(startSectionDetails, playerPosition);

            startSectionDetails.mapSection.ToggleDoors(true);
        }

        public void MoveSections(MapSection oldSection, MapSection newSection)
        {
            if (oldSection != null)
                LOG.Log($"FROM: {oldSection.name} TO: {newSection.name}", LOG.Type.GENERAL);
            else
                LOG.Log($"STARTING: {newSection.name}", LOG.Type.GENERAL);


            if (oldSection != null)
                oldSection.ToggleDoors(false);

            worldPlayer.MoveIntoNewSection(GetSectionDetailsFromSection(newSection), () =>
            {
                newSection.ToggleDoors(true);
            });

            MoveCameraToNewSection(newSection);
        }

        private MapSectionDetails GetSectionDetailsFromLocation(MapSectionLocation location)
        {
            return mapSections.First(x => x.location == location);
        }

        private MapSectionDetails GetSectionDetailsFromSection(MapSection mapSection)
        {
            return mapSections.First(x => x.mapSection == mapSection);
        }

        private void MoveCameraToNewSection(MapSection newSection)
        {
            if (cameraTween != null)
            {
                cameraTween.Kill();
                cameraTween = null;
            }

            cameraTween = mapCamera.transform.DOMove(newSection.transform.position.WithZ(-1), 0.3f).SetLink(this.gameObject);
        }
    }
}