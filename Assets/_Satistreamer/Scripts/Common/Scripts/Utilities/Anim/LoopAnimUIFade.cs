using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    [RequireComponent(typeof(Graphic))]
    public class LoopAnimUIFade : MonoBehaviour
    {
        public float cycleDuration = 1f;
        public AnimationCurve alphaCurve = default;
        public bool isUseUnscaledTime = false;
        private Graphic _graphic;
        private Color _color;

        private void Awake()
        {
            _graphic = GetComponent<Graphic>();
            _color = _graphic.color;
        }

        // Update is called once per frame
        void Update()
        {
            _color.a = alphaCurve.Evaluate(Mathf.Repeat((isUseUnscaledTime ? Time.unscaledTime : Time.time) / cycleDuration, 1f));
            _graphic.color = _color;
        }
    }
}