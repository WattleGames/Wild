using Wattle.Utils;
using System.Collections;
using UnityEngine;

namespace Wattle.Wild.Infrastructure
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        [SerializeField] public AudioClip uiButtonSound;
        [SerializeField] public AudioClip mainMenuMusic;

        public override IEnumerator Initalise()
        {
            initialised = true;
            yield return base.Initalise();
        }
    }
}