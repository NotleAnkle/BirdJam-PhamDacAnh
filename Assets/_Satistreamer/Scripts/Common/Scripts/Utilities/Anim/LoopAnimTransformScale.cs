using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class LoopAnimTransformScale : MonoBehaviour
    {
        public float cycleDuration = 1f;
        public AnimationCurve scaleCurve = default;
        public bool isUseScaleTime = false;
        public float timeOffset = 0f;

        private Transform _transform;
        private Vector3 _originScale;

        private void Awake()
        {
            _transform = transform;
        }

        private void OnEnable()
        {
            _originScale = transform.localScale;
        }

        // Update is called once per frame
        void Update()
        {
            _transform.localScale = scaleCurve.Evaluate(Mathf.Repeat(((isUseScaleTime ? Time.time : Time.unscaledTime) + timeOffset) / cycleDuration, 1f)) * Vector3.one;
        }

        private void OnDisable()
        {
            transform.localScale = _originScale;
        }
    }
}