using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class LoopAnimVibratePosition : MonoBehaviour
    {
        public float amplitude = 0.1f;
        public float frequency = 1f;

        private Vector3 initialPosition;
        private float randomOffsetX;
        private float randomOffsetY;
        private Transform cachedTransform;

        private void OnEnable()
        {
            cachedTransform = transform;
            initialPosition = cachedTransform.localPosition;
            randomOffsetX = Random.Range(0, 2 * Mathf.PI); // Random offset for x-axis
            randomOffsetY = Random.Range(0, 2 * Mathf.PI); // Random offset for ySpeed-axis
        }

        void Update()
        {
            // Calculate new position based on sine and cosine waves with random offsets
            float theta = Time.time * frequency;
            float distanceX = amplitude * Mathf.Cos(theta + randomOffsetX);
            float distanceY = amplitude * Mathf.Sin(theta + randomOffsetY);

            cachedTransform.localPosition = initialPosition + new Vector3(distanceX, distanceY, 0);
        }

        private void OnDisable()
        {
            cachedTransform.localPosition = initialPosition;
        }
    }
}