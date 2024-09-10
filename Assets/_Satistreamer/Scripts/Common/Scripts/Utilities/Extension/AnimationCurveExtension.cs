using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class AnimationCurveExtension
    {
        public static int EvaluateInt(this AnimationCurve animationCurve, float time)
            => Mathf.RoundToInt(animationCurve.Evaluate(time));
    }
}