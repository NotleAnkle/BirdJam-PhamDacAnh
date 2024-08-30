using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhPD
{
    public class LevelManager : Singleton<LevelManager>
    {
        [SerializeField] private ColorBaskets currentColorBaskets, nextColorBaskets;
        [SerializeField] private SOLevelData data;

        private ColorBaskets lastColorBaskets;

        private int index = 2;

        private void Start()
        {
            InitColorBasket(data.colorBaskets[0], currentColorBaskets);
            InitColorBasket(data.colorBaskets[1], nextColorBaskets);
        }

        private void InitColorBasket(ColorBasketData cbData, ColorBaskets baskets)
        {
            baskets.OnInit(cbData.numberOfBasket, cbData.color);
        }


        public bool AddBirdToColorBaskets(Bird bird)
        {
            if(bird.color == currentColorBaskets.color && currentColorBaskets.IsAnyBasketLeft)
            {
                currentColorBaskets.AddBird(bird);
                bird.ChangeLandState(LAND_STATE.COLOR_BASKET);

                return true;
            }
            return false;
        }
        public void CheckColorBaskets(Bird bird)
        {
            currentColorBaskets.CheckComplete(bird);
        }
        public void OnCompleteColorBaskets()
        {
            lastColorBaskets = currentColorBaskets;
            lastColorBaskets.transform.DOMoveX(5f, .5f).OnComplete(() =>
            {
                SimplePool.Release(lastColorBaskets);
            });

            currentColorBaskets = nextColorBaskets;
            currentColorBaskets.isMoving = true;
            currentColorBaskets.transform.DOMoveX(0f, .5f).OnComplete(() =>
            {
                currentColorBaskets.isMoving = false;
            });

            nextColorBaskets = SimplePool.Spawn<ColorBaskets>(PoolType.ColorBaskets);
            nextColorBaskets.transform.localPosition = new Vector3(-5f, 0, 0);
            nextColorBaskets.transform.DOMoveX(-3.5f, 1f);
        }

        public void OnLose()
        {
            Debug.Log("Lose");
        }
    }
}

