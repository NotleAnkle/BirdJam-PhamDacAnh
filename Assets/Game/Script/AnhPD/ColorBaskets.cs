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

        int index;

        public void OnInit(int num, COLOR color)
        {
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
        public bool IsAnyBasketLeft()
        {
            for (int i = 0; i < baskets.Count; i++)
            {
                if (!baskets[i].IsHaveBirdIn)
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }
        public void AddBird(Bird bird)
        {
            bird.OnStartFlying(baskets[index]);
        }
        public void CheckComplete()
        {
            for(int i = 0; i < baskets.Count; i++)
            {
                if (!baskets[i].IsHaveBirdIn || !baskets[i].IsCompleteFlying)
                {
                    return;
                }
            }
            OnComplete();
        }
        private void OnComplete()
        {
            LevelManager.Ins.OnCompleteColorBaskets();
        }
        public void OnDespawn()
        {
            for(int i = 0; i < baskets.Count;i++)
            {
                baskets[i].OnDespawn();
            }
            SimplePool.Despawn(this);
        }
    }
}

