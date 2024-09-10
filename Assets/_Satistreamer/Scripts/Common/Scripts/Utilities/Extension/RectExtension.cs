using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class RectExtension
    {
        public static float ClampX(this Rect rect, float value) => Mathf.Clamp(value, rect.xMin, rect.xMax);
        public static float ClampY(this Rect rect, float value) => Mathf.Clamp(value, rect.yMin, rect.yMax);

        public static Vector2 Clamp(this Rect rect, Vector2 value)
            => new Vector3(
                Mathf.Clamp(value.x, rect.xMin, rect.xMax),
                Mathf.Clamp(value.y, rect.yMin, rect.yMax)
                );

        public static Vector2 ClampOutside(this Rect rect, Vector2 value)
            => new Vector3(
                (value.x < rect.center.x) ? Mathf.Min(value.x, rect.xMin) : Mathf.Max(value.x, rect.xMax),
                (value.y < rect.center.y) ? Mathf.Min(value.y, rect.yMin) : Mathf.Max(value.y, rect.yMax)
                );

        public static Vector3 Clamp(this Rect rect, Vector3 value)
            => new Vector3(
                Mathf.Clamp(value.x, rect.xMin, rect.xMax),
                Mathf.Clamp(value.y, rect.yMin, rect.yMax),
                value.z
                );

        public static bool IsContain(this Rect rect, Vector2 position)
            => rect.xMin < position.x && position.x < rect.xMax && rect.yMin < position.y && position.y < rect.yMax;
        public static bool IsNotContain(this Rect rect, Vector2 position)
            => rect.xMin > position.x || position.x > rect.xMax || rect.yMin > position.y || position.y > rect.yMax;
        
        public static Vector2 GetRandomInside(this Rect rect) => new Vector2(
            UnityEngine.Random.Range(rect.xMin, rect.xMax),
            UnityEngine.Random.Range(rect.yMin, rect.yMax)
            );
        
        public static void DrawGizmo(this Rect rect, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(rect.center, new Vector3(rect.size.x, rect.size.y, 0.1f));
        }
        
        public static Vector2 Repeat(this Rect rect, Vector2 value)
            => new Vector2(
                Mathf.Repeat(value.x - rect.xMin, rect.width) + rect.xMin,
                Mathf.Repeat(value.y - rect.yMin, rect.height) + rect.yMin
                );
    }
}