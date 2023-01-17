using UnityEngine.Audio;
using UnityEngine;

namespace Architecture.Audio
{
    [System.Serializable]
    public struct Sound
    {
        public string name;

        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume;
    }
}