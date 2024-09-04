using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class MonoBehaviourExtension
    {
        public static Coroutine WaitOneFrame(this MonoBehaviour monoBehaviour, System.Action action)
        {
            return monoBehaviour.StartCoroutine(DelayOneFrame(action));
        }
        private static IEnumerator DelayOneFrame(System.Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }

        public static Coroutine WaitFrames(this MonoBehaviour monoBehaviour, System.Action action, int numFrames)
        {
            return monoBehaviour.StartCoroutine(DelayFrames(action, numFrames));
        }
        private static IEnumerator DelayFrames(System.Action action, int numFrames)
        {
            for (int i = 0; i < numFrames; i++)
            {
                yield return null;
            }
            action?.Invoke();
        }

        public static Coroutine WaitToDo(this MonoBehaviour monoBehaviour, System.Action action, float waitDuration)
        {
            if (waitDuration <= 0)
            {
                action?.Invoke();
                return null;
            }
            else
            {
                return monoBehaviour.StartCoroutine(DelayToDo(action, waitDuration));
            }
        }

        private static IEnumerator DelayToDo(System.Action action, float waitDuration)
        {
            yield return new WaitForSeconds(waitDuration);
            action?.Invoke();
        }

        public static Coroutine WaitToDoUntil(this MonoBehaviour monoBehaviour, System.Action action, System.Func<bool> predicate)
        {
            if (predicate())
            {
                action?.Invoke();
                return null;
            }
            else
            {
                return monoBehaviour.StartCoroutine(DelayToDoUntil(action, predicate));
            }
        }

        private static IEnumerator DelayToDoUntil(System.Action action, System.Func<bool> predicate)
        {
            yield return new WaitUntil(predicate);
            action?.Invoke();
        }
    }
}