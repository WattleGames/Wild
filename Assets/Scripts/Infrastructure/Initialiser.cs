using Gamble.Wild.UI;
using Gamble.Utils;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Gamble.Wild.Infrastructure
{
    public enum GameState
    {
        MainMenu,
        Game
    }

    public class Initialiser : PersistentSingleton<Initialiser>
    {
        public static event Action<GameState> OnGameStateChanged;

        [SerializeField] private SingletonManager singletonManager;

#if UNITY_EDITOR
        [Header("Dev Tools")]
        [SerializeField] private bool sandboxMode = false;
#endif

        public bool IsSandbox 
        { 
            get 
            {
#if UNITY_EDITOR
                return sandboxMode;
#else
                return false
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
                        OnGameStateChanged?.Invoke(GameState.MainMenu);
                    });
                }
                else
                {
                    LoadSandbox();
                }
            });
        }

        private void LoadSandbox()
        {
            LoadGame();
        }

        public static void LoadGame()
        {
            SceneManager.Instance.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
            {
                OnGameStateChanged?.Invoke(GameState.Game);
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
    }
}
