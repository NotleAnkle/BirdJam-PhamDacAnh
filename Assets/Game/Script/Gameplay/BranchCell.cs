using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchCell : GameUnit
{
    public GameObject branchesPrefab;
    public Branches branches;
    public BranchData branchData = new BranchData();
    public List<GameObject> gameplayObjs;

    //[ContextMenu("Branch Sort Symmetry")]
    //public void BranchSortSymmetry()
    //{
    //    leftBranches.anchoredPosition = Vector2.zero;
    //    rightBranches.anchoredPosition = Vector2.zero;
    //    branchSortType = BRANCHSORT.SYMMETRY;
    //}

    //[ContextMenu("Branch Sort Unequal")]
    //public void BranchSortUnequal()
    //{
    //    leftBranches.anchoredPosition = new Vector2(0, 66);
    //    rightBranches.anchoredPosition = new Vector2(0, -66);
    //    branchSortType = BRANCHSORT.UNEQUAL;
    //}
    public void AddBranch()
    {
        branchData = new BranchData();
        Branches branch = Instantiate(branchesPrefab, gameObject.transform).GetComponent<Branches>();
        branches = branch;
    }

    [Button(ButtonSizes.Medium)]
    public void SetBranch()
    {
        for (int i = 0; i < branchData.branchSlots.Count; i++)
        {
            branches.branchObjs[i].SetActive(branchData.branchSlots[i].active);
        }
    }

    //[Button(ButtonSizes.Medium), HorizontalGroup("AddBird", 0.25f), GUIColor(0, 1, 0)]
    //public void AddBlue()
    //{
    //    for(int i = 0)
    //    Instantiate(birdPrefab, branches)
    //}

    //[Button(ButtonSizes.Medium), HorizontalGroup("AddBranch", 0.5f), GUIColor(0, 1, 0)]
    //public void AddLeftBranch()
    //{
    //    AddBranch(branchesObjList);
    //}

    //[Button(ButtonSizes.Medium), HorizontalGroup("AddBranch", 0.5f), GUIColor(0, 1, 0)]
    //public void AddRightBranch()
    //{
    //    AddBranch(rightBranchesList);
    //}

    //[Button(ButtonSizes.Medium), HorizontalGroup("RemoveBranch", 0.5f), GUIColor(1, 0.6f, 0.4f)]
    //public void RemoveLeftBranch()
    //{
    //    RemoveBranch(branchesObjList);
    //}

    //[Button(ButtonSizes.Medium), HorizontalGroup("RemoveBranch", 0.5f), GUIColor(1, 0.6f, 0.4f)]
    //public void RemoveRightBranch()
    //{
    //    RemoveBranch(rightBranchesList);
    //}

    public void AddBranch(List<GameObject> branchList)
    {
        for (int i = 0; i < branchList.Count; i++)
        {
            if (!branchList[i].activeInHierarchy)
            {
                branchList[i].SetActive(true);
                break;
            }
        }
    }

    public void RemoveBranch(List<GameObject> branchList)
    {
        for (int i = branchList.Count - 1; i > -1; i--)
        {
            if (branchList[i].activeInHierarchy)
            {
                branchList[i].SetActive(false);
                break;
            }
        }
    }
}

[System.Serializable]
public class BranchData
{
    public int id;
    public List<BranchSlot> branchSlots;

    public BranchData()
    {
        branchSlots = new List<BranchSlot>();
        for (int i = 0; i < 8; i++)
        {
            branchSlots.Add(new BranchSlot(false, new BranchObject(TYPE.NONE)));
        }
    }

    public BranchData(int id, List<BranchSlot> branchSlots)
    {
        this.id = id;
        this.branchSlots = branchSlots;
    }
}

[System.Serializable]
public class BranchSlot
{
    public bool active;
    public BranchObject obj;
    public int objId;
    public Objects objScript;

    public BranchSlot(bool active, BranchObject obj)
    {
        this.active = active;
        this.obj = obj;
    }

    public BranchSlot(bool active, Objects script)
    {
        this.active = active;
    }
}

[System.Serializable]
public class BranchObject
{
    public COLOR color;
    public TYPE type;
    public DIRECTION dir;
    public STATE state;
    public int linkId;
    public List<COLOR> stackList = new List<COLOR>();
    public bool hasKey;

    public BranchObject(COLOR color, TYPE type, DIRECTION dir, STATE state, int linkId, List<COLOR> stackList, bool hasKey)
    {
        this.color = color;
        this.type = type;
        this.dir = dir;
        this.state = state;
        this.linkId = linkId;
        for (int i = 0; i < stackList.Count; i++)
        {
            this.stackList.Add(stackList[i]);
        }
        this.hasKey = hasKey;
    }

    public BranchObject(COLOR birdColor, TYPE type)
    {
        this.color = birdColor;
        this.type = type;
    }

    public BranchObject(TYPE type)
    {
        this.type = type;
    }
}
