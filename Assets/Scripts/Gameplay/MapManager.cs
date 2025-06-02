using DG.Tweening;
using System;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using Wattle.Utils;
using Wattle.Wild.Gameplay.Player;
using Wattle.Wild.Infrastructure;
using Wattle.Wild.Logging;
using Wattle.Wild.UI;

namespace Wattle.Wild.Gameplay
{
    public enum MapSectionLocation
    {
        // World Map
        TOP_LEFT,
        TOP_CENTER,
        TOP_RIGHT,
        CENTER_LEFT,
        CENTER,
        CENTER_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_CENTER,
        BOTTOM_RIGHT,

        // Locations
        WATERFALL,
        CAVE,
        SALOON,
        FAMHOUSE,
        GRAVEYARD,
        CHURCH,
        SCHOOL,
        FACTORY,
        HOSPITAL,
        CARNIVAL,
        CAMP,
        LIBRARY,
        CASTLE
    }

    public class MapManager : MonoBehaviour
    {
        [Serializable]
        public struct MapSectionDetails
        {
            public MapSection mapSection;
            public MapSectionLocation location;
            public AudioClip sectionAudio;
        }

        [SerializeField] private CinemachineCamera mapCamera; // at some point I might do the same thing for the camera as weve done with the player for moving
        [SerializeField] private MapSectionDetails[] mapSections;
        [SerializeField] private WorldPlayer worldPlayer;

        private Tweener cameraTween = null;
        private MapSectionDetails currentSection;

        public MapSectionDetails GetCurrentMapSection()
        { 
            return currentSection;
        }

        public void LoadMap(MapSectionLocation startingLocation)
        {
            // move the player to starting position
            for (int i = 0; i < mapSections.Length; ++i)
            {
                MapSectionDetails sectionDetails = mapSections[i];
                sectionDetails.mapSection.Init(this, i);
                sectionDetails.mapSection.ToggleDoors(false);
            }

            MapSectionDetails startSectionDetails = GetSectionDetailsFromLocation(startingLocation);
            currentSection = startSectionDetails;

            mapCamera.transform.position = currentSection.mapSection.transform.position.WithZ(-1);
            worldPlayer.MoveToNewSection(currentSection);

            currentSection.mapSection.ToggleDoors(true);
        }

        public void MoveSections(MapSection oldSection, MapSection newSection)
        {
            if (oldSection != null)
                LOG.Log($"FROM: {oldSection.name} TO: {newSection.name}", LOG.Type.GENERAL);
            else
                LOG.Log($"STARTING: {newSection.name}", LOG.Type.GENERAL);


            if (oldSection != null)
                oldSection.ToggleDoors(false);

            currentSection = GetSectionDetailsFromSection(newSection);

            Initialiser.ChangeGamestate(GameState.WorldTransition);

            if (IsSectionOnWorldMap(currentSection))
            {
                worldPlayer.MoveIntoNewSection(currentSection, () =>
                {
                    Initialiser.ChangeGamestate(GameState.World);
                    newSection.ToggleDoors(true);
                });

                MoveCameraToNewSection(newSection);
            }
            else
            {
                UILoading.ShowScreen(() =>
                {
                    worldPlayer.MoveToNewSection(currentSection);
                    SetCameraToNewPosition(newSection);

                    UILoading.HideScreen(() =>
                    {
                        Initialiser.ChangeGamestate(GameState.World);
                    });
                });
            }
        }

        private bool IsSectionOnWorldMap(MapSectionDetails sectionDetails)
        {
            bool isWorldMap = sectionDetails.location switch
            {
                // World Map
                MapSectionLocation.BOTTOM_LEFT => true,
                MapSectionLocation.BOTTOM_CENTER => true,
                MapSectionLocation.BOTTOM_RIGHT => true,
                MapSectionLocation.CENTER_LEFT => true,
                MapSectionLocation.CENTER => true,
                MapSectionLocation.CENTER_RIGHT => true,
                MapSectionLocation.TOP_LEFT => true,
                MapSectionLocation.TOP_CENTER => true,
                MapSectionLocation.TOP_RIGHT => true,

                // Locations
                MapSectionLocation.WATERFALL => false,
                MapSectionLocation.CAVE => false,
                MapSectionLocation.SALOON => false,
                MapSectionLocation.FAMHOUSE => false,
                MapSectionLocation.GRAVEYARD => false,
                MapSectionLocation.CHURCH => false,
                MapSectionLocation.SCHOOL => false,
                MapSectionLocation.FACTORY => false,
                MapSectionLocation.HOSPITAL => false,
                MapSectionLocation.CARNIVAL => false,
                MapSectionLocation.CAMP => false,
                MapSectionLocation.LIBRARY => false,
                MapSectionLocation.CASTLE => false,
                        _ => false
            };

            return isWorldMap;
        }

        private MapSectionDetails GetSectionDetailsFromLocation(MapSectionLocation location)
        {
            return mapSections.First(x => x.location == location);
        }

        private MapSectionDetails GetSectionDetailsFromSection(MapSection mapSection)
        {
            return mapSections.First(x => x.mapSection == mapSection);
        }

        private void SetCameraToNewPosition(MapSection newSection)
        {
            mapCamera.transform.position = newSection.transform.position.WithZ(-1);
        }

        private void MoveCameraToNewSection(MapSection newSection, Action onComplete = null)
        {
            if (cameraTween != null)
            {
                cameraTween.Kill();
                cameraTween = null;
            }

            cameraTween = mapCamera.transform.DOMove(newSection.transform.position.WithZ(-1), 0.3f).SetLink(this.gameObject).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
    }
}