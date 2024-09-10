using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities
{
    public class DelayUnityEvent : MonoBehaviour
    {
        public Vector2 delay = Vector2.zero;
        public UnityEvent triggerEvent;

        private void Start()
        {
            this.WaitToDo(triggerEvent.Invoke, UnityEngine.Random.Range(delay.x, delay.y));
        }
    }
}