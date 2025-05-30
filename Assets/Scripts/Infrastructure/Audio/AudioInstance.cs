using Wattle.Wild.Infrastructure;
using System;
using System.Collections;
using UnityEngine;

namespace Wattle.Wild.Audio
{
    public enum AudioType
    {
        SFX,
        MUSIC,
        VOICE
    }

    public class AudioInstance : MonoBehaviour
    {
        public Action<AudioInstance> onInstanceFinished;

        [SerializeField] private AudioSource audioSource;

        private Coroutine audioCoroutine = null;
        private AudioClip audioClip = null;
        private AudioType instanceType;

        public void Load(AudioClip audioClip, AudioType instanceType)
        {
            this.instanceType = instanceType;
            this.audioClip = audioClip;

            SubscribeToSettingEvents();

            Play();
        }

        public void CleanUp()
        {
            if (audioCoroutine != null)
                StopCoroutine(audioCoroutine);

            audioCoroutine = null;
            audioClip = null;

            UnsubscribeToSettingEvents();
        }

        private void Play()
        {
            audioSource.clip = audioClip;
            audioCoroutine = StartCoroutine(Play_Internal());
        }

        private IEnumerator Play_Internal()
        {
            audioSource.Play();
            yield return new WaitUntil(() => !audioSource.isPlaying);
            onInstanceFinished?.Invoke(this);

            audioCoroutine = null;
        }

        private void SubscribeToSettingEvents()
        {
            SaveSystem.Instance.AudioSettings.masterVolume.onValueChanged += OnAudioSettingChanged;

            switch (instanceType)
            {
                case AudioType.SFX:
                    SaveSystem.Instance.AudioSettings.sfxVolume.onValueChanged += OnAudioSettingChanged;
                    break;
                case AudioType.MUSIC:
                    SaveSystem.Instance.AudioSettings.musicVolume.onValueChanged += OnAudioSettingChanged;
                    break;
            }
        }

        private void UnsubscribeToSettingEvents()
        {
            SaveSystem.Instance.AudioSettings.masterVolume.onValueChanged -= OnAudioSettingChanged;
            SaveSystem.Instance.AudioSettings.sfxVolume.onValueChanged -= OnAudioSettingChanged;
            SaveSystem.Instance.AudioSettings.musicVolume.onValueChanged -= OnAudioSettingChanged;
        }

        private void OnAudioSettingChanged(float value)
        {
            audioSource.volume = EvaluateVolume();
        }

        private float EvaluateVolume()
        {
            float masterVolume = SaveSystem.Instance.AudioSettings.masterVolume.Value;
            float paramMod = 1;

            switch (instanceType)
            {
                case AudioType.SFX:
                    paramMod = SaveSystem.Instance.AudioSettings.sfxVolume.Value;
                    break;
                case AudioType.MUSIC:
                    paramMod = SaveSystem.Instance.AudioSettings.musicVolume.Value;
                    break;
            }

            float volume = masterVolume * paramMod;
            return volume;
        }
    }
}
