using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Bird : Objects
{
    public Transform modelTf;
    public BoxCollider birdCol;
    public Image birdImg;
    //public Button tapBtn;
    //public Image eyeImg;
    //public Image sleepEyeImg;
    //public Image keyImage;
    public Vector2 startPos;
    [SerializeField] List<GameObject> birdObjs;
    public bool isMoving;


    public override void OnInit(int objId, COLOR color, TYPE type, bool hasKey, Transform parentTf, DIRECTION dir, BranchSlot slot, int slotId, int cellId, Vector3 startPos)
    {
        base.OnInit(objId, color, type, hasKey, parentTf, dir, slot, slotId, cellId, startPos);
        animator = birdObjs[(int)color].GetComponent<Animator>();
    }
    public override void SetColor(COLOR color)
    {
        //birdImg.color = colors[(int)color];
        for(int i = 0; i < birdObjs.Count; i++)
        {
            birdObjs[i].SetActive(false);
        }
        birdObjs[(int)color].SetActive(true);
    }

    public override void SetColorFixed()
    {
        birdImg.color = fixedColor[(int)color];
    }

    public override void SetObjectDirection()
    {
        base.SetObjectDirection();
        if (state == STATE.LANDED) 
        {
            tf.localRotation = Quaternion.Euler(0, 0, 0);
            return;
        } 
        if (direction == DIRECTION.RIGHT) tf.localRotation = Quaternion.Euler(0, -30, 0);
        else tf.localRotation = Quaternion.Euler(0, 30, 0);
    }

    public override void SetUpTransformAndDirection(Transform tf, DIRECTION dir)
    {
        base.SetUpTransformAndDirection(tf, dir);
        //rectTf.anchoredPosition = startPos;
    }
    public override void SetKeyImage()
    {
        //if (hasKey) keyImage.gameObject.SetActive(true);
        //else keyImage.gameObject.SetActive(false);
    }


    public override void UpdateState()
    {
        base.UpdateState();
        if (state != STATE.LANDED && state != STATE.FINISH)
        {
            //rectTf.anchoredPosition = startPos; 
            //Vector3 branchPos = LevelManager.Ins.branchCells[cellId].branches.branchRects[slotId].position;
            //branchPos.z -= 15;
            //tf.position = branchPos;
            tf.position = startPos3D;
            if (CheckOnTopBranch())
            {
                state = STATE.ACTIVE;
                //tapBtn.interactable = true;
            }
            else
            {
                state = STATE.INACTIVE;
                //tapBtn.interactable = false;
            }
        }
        else
        {
            //rectTf.localScale = Vector3.one;
            //tapBtn.interactable = false;
        }
        UpdateStateVisual();
    }

    public void UpdateStateVisual()
    {
        switch (state)
        {
            case STATE.LANDED:
                break;
            case STATE.ACTIVE:
                //eyeImg.gameObject.SetActive(true);
                //sleepEyeImg.gameObject.SetActive(false);
                break;
            case STATE.INACTIVE:
                //eyeImg.gameObject.SetActive(false);
                //sleepEyeImg.gameObject.SetActive(true);
                break;
        }
    }

    public void OnClick()
    {
        if (state != STATE.ACTIVE) return;
        if (GameManager.Ins.IsState(GameState.Pause)) return;
        LevelManager.Ins.OnClickObject(this);
    }
}
