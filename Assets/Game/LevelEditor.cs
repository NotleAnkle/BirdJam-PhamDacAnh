//using Newtonsoft.Json.Linq;
//using Sirenix.OdinInspector;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEditor;
//using UnityEngine;

//public class LevelEditor : MonoBehaviour
//{
//    public string LevelName;
//    public List<BranchCell> branchCells;
//    public List<BranchData> branchDatas;
//    public Transform mainContentTf;
//    public GameObject branchCellPrefab;
//    public GameObject birdPrefab;
//    public GameObject cagePrefab;
//    public GameObject housePrefab;
//    public GameObject eggPrefab;

//    #region STATS
//    float INIT_Y_POS = 546f;
//    float CELL_HEIGHT = 156f;
//    int MAX_CELL = 8;
//    #endregion

//    [PropertySpace(SpaceBefore = 20)]
//    [Button(ButtonSizes.Medium), GUIColor(0, 1, 0), HorizontalGroup("SetAndAdd", 0.33f)]
//    public void AddCell()
//    {
//        if (branchCells.Count >= MAX_CELL)
//        {
//            Debug.LogError("Max Cell reached");
//            return;
//        }
//        BranchCell branchCell = Instantiate(branchCellPrefab, mainContentTf).GetComponent<BranchCell>();
//        branchCells.Add(branchCell);
//        branchCell.branchData.id = branchCells.Count - 1;
//        branchCell.rectTf.anchoredPosition = new Vector2(0, INIT_Y_POS - (branchCells.Count - 1) * CELL_HEIGHT);
//        branchDatas.Add(branchCell.branchData);
//    }

//    public void SetBranch()
//    {
//        for (int i = 0; i < branchCells.Count; i++)
//        {
//            for (int j = 0; j < branchCells[i].branches.branchObjs.Count; j++)
//            {
//                branchCells[i].branchData = branchDatas[i];
//                branchCells[i].branches.branchObjs[j].SetActive(branchCells[i].branchData.branchSlots[j].active);
//            }
//        }
//    }

//    [PropertySpace(SpaceBefore = 20)]
//    [Button(ButtonSizes.Medium), HorizontalGroup("SetAndAdd", 0.33f)]
//    public void UpdateAll()
//    {
//        SetBranch();
//        SetObject();
//    }

//    public void SetObject()
//    {
//        int index = 0;
//        for (int i = 0; i < branchDatas.Count; i++)
//        {
//            for (int j = 0; j < branchDatas[i].branchSlots.Count; j++)
//            {
//                if (branchDatas[i].branchSlots[j].obj.type != TYPE.NONE && branchDatas[i].branchSlots[j].active)
//                {
//                    branchDatas[i].branchSlots[j].obj.color = branchDatas[i].branchSlots[j].objScript.color;
//                    branchDatas[i].branchSlots[j].obj.type = branchDatas[i].branchSlots[j].objScript.objectType;
//                    branchDatas[i].branchSlots[j].obj.dir = branchDatas[i].branchSlots[j].objScript.direction;
//                    branchDatas[i].branchSlots[j].obj.stackList = branchDatas[i].branchSlots[j].objScript.stackList;
//                    branchDatas[i].branchSlots[j].obj.hasKey = branchDatas[i].branchSlots[j].objScript.hasKey;
//                    branchDatas[i].branchSlots[j].objScript.SetColorFixed();
//                    branchDatas[i].branchSlots[j].objScript.SetObjectDirection();
//                    branchDatas[i].branchSlots[j].objScript.SetKeyImage();
//                    index++;
//                }
//            }
//        }
//    }

//    [PropertySpace(SpaceBefore = 20)]
//    [Button(ButtonSizes.Medium), HorizontalGroup("SetAndAdd", 0.33f), GUIColor(1, 0.6f, 0.4f)]
//    public void ClearAll()
//    {
//        for (int i = 0; i < branchCells.Count; i++)
//        {
//            DestroyImmediate(branchCells[i].gameObject);
//        }
//        branchCells.Clear();
//        branchDatas.Clear();
//    }

//    [PropertySpace(SpaceBefore = 20)]
//    [Button(ButtonSizes.Medium), HorizontalGroup("AddBird", 0.2f),]
//    public void Blue()
//    {
//        SetBranch();
//        AddBird(COLOR.BLUE);
//    }

//    [PropertySpace(SpaceBefore = 20)]
//    [Button(ButtonSizes.Medium), HorizontalGroup("AddBird", 0.2f)]
//    public void Red()
//    {
//        SetBranch();
//        AddBird(COLOR.RED);
//    }

//    [PropertySpace(SpaceBefore = 20)]
//    [Button(ButtonSizes.Medium), HorizontalGroup("AddBird", 0.2f)]
//    public void Green()
//    {
//        SetBranch();
//        AddBird(COLOR.GREEN);
//    }

//    [PropertySpace(SpaceBefore = 20)]
//    [Button(ButtonSizes.Medium), HorizontalGroup("AddBird", 0.2f)]
//    public void Yellow()
//    {
//        SetBranch();
//        AddBird(COLOR.YELLOW);
//    }

//    [PropertySpace(SpaceBefore = 20)]
//    [Button(ButtonSizes.Medium), HorizontalGroup("AddBird", 0.2f)]
//    public void Pink()
//    {
//        SetBranch();
//        AddBird(COLOR.PINK);
//    }

//    [Button(ButtonSizes.Medium), HorizontalGroup("AddBird2", 0.25f)]
//    public void Orange()
//    {
//        SetBranch();
//        AddBird(COLOR.ORANGE);
//    }

//    [Button(ButtonSizes.Medium), HorizontalGroup("AddBird2", 0.25f)]
//    public void Cyan()
//    {
//        SetBranch();
//        AddBird(COLOR.CYAN);
//    }

//    [Button(ButtonSizes.Medium), HorizontalGroup("AddBird2", 0.25f)]
//    public void Magenta()
//    {
//        SetBranch();
//        AddBird(COLOR.MAGENTA);
//    }

//    [Button(ButtonSizes.Medium), HorizontalGroup("AddBird2", 0.25f)]
//    public void LightGreen()
//    {
//        SetBranch();
//        AddBird(COLOR.LIGHTGREEN);
//    }

//    [PropertySpace(SpaceBefore = 20)]
//    [Button(ButtonSizes.Medium), HorizontalGroup("AddObj", 0.33f)]
//    public void AddEgg()
//    {
//        for (int i = 0; i < branchCells.Count; i++)
//        {
//            if (added) break;
//            List<BranchSlot> slots = branchCells[i].branchData.branchSlots;
//            for (int j = 0; j < slots.Count; j++)
//            {
//                if (slots[j].active && slots[j].obj.type == TYPE.NONE)
//                {
//                    Egg egg = Instantiate(eggPrefab, branchCells[i].branches.branchRects[j]).GetComponent<Egg>();
//                    branchCells[i].gameplayObjs.Add(egg.gameObject);
//                    branchCells[i].branchData.branchSlots[j].objScript = egg;
//                    slots[j].obj.type = TYPE.EGG;
//                    added = true;
//                    ObjectEditor edit = egg.gameObject.GetComponent<ObjectEditor>();
//                    edit.cellId = i;
//                    edit.slotId = j;
//                    edit.lvEditor = this;
//                    break;
//                }
//            }
//        }
//        added = false;
//    }

//    [PropertySpace(SpaceBefore = 20)]
//    [Button(ButtonSizes.Medium), HorizontalGroup("AddObj", 0.33f)]
//    public void AddCage()
//    {
//        for (int i = 0; i < branchCells.Count; i++)
//        {
//            if (added) break;
//            List<BranchSlot> slots = branchCells[i].branchData.branchSlots;
//            for (int j = 0; j < slots.Count; j++)
//            {
//                if (slots[j].active && slots[j].obj.type == TYPE.NONE)
//                {
//                    Cage cage = Instantiate(cagePrefab, branchCells[i].branches.branchRects[j]).GetComponent<Cage>();
//                    branchCells[i].gameplayObjs.Add(cage.gameObject);
//                    branchCells[i].branchData.branchSlots[j].objScript = cage;
//                    slots[j].obj.type = TYPE.CAGE;
//                    added = true;
//                    cage.SetColorFixed();
//                    ObjectEditor edit = cage.gameObject.GetComponent<ObjectEditor>();
//                    edit.cellId = i;
//                    edit.slotId = j;
//                    edit.lvEditor = this;
//                    break;
//                }
//            }
//        }
//        added = false;
//    }

//    [PropertySpace(SpaceBefore = 20)]
//    [Button(ButtonSizes.Medium), HorizontalGroup("AddObj", 0.33f)]
//    public void AddHouse()
//    {
//        for (int i = 0; i < branchCells.Count; i++)
//        {
//            if (added) break;
//            List<BranchSlot> slots = branchCells[i].branchData.branchSlots;
//            for (int j = 0; j < slots.Count; j++)
//            {
//                if (slots[j].active && slots[j].obj.type == TYPE.NONE)
//                {
//                    House house = Instantiate(housePrefab, branchCells[i].branches.branchRects[j]).GetComponent<House>();
//                    branchCells[i].gameplayObjs.Add(house.gameObject);
//                    branchCells[i].branchData.branchSlots[j].objScript = house;
//                    slots[j].obj.type = TYPE.HOUSE;
//                    ObjectEditor edit = house.gameObject.GetComponent<ObjectEditor>();
//                    edit.cellId = i;
//                    edit.slotId = j;
//                    edit.lvEditor = this;
//                    added = true;
//                    break;
//                }
//            }
//        }
//        added = false;
//    }

//    bool added;
//    public void AddBird(COLOR color)
//    {
//        for (int i = 0; i < branchCells.Count; i++)
//        {
//            if (added) break;
//            List<BranchSlot> slots = branchCells[i].branchData.branchSlots;
//            for (int j = 0; j < slots.Count; j++)
//            {
//                if (slots[j].active && slots[j].obj.type == TYPE.NONE)
//                {
//                    Bird bird = Instantiate(birdPrefab, branchCells[i].branches.branchRects[j]).GetComponent<Bird>();
//                    branchCells[i].gameplayObjs.Add(bird.gameObject);
//                    branchCells[i].branchData.branchSlots[j].objScript = bird;
//                    slots[j].obj.type = TYPE.BIRD;
//                    slots[j].obj.color = color;
//                    bird.color = color;
//                    bird.SetColorFixed();
//                    ObjectEditor edit = bird.gameObject.GetComponent<ObjectEditor>();
//                    edit.cellId = i;
//                    edit.slotId = j;
//                    added = true;
//                    edit.lvEditor = this;
//                    break;
//                }
//            }
//        }
//        added = false;
//    }

//    bool removed;
//    [Button(ButtonSizes.Medium), GUIColor(1, 0.6f, 0.4f)]
//    public void RemoveLastObject()
//    {
//        for (int i = branchCells.Count - 1; i > -1; i--)
//        {
//            if (removed) break;
//            List<BranchSlot> slots = branchCells[i].branchData.branchSlots;
//            for (int j = slots.Count - 1; j > -1; j--)
//            {
//                if (slots[j].obj.type != TYPE.NONE)
//                {
//                    DestroyImmediate(branchCells[i].gameplayObjs[branchCells[i].gameplayObjs.Count - 1]);
//                    branchCells[i].gameplayObjs.RemoveAt(branchCells[i].gameplayObjs.Count - 1);
//                    slots[j].obj.type = TYPE.NONE;
//                    removed = true;
//                    break;
//                }
//            }
//        }
//        removed = false;
//    }

//    [ContextMenu("Load")]
//    public void LoadLevel()
//    {
//        LevelDataSO lvlData = ((LevelDataSO)Resources.Load("LevelData/Lvl_" + LevelName));
//        for (int i = 0; i < lvlData.branchDatas.Count; i++)
//        {
//            AddCell();
//            branchDatas[i] = lvlData.branchDatas[i];
//            List<BranchSlot> slots = branchDatas[i].branchSlots;
//            for (int j = 0; j < slots.Count; j++)
//            {
//                switch (slots[j].obj.type)
//                {
//                    case TYPE.BIRD:
//                        Bird bird = Instantiate(birdPrefab, branchCells[i].branches.branchRects[j]).GetComponent<Bird>();
//                        branchCells[i].gameplayObjs.Add(bird.gameObject);
//                        bird.color = slots[j].obj.color;
//                        bird.direction = slots[j].obj.dir;
//                        bird.hasKey = slots[j].obj.hasKey;
//                        bird.SetColorFixed();
//                        bird.SetObjectDirection();
//                        ObjectEditor edit = bird.gameObject.GetComponent<ObjectEditor>();
//                        edit.cellId = i;
//                        edit.slotId = j;
//                        edit.lvEditor = this;
//                        edit.AssignScript();
//                        break;
//                    case TYPE.HOUSE:
//                        House house = Instantiate(housePrefab, branchCells[i].branches.branchRects[j]).GetComponent<House>();
//                        branchCells[i].gameplayObjs.Add(house.gameObject);
//                        house.stackList = slots[j].obj.stackList;
//                        house.direction = slots[j].obj.dir;
//                        house.SetObjectDirection();
//                        ObjectEditor editHouse = house.gameObject.GetComponent<ObjectEditor>();
//                        editHouse.cellId = i;
//                        editHouse.slotId = j;
//                        editHouse.lvEditor = this;
//                        editHouse.AssignScript();
//                        break;
//                    case TYPE.CAGE:
//                        Cage cage = Instantiate(cagePrefab, branchCells[i].branches.branchRects[j]).GetComponent<Cage>();
//                        branchCells[i].gameplayObjs.Add(cage.gameObject);
//                        cage.color = slots[j].obj.color;
//                        cage.direction = slots[j].obj.dir;
//                        cage.SetColorFixed();
//                        cage.SetObjectDirection();
//                        ObjectEditor editCage = cage.gameObject.GetComponent<ObjectEditor>();
//                        editCage.cellId = i;
//                        editCage.slotId = j;
//                        editCage.lvEditor = this;
//                        editCage.AssignScript();
//                        break;
//                    case TYPE.EGG:
//                        Egg egg = Instantiate(eggPrefab, branchCells[i].branches.branchRects[j]).GetComponent<Egg>();
//                        branchCells[i].gameplayObjs.Add(egg.gameObject);
//                        egg.color = slots[j].obj.color;
//                        ObjectEditor editEgg = egg.gameObject.GetComponent<ObjectEditor>();
//                        editEgg.cellId = i;
//                        editEgg.slotId = j;
//                        editEgg.lvEditor = this;
//                        editEgg.AssignScript();
//                        break;
//                }
//            }
//        }
//        UpdateAll();
//    }

//    [ContextMenu("Export")]
//    public void ExportLevel()
//    {
//        UpdateAll();
//        LevelDataSO lvlData = ScriptableObject.CreateInstance<LevelDataSO>();
//        AssetDatabase.CreateAsset(lvlData, "Assets/Game/Resources/LevelData/Lvl_" + LevelName + ".asset");
//        AssetDatabase.SaveAssets();
//        EditorUtility.FocusProjectWindow();
//        EditorUtility.SetDirty(lvlData);
//        Selection.activeObject = lvlData;
//        for (int i = 0; i < branchCells.Count; i++)
//        {
//            lvlData.branchDatas.Add(branchCells[i].branchData);
//        }
//    }
//}
