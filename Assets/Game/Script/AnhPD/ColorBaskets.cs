using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhPD
{
    public class ColorBaskets : GameUnit
    {
        [SerializeField] private Material[] materials;
        [SerializeField] private MeshRenderer meshRenderer;
        public List<Basket> baskets;
        public COLOR color;
        public bool isMoving = false;

        private int index = 0, numberOfBird = 0;

        public void OnInit(int num, COLOR color)
        {
            index = 0;
            numberOfBird = 0;
            for(int i = 0; i < baskets.Count; i++)
            {
                SimplePool.Release(baskets[i]);
            }
            baskets.Clear();
            float xMin;
            float xSpacing = 1f;
            xMin = (-num / 2 + (num%2 == 0 ? 0.5f : 0f)) * xSpacing;
            for (int i = 0; i < num; i++)
            {
                Basket basket = SimplePool.Spawn<Basket>(PoolType.Basket, transform);
                basket.transform.localPosition = new Vector2(i * 1f + xMin, 0);
                baskets.Add(basket);
            }
            meshRenderer.material = materials[(int)color];
            this.color = color;
        }
        public bool IsAnyBasketLeft => index < baskets.Count;
        public void AddBird(Bird bird)
        {
            if (isMoving) return;
            bird.OnStartFlying(baskets[index].transform.position);
            index++;
        }
        public void CheckComplete(Bird bird)
        {
            baskets[numberOfBird].SetBird(bird);

            numberOfBird++;
            if (numberOfBird == baskets.Count)
            {
                OnComplete();
            }
        }
        private void OnComplete()
        {
            LevelManager.Ins.OnCompleteColorBaskets();
        }

    }
}

