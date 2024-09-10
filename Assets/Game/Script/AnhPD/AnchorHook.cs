using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhPD
{
    public class AnchorHook : MonoBehaviour
    {
        [SerializeField] private List<Bird> birds;
        [SerializeField] private HookType hookType;
        [SerializeField] private GameObject line;
        private void Start()
        {
            UpdateHeadOfLine();
        }
        public void AddBird(Bird bird)
        {
            birds.Add(bird);
            bird.SetHook(this);
        }
        public void OnRemoveBird(Bird bird)
        {
            birds.Remove(bird);
            bird.SetHook(null);
            if(birds.Count < 1)
            {
                line.SetActive(false);
            }
            else
            {
                UpdateHeadOfLine();
            }
        }
        private void UpdateHeadOfLine()
        {
            birds[birds.Count - 1].EnableOutline();

        }
        public void OnCollide()
        {
            switch(hookType)
            {
                case HookType.Normal:
                    break;
                case HookType.DropAble:
                    birds[birds.Count - 1].OnFall();
                    break;
            }
        }
        public enum HookType
        {
            Normal = 0,
            DropAble = 1,
        }
    }
}

