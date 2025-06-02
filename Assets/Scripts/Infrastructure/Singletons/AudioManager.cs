using Wattle.Wild.Audio;
using Wattle.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using AudioType = Wattle.Wild.Audio.AudioType;
using Wattle.Wild.Gameplay;

namespace Wattle.Wild.Infrastructure
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private Transform instanceParent;
        [SerializeField] private AudioInstance audioInstancePrefab;

        private ObjectPool<AudioInstance> instancePool;

        private int defaultCapacity = 5;
        private int maxCapacity = 100;

        private AudioInstance musicInstance = null;

        private GameState currentGameState;

        public override IEnumerator Initalise()
        {
            Initialiser.OnGameStateChanged += OnGameStateChanged;

            instancePool = new ObjectPool<AudioInstance>(
                createFunc: () => { return CreateAudioInstance(); },
                OnAudioInstanceRetrieved,
                OnAudioInstanceReleased,
                OnAudioInstanceDestroyed,
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxCapacity
                );

            musicInstance = Instantiate(audioInstancePrefab, instanceParent);

            initialised = true;

            yield return base.Initalise();
        }

        private void OnGameStateChanged(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    musicInstance.Load(ResourceManager.Instance.mainMenuMusic, AudioType.MUSIC);
                    break;
                case GameState.World:
                    {
                        MapManager mapManager = FindFirstObjectByType<MapManager>();

                        if (mapManager != null)
                        {
                            AudioClip audioClip = mapManager.GetCurrentMapSection().sectionAudio;
                            if (audioClip != null)
                            {
                                if (musicInstance.Audio == null || audioClip.name != musicInstance.Audio.name)
                                    PlayMusic(audioClip);
                            }
                        }
                        break;
                    }
                case GameState.Conversation:
                    {
                        // need to check the map managers music (conversation also?) 
                        break;
                    }
            }

            currentGameState = gameState;
        }       

        private void OnApplicationQuit()
        {
            Initialiser.OnGameStateChanged -= OnGameStateChanged;

            StartCoroutine(musicInstance.CleanUp());

            instancePool.Dispose();
            instancePool = null;
        }

        public static void PlayMusic(AudioClip musicTrack)
        {
            Instance.musicInstance.Load(musicTrack, AudioType.MUSIC);
        }

        public static void Play(AudioClip audio, Vector3 position, AudioType audioType)
        {
            AudioInstance instance = Instance.instancePool.Get();
            instance.transform.position = position;

            instance.Load(audio, audioType);
        }

        private AudioInstance CreateAudioInstance()
        {
            AudioInstance audioInstance = Instantiate(audioInstancePrefab, instanceParent);
            audioInstance.gameObject.SetActive(false);

            return audioInstance;
        }
        private void OnAudioInstanceRetrieved(AudioInstance instance)
        {
            instance.onInstanceFinished += OnAudioInstanceFinished;
            instance.gameObject.SetActive(true);
        }

        private void OnAudioInstanceReleased(AudioInstance instance)
        {
            instance.onInstanceFinished -= OnAudioInstanceFinished;
            instance.gameObject.SetActive(false);
            StartCoroutine(instance.CleanUp());
        }

        private void OnAudioInstanceDestroyed(AudioInstance instance)
        {
            instance.onInstanceFinished -= OnAudioInstanceFinished;
            StartCoroutine(instance.CleanUp(() =>
            {
                Destroy(instance.gameObject);

            }));
        }

        private void OnAudioInstanceFinished(AudioInstance audioInstance)
        {
            instancePool.Release(audioInstance);
        }
    }
}
