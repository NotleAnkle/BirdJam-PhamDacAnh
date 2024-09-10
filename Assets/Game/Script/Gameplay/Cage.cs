using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cage : Objects
{
    public Vector2 startPos;
    public Image birdImg;
    public List<GameObject> birdObjs;

    public override void SetUpTransformAndDirection(Transform tf, DIRECTION dir)
    {
        base.SetUpTransformAndDirection(tf, dir);
    }

    public override void SetColor(COLOR color)
    {
        //birdImg.color = colors[(int)color];
        for (int i = 0; i < birdObjs.Count; i++)
        {
            birdObjs[i].SetActive(false);
        }
        birdObjs[(int)color].SetActive(true);
    }

    public override void SetColorFixed()
    {
        birdImg.color = fixedColor[(int)color];
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (CheckKey())
        {
            Bird bird = SimplePool.Spawn3D<Bird>(PoolController.Ins.birds3D, Vector3.zero, Quaternion.identity);
            bird.ResetObject();
            LevelManager.Ins.objList[LevelManager.Ins.objList.IndexOf(this)] = bird;
            bird.OnInit(LevelManager.Ins.objList.IndexOf(bird), color, TYPE.BIRD, false, transform.parent, direction, branchSlot, slotId, cellId, startPos3D);
            LevelManager.Ins.SetupObjectLink();
            LevelManager.Ins.UpdateObjectStateCell(bird.cellId);
            SimplePool.Despawn(this);
        }
    }

    public bool CheckKey()
    {
        List<Objects> objList = LevelManager.Ins.objList;
        bool key = false;
        for (int i = 0; i < objList.Count; i++)
        {
            if (objList[i].objectType == TYPE.BIRD && objList[i].hasKey && objList[i].state == STATE.FINISH && objList[i].color == color)
            {
                key = true;
                break;
            }
        }
        return key;
    }
}
