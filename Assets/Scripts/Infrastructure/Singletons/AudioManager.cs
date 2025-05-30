using Wattle.Wild.Audio;
using Wattle.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using AudioType = Wattle.Wild.Audio.AudioType;

namespace Wattle.Wild.Infrastructure
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private Transform instanceParent;
        [SerializeField] private AudioInstance audioInstancePrefab;

        private ObjectPool<AudioInstance> instancePool;

        private int defaultCapacity = 5;
        private int maxCapacity = 100;

        public override IEnumerator Initalise()
        {
            instancePool = new ObjectPool<AudioInstance>(
                createFunc: () => { return CreateAudioInstance(); },
                OnAudioInstanceRetrieved,
                OnAudioInstanceReleased,
                OnAudioInstanceDestroyed,
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxCapacity
                );

            initialised = true;

            yield return base.Initalise();
        }

        private void OnDestroy()
        {
            instancePool.Dispose();
            instancePool = null;
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
            instance.CleanUp();
        }

        private void OnAudioInstanceDestroyed(AudioInstance instance)
        {
            instance.onInstanceFinished -= OnAudioInstanceFinished;
            instance.CleanUp();

            Destroy(instance.gameObject);
        }

        private void OnAudioInstanceFinished(AudioInstance audioInstance)
        {
            instancePool.Release(audioInstance);
        }
    }
}
