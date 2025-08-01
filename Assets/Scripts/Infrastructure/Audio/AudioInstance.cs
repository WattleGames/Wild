using Wattle.Wild.Infrastructure;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

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

        public AudioClip Audio => audioClip;
        public bool IsPlaying => audioSource.isPlaying;

        [SerializeField] private AudioSource audioSource;

        private Coroutine audioCoroutine = null;
        private AudioClip audioClip = null;
        private AudioType instanceType;

        private Action onCompleteCallback = null;

        public void Load(AudioClip audioClip, AudioType instanceType, Action onCompleteCallback)
        {
            StartCoroutine(CleanUp(() =>
            {
                if (audioSource != null)
                {
                    this.onCompleteCallback = onCompleteCallback;
                    this.instanceType = instanceType;
                    this.audioClip = audioClip;

                    if (instanceType == AudioType.MUSIC)
                    {
                        audioSource.loop = true;
                        audioSource.spatialize = false;
                    }

                    SubscribeToSettingEvents();

                    Play();
                }
            }));
        }

        public IEnumerator CleanUp(Action onComplete = null)
        {
            if (audioClip == null)
            {
                onComplete?.Invoke();
            }
            else
            {
                if (instanceType == AudioType.MUSIC)
                {
                    yield return FadeMusic(false);
                }
                else if (instanceType == AudioType.VOICE)
                {
                    audioSource.Stop();
                }

                if (audioCoroutine != null)
                    StopCoroutine(audioCoroutine);

                audioCoroutine = null;
                audioClip = null;

                UnsubscribeToSettingEvents();

                onComplete?.Invoke();
            }

            yield return null;
        }

        private void Play()
        {
            audioSource.clip = audioClip;
            audioCoroutine = StartCoroutine(Play_Internal());
        }

        private IEnumerator Play_Internal()
        {
            audioSource.volume = 0;

            if (instanceType == AudioType.MUSIC)
                StartCoroutine(FadeMusic(true));
            else
                audioSource.volume = EvaluateVolume();

            audioSource.Play();
            yield return new WaitUntil(() => !audioSource.isPlaying);

            onCompleteCallback?.Invoke();
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
                case AudioType.VOICE:
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
                case AudioType.VOICE:
                    paramMod = SaveSystem.Instance.AudioSettings.dialogueVolume.Value;
                    break;
            }

            float volume = masterVolume * paramMod;
            return volume;
        }

        private IEnumerator FadeMusic(bool fadeIn)
        {
            Tweener tween = audioSource.DOFade(fadeIn ? SaveSystem.Instance.AudioSettings.musicVolume.Value : 0, 1).SetAutoKill();
            yield return new WaitUntil(() => !tween.active);
        }
    }
}
