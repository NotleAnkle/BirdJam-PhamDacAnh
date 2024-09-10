using AudioEnum;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace AnhPD
{
    public class Bird : GameUnit
    {
        [Header("Reference")]
        [SerializeField] private SkinnedMeshRenderer[] skins;
        [SerializeField] private Material[] materials;
        [SerializeField] private Material outlineMaterial;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject eyelidL, eyelidR;
        [SerializeField] private SphereCollider collide;
        [SerializeField] private HingeJoint joint;
        [SerializeField] private AnchorHook currentHook;
        public Rigidbody rb;
        [SerializeField] private Bird previousSleepyBird;
        [SerializeField] private List<Bird> followingSleepyBirds = new List<Bird>();

        [Header("Attribute")]
        public COLOR color;
        [SerializeField] private List<SfxType> hitSfxs;
        [SerializeField] private LAND_STATE state = LAND_STATE.NONE;
        [SerializeField] private bool isCanTouch = false;

        private Basket currentBasket;

        private void OnEnable()
        {
            if (isCanTouch)
            {
                EnableOutline();
            }
        }

        public void OnInit(COLOR skinColor)
        {
            color = skinColor;
            ChangeSkinClor(skinColor);
            collide.enabled = true;
            state = LAND_STATE.NONE;

            previousSleepyBird = null;
            followingSleepyBirds = new List<Bird>();
        }
        public void InitColor(COLOR skinColor)
        {
            color = skinColor;
            ChangeSkinClor(skinColor);
        }
        private void OnMouseDown()
        {
            if (!isCanTouch || !LevelManager.Ins.isAllowOnMouseDown) return;
            switch (state)
            {
                case LAND_STATE.COLOR_BASKET:
                    return;
                case LAND_STATE.FREE_BASKET:
                    LevelManager.Ins.AddBirdToColorBaskets(this);
                    break;
                case LAND_STATE.NONE:
                    OnFall();
                    break;
                case LAND_STATE.FREEZE:
                    UnFreeze();
                    break;
            }
        }

        public void OnFall()
        {
            DisableOutline();
            currentHook?.OnRemoveBird(this);
            currentHook = null;
            Destroy(joint);
            state = LAND_STATE.FALLING;
        }

        Sequence sequence;
        public void OnStartFlying(Basket basket)
        {
            OnBirdAwake();

            float flyduration = 0.75f;

            if (state == LAND_STATE.FREE_BASKET)
            {
                //sequence.Complete();
                sequence.Append(transform.DOMoveY(transform.position.y + .75f, .5f));
                ChangeLandState(LAND_STATE.COLOR_BASKET);
                flyduration = .5f;
            }

            SetCurrentBasket(basket);

            sequence.Append(transform.DOMove(basket.TF.position + Vector3.up * .5f, flyduration).SetEase(Ease.InBack));
            sequence.Append(transform.DOMove(basket.TF.position, .25f));
            sequence.OnComplete(() =>
            {
                ChangeAnim(CONSTANTS.ANIM_IDLE);
                currentBasket.OnBirdCompleteFlying();

                collide.enabled = (state != LAND_STATE.COLOR_BASKET);

                if (state == LAND_STATE.COLOR_BASKET )
                {
                    LevelManager.Ins.CheckColorBaskets();
                }
            });
        }
        public void OnBirdAwake()
        {
            ChangeAnim(CONSTANTS.ANIM_FLY);
            ChangeEyeState(EyeState.BOTH_OPEN);

            collide.enabled = false;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            sequence = DOTween.Sequence();

            sequence.Append(transform.DORotate(Vector3.zero, .5f));
        }
        public void ChangeLandState(LAND_STATE state)
        {
            this.state = state;
        }

        private void OnCollisionEnter(Collision collision)
        {
            eyeState = (EyeState) Random.Range(0,3);
            ChangeEyeState(eyeState);

            skins[1].material = materials[(int)COLOR.DARKPURPLE];
            skins[2].material = materials[(int)COLOR.DARKPURPLE];

            AudioManager.Ins.PlayRandomSfx(hitSfxs);

            if(collision.gameObject.tag == CONSTANTS.TAG_BIRD)
            {
                switch (state)
                {
                    case LAND_STATE.NONE:
                        currentHook?.OnCollide();
                        break;
                    case LAND_STATE.FREEZE:
                        Bird bird = collision.gameObject.GetComponent<Bird>();
                        if(bird.state == LAND_STATE.FALLING)
                        {
                            bird.Freeze(this);
                            
                            AddFollowingSleepyBird(bird);
                        }
                        break;
                }
            }
        }
        //hook
        public void SetHook(AnchorHook hook)
        {
            currentHook = hook;
            transform.SetParent(hook?.transform);
        }
        public void SetConnetedBody(Rigidbody rb)
        {
            if(joint != null) return;
            joint = gameObject.AddComponent<HingeJoint>();
            joint.connectedBody = rb;
        }
        //---
        public bool IsState(LAND_STATE state)
        {
            return state == this.state;
        }
        public void SetCurrentBasket(Basket basket)
        {
            currentBasket?.RemoveBird();
            currentBasket = basket;
            currentBasket.SetBird(this);
            TF.SetParent(currentBasket.TF);
        }
        public void OnDespawn()
        {
            SimplePool.Despawn(this);
        }

    #region sleepyBird
        public void UnFreeze()
        {
            ChangeLandState(LAND_STATE.UNFREEZE);

            rb.useGravity = true;

            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

            previousSleepyBird?.OnFollowingSleepyBirdUnFreezy(this);
            DisableOutline();

            StartCoroutine(DelayFalling());
        }
        private IEnumerator DelayFalling()
        {
            yield return new WaitForSeconds(.3f);
            if(state == LAND_STATE.UNFREEZE)
            {
                ChangeLandState(LAND_STATE.FALLING);
            }
        }
        public void Freeze(Bird bird)
        {
            previousSleepyBird = bird;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            rb.velocity = Vector3.zero;
            rb.useGravity = false;

            state = LAND_STATE.FREEZE;
            EnableOutline();
        }
        public void OnFollowingSleepyBirdUnFreezy(Bird bird)
        {
            followingSleepyBirds.Remove(bird);
            if(followingSleepyBirds.Count < 1)
            {
                EnableOutline();
            }
        }
        public void AddFollowingSleepyBird(Bird bird)
        {
            isCanTouch = false;
            DisableOutline();
            followingSleepyBirds.Add(bird);
        }
    #endregion
    
    #region outline
        public void EnableOutline()
        {
            isCanTouch = true;
            if (skins[3].material != outlineMaterial)
            {
                skins[3].material = outlineMaterial;
            }

            skins[3].material.SetFloat("_Scale", 1.1f);
        }
        public void DisableOutline()
        {
            skins[3].material.SetFloat("_Scale", 0);
        }
    #endregion

    #region skin
        private void ChangeSkinClor(COLOR skinColor)
        {
            for (int i = 0; i < 3; i++)
            {
                skins[i].material = materials[(int)skinColor];
            }
        }
        enum EyeState
        {
            BOTH_CLOSE = 0,
            LEFT_OPEN = 1,
            RIGHT_OPEN = 2,
            BOTH_OPEN = 3,
        }
        private EyeState eyeState = EyeState.BOTH_CLOSE;
        private void ChangeEyeState(EyeState state)
        {
            eyeState = state;
            switch (eyeState)
            {
                case EyeState.BOTH_CLOSE:
                    eyelidL.SetActive(true);
                    eyelidR.SetActive(true);
                    break;
                case EyeState.LEFT_OPEN:
                    eyelidL.SetActive(false);
                    eyelidR.SetActive(true);
                    break;
                case EyeState.RIGHT_OPEN:
                    eyelidL.SetActive(true);
                    eyelidR.SetActive(false);
                    break;
                case EyeState.BOTH_OPEN:
                    eyelidL.SetActive(false);
                    eyelidR.SetActive(false);
                    break;
            }
        }
        #endregion

    #region anim
        private string curAnimName;
        public void ChangeAnim(string animName)
        {
            ResetAnim();
            curAnimName = animName;
            animator.SetBool(curAnimName, true);
        }

        public void ResetAnim()
        {
            if (curAnimName != null)
            {
                animator.SetBool(curAnimName, false);
            }
        }
        #endregion

#if UNITY_EDITOR

        [CustomEditor(typeof(Bird))]
        public class BirdEditor : Editor
        {
            Bird bird;
            private COLOR color;

            private void OnEnable()
            {
                bird = (Bird)target;
                color = bird.color;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (!EditorApplication.isPlaying && color != bird.color)
                {
                    bird.ChangeSkinClor(bird.color);
                    color = bird.color;
                }
            }
        }

#endif
    }
}

