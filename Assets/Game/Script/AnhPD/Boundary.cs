using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhPD
{
    public class Boundary : MonoBehaviour
    {
        [SerializeField] private Basket[] baskets;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == CONSTANTS.TAG_BIRD)
            {
                Bird bird = other.GetComponent<Bird>();
                if (!LevelManager.Ins.AddBirdToColorBaskets (bird))
                {
                    Basket basket = GetTargetPosition();
                    if (basket == null)
                    {
                        bird.OnBirdAwake();
                        LevelManager.Ins.OnLose();
                    }
                    else
                    {
                        bird.OnStartFlying(basket);
                        bird.ChangeLandState(LAND_STATE.FREE_BASKET);
                    }
                }
            }
        }
        private Basket GetTargetPosition()
        {
            for(int i = 0; i < baskets.Length; i++)
            {
                if (!baskets[i].IsHaveBirdIn)
                {
                    return baskets[i];
                }
            }
            return null;
        }
    }
}

