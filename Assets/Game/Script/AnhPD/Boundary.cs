using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhPD
{
    public class Boundary : MonoBehaviour
    {
        [SerializeField] private Basket[] target;

        private int index = -1;
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == CONSTANTS.TAG_BIRD)
            {
                Bird bird = other.GetComponent<Bird>();
                if (!LevelManager.Ins.AddBirdToColorBaskets (bird))
                {
                    Transform pos = GetTargetPosition();
                    if (Vector3.Distance(transform.position, pos.position) < .5f)
                    {
                        bird.OnBirdAwake();
                        LevelManager.Ins.OnLose();
                    }
                    else
                    {
                        bird.OnStartFlying(pos.position);
                        bird.ChangeLandState(LAND_STATE.FREE_BASKET);
                    }
                }
            }
        }
        private Transform GetTargetPosition()
        {
            
            if(index < target.Length - 1)
            {
                index++;
                Transform pos = target[index].transform;
                if (target[index].IsHaveBirdIn)
                {
                    return GetTargetPosition();
                }
                return pos;
            }
            return transform;
        }
    }
}

