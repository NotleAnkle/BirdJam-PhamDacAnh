using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class LoopSound : MonoBehaviour
    {
        public AudioClip clip;
        public float interval = 1f;
        public AudioSource audioSource;
        private Coroutine soundCoroutine;

        private void OnEnable()
        {
            if (soundCoroutine != null) StopCoroutine(soundCoroutine);
            soundCoroutine = StartCoroutine(PlaySoundWithInterval());
        }

        private void OnDisable()
        {
            if (soundCoroutine != null) StopCoroutine(soundCoroutine);
        }

        IEnumerator PlaySoundWithInterval()
        {
            while (true)
            {
                audioSource.PlayOneShot(clip);
                yield return new WaitForSeconds(interval + clip.length);
            }
        }
    }
}