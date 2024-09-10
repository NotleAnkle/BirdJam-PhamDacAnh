using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class House : Objects
{
    public Vector2 startPos;
    public TMP_Text birdNumTxt;

    public override void SetUpTransformAndDirection(Transform tf, DIRECTION dir)
    {
        base.SetUpTransformAndDirection(tf, dir);
        //rectTf.anchoredPosition = startPos;
    }

    public void UpdateBirdNum()
    {
        //birdNumTxt.text = stackList.Count.ToString();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        CheckSpawnBird();
        UpdateBirdNum();
    }

    public void CheckSpawnBird()
    {
        List<Objects> obj = LevelManager.Ins.objList;
        if (linkId > obj.Count - 1) return;
        if(linkId == -1)
        {
            SpawnBird();
        }
        else if (obj[linkId].state == STATE.LANDED || obj[linkId].state == STATE.FINISH)
        {
            SpawnBird(obj);
        }

        if(stackList.Count < 1){
            state = STATE.FINISH;
        }
        CheckFinish();
    }

    public void CheckFinish()
    {
        if(state == STATE.FINISH)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void SpawnBird(List<Objects> obj = null)
    {
        if (stackList.Count < 1) return;
        Bird bird = SimplePool.Spawn3D<Bird>(PoolController.Ins.birds3D, Vector3.zero, Quaternion.identity);
        LevelManager.Ins.objList.Add(bird);
        if (obj == null)
        {
            BranchCell cell = LevelManager.Ins.branchCells[cellId];
            int slotIndex = (direction == DIRECTION.RIGHT) ? slotId + 1 : slotId - 1;
            Transform parentTf = cell.branches.branchObjs[slotIndex].transform;
            BranchSlot slot = cell.branchData.branchSlots[slotIndex];
            bird.OnInit(LevelManager.Ins.objList.IndexOf(bird), stackList[stackList.Count - 1], TYPE.BIRD, false, parentTf, direction, slot, slotIndex, cellId, startPos3D);
        }
        else
        {
            bird.OnInit(LevelManager.Ins.objList.IndexOf(bird), stackList[stackList.Count - 1], TYPE.BIRD, false, obj[linkId].tf.parent, direction, obj[linkId].branchSlot, obj[linkId].slotId, obj[linkId].cellId, obj[linkId].startPos3D);
        }
        stackList.RemoveAt(stackList.Count - 1);
        LevelManager.Ins.SetupObjectLink();
        LevelManager.Ins.UpdateObjectStateCell((obj == null)? cellId : obj[linkId].cellId);
    }
}
