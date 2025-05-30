using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using PlasticGui;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Gamble.BattleCards.Tools
{
    [InitializeOnLoad]
    public class SceneSleectorToolBar
    {
        private static string[] sceneNames;
        private static int index = 0;

        static SceneSleectorToolBar()
        {
            EditorBuildSettings.sceneListChanged += GenerateSceneDropDown;
            EditorSceneManager.sceneOpened += SetDropdownIndex;

            GenerateSceneDropDown();
            SetDropdownIndex(EditorSceneManager.GetActiveScene());

            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }

        ~SceneSleectorToolBar()
        {
            EditorBuildSettings.sceneListChanged -= GenerateSceneDropDown;
            EditorSceneManager.sceneOpened -= SetDropdownIndex;
        }

        private static void SetDropdownIndex(Scene scene, OpenSceneMode mode = OpenSceneMode.Single)
        {
            index = sceneNames.Contains(scene.name) ? Array.IndexOf(sceneNames, scene.name) : 0;
        }

        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 100;

            int dropDownIndex = EditorGUILayout.Popup(index, sceneNames);

            if (dropDownIndex != index)
            {
                index = dropDownIndex;
                if (index != 0)
                {
                    string sceneName = sceneNames[index];

                    SceneHelper.StartScene(sceneName);
                }
            }
        }

        private static void GenerateSceneDropDown()
        {
            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

            sceneNames ??= new string[0];
            ArrayUtility.Add(ref sceneNames, "Select Scene");
            ArrayUtility.AddRange(ref sceneNames, Array.ConvertAll(buildScenes, x => System.IO.Path.GetFileNameWithoutExtension(x.path)));
        }

        static class SceneHelper
        {
            static string sceneToOpen;

            public static void StartScene(string sceneName)
            {
                if (EditorApplication.isPlaying)
                {
                    EditorApplication.isPlaying = false;
                }

                sceneToOpen = sceneName;
                EditorApplication.update += OnUpdate;
            }

            static void OnUpdate()
            {
                if (sceneToOpen == null ||
                    EditorApplication.isPlaying || EditorApplication.isPaused ||
                    EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    return;
                }

                EditorApplication.update -= OnUpdate;

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    // need to get scene via search because the path to the scene
                    // file contains the package version so it'll change over time
                    string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
                    if (guids.Length == 0)
                    {
                        Debug.LogWarning("Couldn't find scene file");
                    }
                    else
                    {
                        string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                        EditorSceneManager.OpenScene(scenePath);
                    }
                }
                sceneToOpen = null;
            }
        }
    }

    
}
