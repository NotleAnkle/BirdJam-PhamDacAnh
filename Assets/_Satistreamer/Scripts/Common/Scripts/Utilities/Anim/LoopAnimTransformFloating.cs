using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class LoopAnimTransformFloating : MonoBehaviour
    {
        public float cycleDuration = 1f;
        public float positionCurveMul = 1f;
        public AnimationCurve positionCurveX = AnimationCurve.Constant(0, 1, 0f);
        public AnimationCurve positionCurveY = AnimationCurve.EaseInOut(0, 0f, 1, 1f);
        public bool isUseScaleTime = false;
        public bool isRandomTimeOffset = false;
        public Vector2 timeOffset = Vector2.zero;

        private Transform _transform;
        private Vector3 _startPos;

        private void OnEnable()
        {
            _transform = transform;
            _startPos = _transform.localPosition;
            if (isRandomTimeOffset)
            {
                timeOffset = new Vector2(
                    UnityEngine.Random.Range(0f, cycleDuration),
                    UnityEngine.Random.Range(0f, cycleDuration)
                    );
            }
        }

        // Update is called once per frame
        void Update()
        {
            _transform.localPosition = new Vector3(
                _startPos.x + positionCurveMul * positionCurveX.Evaluate(Mathf.Repeat(((isUseScaleTime ? Time.time : Time.unscaledTime) + timeOffset.x) / cycleDuration, 1f)),
                _startPos.y + positionCurveMul * positionCurveY.Evaluate(Mathf.Repeat(((isUseScaleTime ? Time.time : Time.unscaledTime) + timeOffset.y) / cycleDuration, 1f)),
                _startPos.z
                );
        }

        private void OnDisable()
        {
            _transform.localPosition = _startPos;
        }
    }
}