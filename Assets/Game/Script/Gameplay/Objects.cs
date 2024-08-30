using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objects : GameUnit
{
    public int objectId;
    public COLOR color;
    public TYPE objectType;
    public DIRECTION direction;
    public bool hasKey;
    public STATE state;
    public int cellId;
    public int slotId;
    public BranchSlot branchSlot;
    [HideInInspector] public Vector3 leftDirScale = new Vector3(-1, 1, 1);
    public int linkId;
    public List<COLOR> stackList = new List<COLOR>();
    public List<Color> fixedColor;
    public Vector3 startPos3D;
    public Animator animator;
    protected string curAnimName;

    public virtual void OnInit(int objId, COLOR color, TYPE type, bool hasKey, Transform parentTf, DIRECTION dir, BranchSlot slot, int slotId, int cellId, Vector3 startPos)
    {
        tf.localScale = Vector3.one;
        state = STATE.INACTIVE;
        objectId = objId;
        this.color = color;
        objectType = type;
        this.hasKey = hasKey;
        startPos3D = startPos;
        SetKeyImage();
        SetColor(color);
        SetUpTransformAndDirection(parentTf, dir);
        SetUpSlot(slot, slotId, cellId);
        SetObjectDirection();
    }

    public void ResetObject()
    {
        color = COLOR.NONE;
        objectType = TYPE.NONE;
        hasKey = false;
        state = STATE.INACTIVE;
        branchSlot = null;
        stackList.Clear();
    }

    public virtual void SetObjectDirection()
    {
        if (direction == DIRECTION.RIGHT) tf.localRotation = Quaternion.Euler(0, -30, 0);
        else tf.localRotation = Quaternion.Euler(0, 30, 0);
    }

    public virtual void SetUpTransformAndDirection(Transform tf, DIRECTION dir)
    {
        //this.tf.SetParent(tf);
        direction = dir;
        SetObjectDirection();
    }

    public virtual void SetUpSlot(BranchSlot slot, int slotId, int cellId)
    {
        this.slotId = slotId;
        this.cellId = cellId;
        branchSlot = slot;
        branchSlot.objId = objectId;
    }

    public virtual void SetColor(COLOR color) { }
    public virtual void SetColorFixed() { }

    public virtual void SetKeyImage() { }


    public void EggTrans()
    {
        Egg egg = SimplePool.Spawn3D<Egg>(PoolController.Ins.eggs3D, Vector3.zero, Quaternion.identity);
        egg.ResetObject();
        LevelManager.Ins.objList[LevelManager.Ins.objList.IndexOf(this)] = egg;
        egg.objectId = LevelManager.Ins.objList.IndexOf(egg);
        egg.SetUpTransformAndDirection(transform.parent, direction);
        egg.SetUpSlot(branchSlot, slotId, cellId);
        egg.color = color;
        egg.state = STATE.INACTIVE;
        egg.objectType = TYPE.EGG;
        LevelManager.Ins.SetupObjectLink();
        SimplePool.Despawn(this);
    }

    public void CageTrans()
    {
        Cage cage = SimplePool.Spawn3D<Cage>(PoolController.Ins.cage3D, Vector3.zero, Quaternion.identity);
        cage.ResetObject();
        LevelManager.Ins.objList[LevelManager.Ins.objList.IndexOf(this)] = cage;
        cage.objectId = LevelManager.Ins.objList.IndexOf(cage);
        cage.SetUpTransformAndDirection(transform.parent, direction);
        cage.SetUpSlot(branchSlot, slotId, cellId);
        cage.color = color;
        cage.state = STATE.INACTIVE;
        cage.SetColor(color);
        cage.objectType = TYPE.CAGE;
        LevelManager.Ins.SetupObjectLink();
        SimplePool.Despawn(this);
    }

    public bool CheckOnTopBranch()
    {
        List<Objects> objs = LevelManager.Ins.objList;
        List<BranchSlot> slots = LevelManager.Ins.branchCells[cellId].branchData.branchSlots;
        int start = (direction == DIRECTION.RIGHT) ? slotId + 1 : slotId - 1;
        int end = (direction == DIRECTION.RIGHT) ? slots.Count : -1;
        int step = (direction == DIRECTION.RIGHT) ? 1 : -1;

        if (start == end) return true;

        for (int i = start; i != end; i += step)
        {
            if (!slots[i].active) break;
            if (slots[i].objId != -1)
            {
                if (objs[slots[i].objId].state != STATE.LANDED && objs[slots[i].objId].state != STATE.FINISH)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public virtual void UpdateState(){ }

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
}
