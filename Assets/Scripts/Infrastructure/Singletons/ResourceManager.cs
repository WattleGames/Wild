using Wattle.Utils;
using System.Collections;
using UnityEngine;

namespace Wattle.Wild.Infrastructure
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        [SerializeField] public AudioClip uiButtonSound;

        public override IEnumerator Initalise()
        {
            initialised = true;
            yield return base.Initalise();
        }
    }
}