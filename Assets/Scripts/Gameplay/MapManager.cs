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
    [Serializable]
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
        TAVERN,
        FAMHOUSE,
        GRAVEYARD,
        CHURCH,
        SCHOOL,
        FACTORY,
        HOSPITAL,
        CARNIVAL,
        CAMP,
        LIBRARY,
        CASTLE,
        STARTING_AREA
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
        private Vector3 lastPlayerPosition;

        public MapSectionDetails GetCurrentMapSection()
        { 
            return currentSection;
        }

        public void LoadMap(MapSectionLocation startingLocation, bool isDev)
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

            if (!isDev)
                worldPlayer.MoveToNewSection(currentSection, false);
            else
                worldPlayer.transform.position = currentSection.mapSection.transform.position;

            lastPlayerPosition = Vector3.zero;

            currentSection.mapSection.ToggleDoors(true);
        }

        public void MoveSections(MapSection newSection)
        {
            currentSection.mapSection.ToggleDoors(false);

            MapSectionDetails newSectionDetails = GetSectionDetailsFromSection(newSection);

            Initialiser.ChangeGamestate(GameState.WorldTransition);

            if (IsSectionOnWorldMap(currentSection) && IsSectionOnWorldMap(newSectionDetails))
            {
                worldPlayer.MoveIntoNewSection(newSectionDetails, () =>
                {
                    currentSection = newSectionDetails;
                    Initialiser.ChangeGamestate(GameState.World);
                    newSection.ToggleDoors(true);
                });

                MoveCameraToNewSection(newSection);
            }
            else
            {
                UILoading.ShowScreen(() =>
                {
                    bool isLocation = !IsSectionOnWorldMap(newSectionDetails);
                    bool areBothLocations = isLocation && !IsSectionOnWorldMap(currentSection);

                    if (!areBothLocations && isLocation)
                        lastPlayerPosition = worldPlayer.transform.position;
                    else
                        worldPlayer.transform.position = lastPlayerPosition;

                    worldPlayer.MoveToNewSection(newSectionDetails, isLocation);

                    SetCameraToNewPosition(newSection);
                    newSection.ToggleDoors(true);

                    UILoading.HideScreen(() =>
                    {
                        currentSection = newSectionDetails;
                        Initialiser.ChangeGamestate(GameState.World);
                    });
                });
            }
        }

        public void MoveToLocation(string location)
        {

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
                MapSectionLocation.TAVERN => false,
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
                MapSectionLocation.STARTING_AREA => false,
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