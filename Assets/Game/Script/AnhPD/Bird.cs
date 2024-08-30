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
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject eyelidL, eyelidR;
        [SerializeField] private SphereCollider collide;
        [SerializeField] private HingeJoint joint;
        [SerializeField] private AnchorHook currentHook;
        public Rigidbody rb;


        [Header("Attribute")]
        public COLOR color;
        [SerializeField] private List<SfxType> hitSfxs;
        [SerializeField] private LAND_STATE state = LAND_STATE.NONE;
        private bool isCanTouch = false;
        private Bird previousSleepyBird;
        private List<Bird> followingSleepyBirds = new List<Bird>();
        private Basket currentBasket;

        public void OnInit(COLOR skinColor)
        {
            color = skinColor;
            ChangeSkinClor(skinColor);
            collide.enabled = true;
            state = LAND_STATE.NONE;

            previousSleepyBird = null;
            followingSleepyBirds = new List<Bird>();
        }
        private void OnMouseDown()
        {
            if (!isCanTouch) return;
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
                case LAND_STATE.SLEEPY:
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
        public void OnStartFlying(Vector3 pos)
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

            sequence.Append(transform.DOMove(pos + Vector3.up * .5f, flyduration).SetEase(Ease.InBack));
            sequence.Append(transform.DOMove(pos, .25f));
            sequence.OnComplete(() =>
            {
                ChangeAnim(CONSTANTS.ANIM_IDLE);
                collide.enabled = (state != LAND_STATE.COLOR_BASKET);
                if(state == LAND_STATE.COLOR_BASKET )
                {
                    LevelManager.Ins.CheckColorBaskets(this);
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
                    case LAND_STATE.SLEEPY:
                        Bird bird = collision.gameObject.GetComponent<Bird>();
                        if(bird.state == LAND_STATE.FALLING)
                        {
                            bird.Freeze(this);
                            this.DisableOutline();
                            isCanTouch = false;
                            followingSleepyBirds.Add(bird);
                        }
                        break;
                }
            }
        }
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
    #region sleepyBird
        public void UnFreeze()
        {
            rb.useGravity = true;

            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

            previousSleepyBird?.OnFollowingSleepyBirdUnFreezy(this);
            DisableOutline();
        }
        public void Freeze(Bird bird)
        {
            if (state != LAND_STATE.FALLING) return;
            previousSleepyBird = bird;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            rb.velocity = Vector3.zero;
            rb.useGravity = false;

            state = LAND_STATE.SLEEPY;
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
    #endregion
    
    #region outline
        public void EnableOutline()
        {
            isCanTouch = true;
            skins[0].materials[1].SetFloat("_Scale", 1.075f);
        }
        public void DisableOutline()
        {
            skins[0].materials[1].SetFloat("_Scale", 0);
        }
    #endregion

    #region skin
        private void ChangeSkinClor(COLOR skinColor)
        {
            Material material = materials[(int)skinColor];
            for(int i = 0; i < skins.Length; i++)
            {
                skins[i].material = material;
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

