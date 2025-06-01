using System;
using UnityEngine;
using Wattle.Wild.Infrastructure;

namespace Wattle.Wild.UI
{
    public class CanvasCameraFinder : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

        private void OnEnable()
        {
            SetCamera();

            SingletonManager.NotifyOnInitialised(() =>
            {
                SceneManager.Instance.OnSceneChanged += OnSceneChanged;
            });
        }

        private void OnDisable()
        {
            SceneManager.Instance.OnSceneChanged -= OnSceneChanged;
        }

        private void OnSceneChanged(UnityEngine.SceneManagement.Scene scene)
        {
            SetCamera();
        }

        private void SetCamera()
        {
            canvas.worldCamera = Camera.main;
        }
    }
}

