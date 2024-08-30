using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class UICanvas : MonoBehaviour
{
    //public bool IsAvoidBackKey = false;
    [FormerlySerializedAs("IsDestroyOnClose")]
    public bool isDestroyOnClose;

    [SerializeField]  private CanvasGroup canvasGroup;

    [SerializeField] private bool useAnimator;
    [SerializeField] private bool useFadeTransition;
    [ShowIf("useAnimator")] [SerializeField]
    protected Animator animator;

    private string _currentAnim = " ";

    private RectTransform _mRectTransform;

    [SerializeField] protected RectTransform rectTFContent;

    public RectTransform MRectTransform
    {
        get
        {
            _mRectTransform = _mRectTransform ? _mRectTransform : gameObject.transform as RectTransform;
            return _mRectTransform;
        }
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void Setup(object param = null)
    {
        UIManager.Ins.AddBackUI(this);
        UIManager.Ins.PushBackAction(this, BackKey);
    }

    protected virtual void BackKey()
    {

    }

    public virtual void Open(object param = null)
    {
        UpdateUI();
        if (useFadeTransition) canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        //anim
        if (useAnimator) OpenAnimationAnim();
        if (useFadeTransition) RunFadeInTransition();

        if (rectTFContent != null)
        {
            rectTFContent.DOScale(1.05f, .2f)
                .OnComplete(() =>
                {
                    rectTFContent.DOScale(1, .1f);
                });
        }
    }

    private void RunFadeInTransition()
    {
        canvasGroup.alpha = 0;
        DOVirtual.Float(0, 1, 0.2f, (v) =>
        {
            canvasGroup.alpha = v;
        });
    }

    private void RunFadeOutTransition()
    {
        DOVirtual.Float(1, 0, 0.2f, (v) =>
        {
            canvasGroup.alpha = v;
        }).OnComplete(OnClose);
    }

    private void OpenAnimationAnim()
    {
        //ChangeAnim(Constants.OPEN);
    }

    private void CloseAnimationAnim(Action onCompleteAction, float time = 0.2f)
    {
        //ChangeAnim(Constants.CLOSE);
        DOVirtual.DelayedCall(time, () => onCompleteAction?.Invoke());
    }
    
    public virtual void UpdateUI() {}

    public virtual void Close()
    {
        UIManager.Ins.RemoveBackUI(this);
        //anim

        if (rectTFContent != null)
        {
            rectTFContent.DOScale(.5f, .25f);
        }

        if (useFadeTransition)
        {
            RunFadeOutTransition();
            return;
        }
        if (useAnimator) CloseAnimationAnim(OnClose);
        else OnClose();
    }

    public virtual void CloseDirectly(object param = null)
    {
        if (UIManager.Ins.IsContain(this))
        {
            UIManager.Ins.RemoveBackUI(this);
        }
        if (useAnimator) CloseAnimationAnim(OnClose);
        else OnClose();
    }
    
    private void OnClose()
    {
        gameObject.SetActive(false);
        if (isDestroyOnClose) Destroy(gameObject);
    }

    private void ChangeAnim(string animName)
    {
        animator.ResetTrigger(_currentAnim);
        _currentAnim = animName;
        animator.SetTrigger(_currentAnim);
    }
}
