using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using Wattle.Utils;
using Wattle.Wild.Gameplay.Player;
using Wattle.Wild.Logging;

public class MapManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera mapCamera;
    [SerializeField] private MapSection[] mapSections;
    [SerializeField] private WorldPlayer worldPlayer;

    private int currentIndex = 4;
    private Tweener cameraTween = null;

    private void OnEnable()
    {
        LoadMap();
    }

    public void LoadMap()
    {
        // move the player to starting position
        for (int i = 0; i < mapSections.Length; ++i)
        {
            MapSection section = mapSections[i];
            section.Init(this, i);
            section.ToggleDoors(false);
        }

        MapSection startSection = mapSections[currentIndex];
        MoveSections(null, startSection);

        worldPlayer.transform.position = startSection.transform.position;
    }

    public void MoveSections(MapSection oldSection, MapSection newSection)
    {
        if (oldSection != null)
            LOG.Log($"FROM: {oldSection.name} TO: {newSection.name}", LOG.Type.GENERAL);
        else
            LOG.Log($"STARTING: {newSection.name}", LOG.Type.GENERAL);


        if (oldSection != null)
            oldSection.ToggleDoors(false);

        worldPlayer.Nudge(() =>
        {
            newSection.ToggleDoors(true);
        });

        MoveCameraToNewSection(newSection);
    }

    private void MoveCameraToNewSection(MapSection newSection)
    {
        if (cameraTween != null)
        {
            cameraTween.Kill();
            cameraTween = null;
        }

        cameraTween = mapCamera.transform.DOMove(newSection.transform.position.WithZ(-1), 0.3f);
    }
}
