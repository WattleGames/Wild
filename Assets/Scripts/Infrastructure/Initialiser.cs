using Wattle.Utils;
using System;
using Wattle.Wild.Gameplay;
using System.Collections;
using Wattle.Wild.UI;
using DG.Tweening;





#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Wattle.Wild.Infrastructure
{
    public enum GameState
    {
        MainMenu,
        World,
        WorldTransition,
        Conversation,
        Paused,
    }

    public class Initialiser : PersistentSingleton<Initialiser>
    {
        public static event Action<GameState> OnGameStateChanged;
        private static GameState gameState;

        [SerializeField] private SingletonManager singletonManager;
        [SerializeField] private CanvasGroup endCanvas;

#if UNITY_EDITOR
        [Header("Dev Tools")]
        [SerializeField] private bool sandboxMode = false;
        [SerializeField] private bool clearSave = false;
        [SerializeField] private MapSectionLocation startingLocation = MapSectionLocation.CENTER;
#endif

        public bool IsSandbox
        {
            get
            {
#if UNITY_EDITOR
                return sandboxMode;
#else
                return false;
#endif
            }
        }

        // Probably profile related stuff in here

        private void OnEnable()
        {
            InitialiseApplication();
        }

        public void InitialiseApplication()
        {
            singletonManager.InitialiseSingletons(() =>
            {
                bool loadDemo = false;

#if UNITY_EDITOR
                loadDemo = this.sandboxMode;
#endif

                if (!loadDemo)
                {
                    SceneManager.Instance.LoadScene("MainMenu", UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
                    {
                        ChangeGamestate(GameState.MainMenu);
                    });
                }
                else
                {
                    LoadSandbox();
                }
            });
        }

        public void EndGame()
        {
            StartCoroutine(StartEndSequence());

            IEnumerator StartEndSequence()
            {
                bool screenLoaded = false;

                AudioManager.PlayMusic(null);

                UILoading.ShowScreen(() =>
                {
                    screenLoaded = true;
                });

                yield return new WaitUntil(() =>  screenLoaded);

                yield return new WaitForSeconds(0.5f);

                yield return HelperFunctions.FadeCanvas(endCanvas, true, 0.4f);

                yield return new WaitForSeconds(0.4f);

                yield return HelperFunctions.FadeCanvas(endCanvas, false, 0.4f);

                SceneManager.Instance.LoadScene("MainMenu", UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
                {
                    ChangeGamestate(GameState.MainMenu);
                });
            }
        }

        private void LoadSandbox()
        {
            LoadGame();
        }

        public static void LoadGame()
        {
            MapSectionLocation location;
#if UNITY_EDITOR
            if (Instance.IsSandbox)
                location = Instance.startingLocation;
            else
                location = MapSectionLocation.STARTING_AREA;
#else
            location = MapSectionLocation.STARTING_AREA;
#endif

            SceneManager.Instance.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
            {
                MapManager mapManager = FindAnyObjectByType<MapManager>();
                if (mapManager != null)
                {
                    mapManager.LoadMap(location, Instance.IsSandbox);
                }

                ChangeGamestate(GameState.World);
            });
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static void ChangeGamestate(GameState state)
        {
            gameState = state;
            OnGameStateChanged?.Invoke(gameState);
        }
    }
}
