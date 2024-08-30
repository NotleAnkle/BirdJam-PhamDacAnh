using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhPD 
{
    public class GameUnit : MonoBehaviour
    {
        private Transform _tf;
        public Transform TF
        {
            get
            {
                if (_tf == null)
                {
                    _tf = transform;
                }
                return _tf;
            }
        }

        public PoolType poolType;

        // TODO: Add register and remove events to OnInit and OnDestroy methods
    }
}
