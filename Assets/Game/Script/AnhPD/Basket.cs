using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhPD
{
    public class Basket : GameUnit
    {
        private Bird currentBird;
        public bool IsHaveBirdIn => currentBird != null;

        public void SetBird(Bird bird)
        {
            currentBird = bird;
            bird.transform.SetParent(transform);
        }
    }

}
