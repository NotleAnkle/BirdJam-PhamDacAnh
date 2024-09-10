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

        private int index = 2, completeCount = 0;

        public bool isAllowOnMouseDown = true;

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
            if(bird.color == currentColorBaskets.color && currentColorBaskets.IsAnyBasketLeft() && !currentColorBaskets.isMoving)
            {
                currentColorBaskets.AddBird(bird);
                bird.ChangeLandState(LAND_STATE.COLOR_BASKET);

                return true;
            }
            return false;
        }
        public void CheckColorBaskets()
        {
            currentColorBaskets.CheckComplete();
        }
        public void OnCompleteColorBaskets()
        {
            lastColorBaskets = currentColorBaskets;
            lastColorBaskets.transform.DOMoveX(5f, .5f).OnComplete(() =>
            {
                lastColorBaskets.OnDespawn();
            });

            completeCount++;
            if (completeCount == data.colorBaskets.Count)
            {
                OnCompleteLevel();
            }

            currentColorBaskets = nextColorBaskets;
            currentColorBaskets.isMoving = true;
            currentColorBaskets?.transform.DOMoveX(0f, .5f).OnComplete(() =>
            {
                currentColorBaskets.isMoving = false;
            });


            if (index < data.colorBaskets.Count)
            {
                nextColorBaskets = SimplePool.Spawn<ColorBaskets>(PoolType.ColorBaskets);
                nextColorBaskets.OnInit(data.colorBaskets[index].numberOfBasket, data.colorBaskets[index].color);
                nextColorBaskets.transform.localPosition = new Vector3(-5f, 0, 0);
                nextColorBaskets.transform.DOMoveX(-3.5f, 1f);

                index++;
            }
            else
            {
                nextColorBaskets = null;
            }
        }
        private void OnCompleteLevel()
        {
            Debug.Log("Complete");
        }
        public void OnLose()
        {
            Debug.Log("Lose");
        }
    }
}

