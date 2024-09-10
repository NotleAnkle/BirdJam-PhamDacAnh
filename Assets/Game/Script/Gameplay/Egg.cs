using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : Objects
{
    public Vector2 eggStartPos;

    //public override void SetUpTransformAndDirection(Transform tf, DIRECTION dir)
    //{
    //    base.SetUpTransformAndDirection(tf, dir);
    //}

    public override void UpdateState()
    {
        base.UpdateState();
        if (CheckOnTopBranch())
        {
            state = STATE.ACTIVE;
        }
        else
        {
            state = STATE.INACTIVE;
        }
        OnActive();
    }

    public void OnActive()
    {
        if (state == STATE.ACTIVE)
        {
            Vector3 particlePos = startPos3D;
            particlePos.z -= 5;
            ParticlePool.Play(PoolController.Ins.featherExCloudColor[(int)color], particlePos, Quaternion.identity);
            Bird bird = SimplePool.Spawn3D<Bird>(PoolController.Ins.birds3D, startPos3D, Quaternion.identity);
            LevelManager.Ins.objList[LevelManager.Ins.objList.IndexOf(this)] = bird;
            bird.OnInit(LevelManager.Ins.objList.IndexOf(bird), color, TYPE.BIRD, false, transform.parent, direction, branchSlot, slotId, cellId, startPos3D);
            LevelManager.Ins.SetupObjectLink();
            LevelManager.Ins.UpdateObjectStateCell(cellId);
            SimplePool.Despawn(this);
        }
    }
}
