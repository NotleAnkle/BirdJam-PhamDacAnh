using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class LoopAnimSpriteRendererFade : MonoBehaviour
    {
        public float cycleDuration = 1f;
        public AnimationCurve alphaCurve = default;
        public float timeOffset = 0f;

        private SpriteRenderer _renderer;
        private Color _color;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _color = _renderer.color;
            if (timeOffset < 0f)
            {
                timeOffset = Random.Range(0f, cycleDuration);
            }
        }

        // Update is called once per frame
        void Update()
        {
            _color.a = alphaCurve.Evaluate(Mathf.Repeat((Time.time + timeOffset) / cycleDuration, 1f));
            _renderer.color = _color;
        }
    }
}