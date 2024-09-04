using System.Collections;
using UnityEngine;
using MoreMountains.NiceVibrations;

namespace Utilities
{
    public class LoopVibrateHaptic : MonoBehaviour
    {
        [Range(0f,1f)] public float intensity = 0.5f;
        [Range(0f, 1f)] public float sharpness = 0.5f;
        public float duration;
        private Coroutine vibrationCoroutine;

        private void OnEnable()
        {
            if (vibrationCoroutine != null)
            {
                StopCoroutine(vibrationCoroutine);
            }
            vibrationCoroutine = StartCoroutine(IEVibrate());
        }

        private IEnumerator IEVibrate()
        {
            while (true)
            {
                //MMVibrationManager.ContinuousHaptic(intensity, sharpness, duration);
                yield return new WaitForSeconds(duration);
            }
        }

        private void OnDisable()
        {
            if (vibrationCoroutine != null)
            {
                StopCoroutine(vibrationCoroutine);
            }
            //MMVibrationManager.StopContinuousHaptic();
        }
    }
}