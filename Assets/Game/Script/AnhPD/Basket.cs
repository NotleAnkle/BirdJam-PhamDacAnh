using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhPD
{
    public class Basket : GameUnit
    {
        private Bird currentBird;
        public bool IsHaveBirdIn = false;
        public bool IsCompleteFlying = true;

        public void SetBird(Bird bird)
        {
            currentBird = bird;
            IsHaveBirdIn = true;
            IsCompleteFlying = false;
        }
        public void RemoveBird()
        {
            currentBird = null;
            IsHaveBirdIn = false;
        }
        public void OnBirdCompleteFlying()
        {
            IsCompleteFlying = true;
        }
        public void OnDespawn()
        {
            currentBird?.OnDespawn();

            SimplePool.Despawn(this);
        }
    }

}
