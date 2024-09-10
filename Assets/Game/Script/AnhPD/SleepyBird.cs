using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhPD
{
    public class SleepyBird : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == CONSTANTS.TAG_BIRD)
            {
                Bird bird = collision.gameObject.GetComponent<Bird>();
                if (bird.IsState(LAND_STATE.FALLING))
                {
                    bird.Freeze(null);
                }
            }
        }
    }
}

