using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Linq;

public class LevelManager : Singleton<LevelManager>
{
    public CanvasGameplay canvasGame;
    public List<BranchCell> branchCells;
    public List<Color> colors = new List<Color>();
    public List<Objects> objList = new List<Objects>();
    List<LevelDataSO> levels = new List<LevelDataSO>();
    private Stack<Memento> undoQueue = new Stack<Memento>();
    DataManager dataIns;
    private bool onAction;

    public Sequence birdSequence;
    public bool OnAction
    {
        get { return onAction; }
        set
        {
            onAction = value;
            canvasGame.UpdateUI();
        }
    }
    [HideInInspector] public bool useAddSlot;
    int totalBird;
    int curPhase;
    float CELL_HEIGHT = 156f;
    float FLY_ANIM_TIME = 0.8f;
    float OBTAIN_ANIM_TIME = 0.3f;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        canvasGame = UIManager.Ins.OpenUIGameplay<CanvasGameplay>();
        UIManager.Ins.CloseUI<CanvasGameplay>();
    }

    private void Start()
    {
        dataIns = DataManager.Ins;
    }

    public void LoadLevel()
    {
        curPhase = 0;
        SimplePool.CollectAll();
        levels.Clear();
        int curLevel = dataIns.NormalLevelIndex + 1;
        CheckTutorial(curLevel);
        for (int i = 1; i < CONSTANTS.MAX_PHASE_PER_LEVEL; i++)
        {
            LevelDataSO level = (LevelDataSO)Resources.Load("LevelData/Lvl_" + curLevel + "_" + i);
            if (level == null) break;
            levels.Add((LevelDataSO)Resources.Load("LevelData/Lvl_" + curLevel + "_" + i));
        }
        InitLevel();
    }

    public void CheckTutorial(int curLevel)
    {
        List<FeatureUnlockData> tutData = dataIns.TutorialData.unlockDatas;
        for (int i = 0; i < tutData.Count; i++)
        {
            if (curLevel == tutData[i].level && tutData[i].unlock == false)
            {
                int index = dataIns.GameData.user.featuresUnlock;
                if (curLevel > 1) UIManager.Ins.OpenUI<CanvasPopupTutorial>().OnInit(tutData[index].featureSprite, tutData[index].featureAction, tutData[index].featureDes, tutData[index].clip, (FEATURE)index);
                dataIns.GameData.user.featuresUnlock++;
                dataIns.Save();
                tutData[i].unlock = true;
                break;
            }
        }
    }

    public void InitLevel()
    {
        OnAction = false;
        useAddSlot = false;
        totalBird = 0;
        canvasGame.ResetSlots();
        branchCells.Clear();
        undoQueue.Clear();
        objList.Clear();
        colors.Shuffle();
        LevelDataSO level = levels[curPhase];
        //Calculate cell Y position
        float yInitPos = 0 + CELL_HEIGHT / 2 * (level.branchDatas.Count - 1);
        List<int> branchIndex = new List<int>();
        List<Branch3d> branch3Ds = new List<Branch3d>();
        for (int i = 0; i < level.branchDatas.Count; i++)
        {
            BranchCell branch = SimplePool.Spawn<BranchCell>(PoolController.Ins.branchCell, new Vector3(0, yInitPos - CELL_HEIGHT * i, 0), Quaternion.identity);
            branchCells.Add(branch);
            branch.branchData = new BranchData(level.branchDatas[i].id, level.branchDatas[i].branchSlots);
            List<BranchSlot> slots = branch.branchData.branchSlots;
            BranchObject obj;

            int branchCount = 0;
            Vector3 branchPos = Vector3.zero;
            Transform objPos = null;
            branchIndex.Clear();
            branch3Ds.Clear();
            for (int k = 0; k < slots.Count; k++)
            {
                if (slots[k].active)
                {
                    branchCount++;
                    branchIndex.Add(k);
                    if (branchIndex.Count == 1)
                    {
                        branchPos = branch.branches.branchRects[branchIndex[0]].position;
                    }
                    else if (branchIndex.Count % 2 != 0)
                    {
                        branchPos = branch.branches.branchRects[branchIndex[branchIndex.Count / 2]].position;
                    }
                    else
                    {
                        branchPos = (branch.branches.branchRects[branchIndex[branchIndex.Count / 2]].position + branch.branches.branchRects[branchIndex[branchIndex.Count / 2 - 1]].position) / 2;
                    }
                    if (k == slots.Count - 1)
                    {
                        Branch3d branch3D = SimplePool.Spawn3D<Branch3d>(PoolController.Ins.branchs[branchCount - 1], branchPos, Quaternion.identity);
                        branch3Ds.Add(branch3D);
                        int tempInt = k;
                        for (int u = branch3D.slotIndex.Count - 1; u > -1; u--)
                        {
                            branch3D.slotIndex[u] = tempInt;
                            tempInt--;
                        }
                        branchIndex.Clear();
                    }
                }
                else
                {
                    if (branchCount > 0)
                    {
                        Branch3d branch3D = SimplePool.Spawn3D<Branch3d>(PoolController.Ins.branchs[branchCount - 1], branchPos, Quaternion.identity);
                        branch3Ds.Add(branch3D);
                        int tempInt = k - 1;
                        for (int u = branch3D.slotIndex.Count - 1; u > -1; u--)
                        {
                            branch3D.slotIndex[u] = tempInt;
                            tempInt--;
                        }
                        branchCount = 0;
                        branchIndex.Clear();
                    }
                }
            }

            for (int j = 0; j < slots.Count; j++)
            {

                slots[j].objId = -1;
                obj = slots[j].obj;
                if (slots[j].active && slots[j].obj.type != TYPE.NONE)
                {
                    for (int u = 0; u < branch3Ds.Count; u++)
                    {
                        for (int e = 0; e < branch3Ds[u].slotIndex.Count; e++)
                        {
                            if (j == branch3Ds[u].slotIndex[e])
                            {
                                int index = branch3Ds[u].slotIndex.IndexOf(branch3Ds[u].slotIndex[e]);
                                objPos = branch3Ds[u].listPoint[index];
                                break;
                            }
                        }
                    }
                    Vector3 finalObjPos = objPos.position;
                    finalObjPos.z -= 15;
                    switch (slots[j].obj.type)
                    {
                        case TYPE.BIRD:
                            Bird bird = SimplePool.Spawn3D<Bird>(PoolController.Ins.birds3D, finalObjPos, Quaternion.identity);
                            objList.Add(bird);
                            bird.OnInit(objList.IndexOf(bird), obj.color, TYPE.BIRD, obj.hasKey, branch.branches.branchRects[j], obj.dir, slots[j], j, i, finalObjPos);
                            totalBird++;
                            break;
                        case TYPE.EGG:
                            Egg egg = SimplePool.Spawn3D<Egg>(PoolController.Ins.eggs3D, objPos.position, Quaternion.identity);
                            objList.Add(egg);
                            egg.OnInit(objList.IndexOf(egg), obj.color, TYPE.EGG, obj.hasKey, branch.branches.branchRects[j], obj.dir, slots[j], j, i, finalObjPos);
                            totalBird++;
                            break;
                        case TYPE.HOUSE:
                            House house = SimplePool.Spawn3D<House>(PoolController.Ins.house3D, objPos.position, Quaternion.identity);
                            objList.Add(house);
                            house.OnInit(objList.IndexOf(house), obj.color, TYPE.HOUSE, obj.hasKey, branch.branches.branchRects[j], obj.dir, slots[j], j, i, finalObjPos);
                            totalBird += slots[j].obj.stackList.Count;
                            if (house.stackList.Count > 0) house.stackList.Clear();
                            for (int k = 0; k < slots[j].obj.stackList.Count; k++)
                            {
                                house.stackList.Add(slots[j].obj.stackList[k]);
                            }
                            house.UpdateBirdNum();
                            break;
                        case TYPE.CAGE:
                            Cage cage = SimplePool.Spawn3D<Cage>(PoolController.Ins.cage3D, objPos.position, Quaternion.identity);
                            objList.Add(cage);
                            cage.OnInit(objList.IndexOf(cage), obj.color, TYPE.CAGE, obj.hasKey, branch.branches.branchRects[j], obj.dir, slots[j], j, i, finalObjPos);
                            totalBird++;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        for (int i = 0; i < canvasGame.slots.Count; i++)
        {
            canvasGame.slots[i].bird = null;
        }

        SetupObjectLink();
        UpdateObjectState();
    }

    public void SetupObjectLink()
    {
        foreach (var cell in branchCells)
        {
            List<BranchSlot> slots = cell.branchData.branchSlots;
            for (int j = 0; j < slots.Count; j++)
            {
                if (slots[j].objId == -1) continue;
                Objects curScript = objList[slots[j].objId];
                if (curScript.direction == DIRECTION.RIGHT)
                    curScript.linkId = slots[j + 1].objId;
                if (curScript.direction == DIRECTION.LEFT)
                    curScript.linkId = slots[j - 1].objId;
            }
        }
    }

    public void UpdateObjectState()
    {
        for (int i = 0; i < branchCells.Count; i++)
        {
            List<BranchSlot> slots = branchCells[i].branchData.branchSlots;
            for (int j = 0; j < slots.Count; j++)
            {
                if (slots[j].objId == -1) continue;
                objList[slots[j].objId].UpdateState();
            }
        }
        canvasGame.UpdateUI();
    }

    public void UpdateObjectStateCell(int cellId)
    {
        List<BranchSlot> slots = branchCells[cellId].branchData.branchSlots;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].objId == -1) continue;
            objList[slots[i].objId].UpdateState();
        }
        canvasGame.UpdateUI();
    }

    public void OnClickObject(Bird bird)
    {
        //if (OnAction) return;
        if (GetSlotAmount() >= canvasGame.slots.Count && CheckMoving()) return;
        OnAction = true;
        int index = FindSlot(bird);
        if (index != -1)
        {
            if (canvasGame.slots[index].bird != null)
            {
                int newIndex = canvasGame.slots.FindIndex(slot => slot.bird == null);
                canvasGame.slots[newIndex].bird = canvasGame.slots[index].bird;
                canvasGame.slots[index].bird = null;
                birdSequence.Append(canvasGame.slots[newIndex].bird.tf.DOMove((Vector2)canvasGame.slots[newIndex].slotRect.position, FLY_ANIM_TIME));
            }
            UpdateBirdData(index, bird);
            bird.isMoving = true;
            ParticlePool.Play(PoolController.Ins.featherExSpark, bird.tf.position, Quaternion.identity);
            bird.ChangeAnim(CONSTANTS.ANIM_FLY);
            birdSequence.Append(bird.tf.DOMove((Vector2)canvasGame.slots[index].slotRect.position, FLY_ANIM_TIME).OnComplete(() =>
            {
                OnAction = false;
                bird.isMoving = false;
                //bird.tf.localRotation = Quaternion.Euler(0, 0, 0);
                birdSequence.Append(bird.tf.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 0.2f));
                if (CheckSlotAmount()) UpdateSlot(CheckColor2());
            }));
        }
        else
        {
            OnAction = false;
            CheckLose(CheckColor());
        }
    }

    private bool CheckMoving()
    {
        for (int i = 0; i < canvasGame.slots.Count; i++)
        {
            if (canvasGame.slots[i].bird != null)
            {
                if (canvasGame.slots[i].bird.isMoving) return true;
            }
        }
        return false;
    }

    private int FindSlot(Bird bird)
    {
        int index = -1;
        int counter = 0;
        for (int i = 0; i < canvasGame.slots.Count; i++)
        {
            if (canvasGame.slots[i].bird != null)
            {
                if (canvasGame.slots[i].bird.color == bird.color && i < canvasGame.slots.Count - 1)
                {
                    index = i + 1;
                }
                counter++;
            }
        }
        if (counter == canvasGame.slots.Count) return -1;
        if (index == -1) index = canvasGame.slots.FindIndex(slot => slot.bird == null);
        return index;
    }

    private void SortSlot()
    {
        int newIndex = 0;
        for (int i = 0; i < canvasGame.slots.Count; i++)
        {
            if (canvasGame.slots[i].bird != null)
            {
                if (newIndex != i)
                {
                    canvasGame.slots[newIndex].bird = canvasGame.slots[i].bird;
                    canvasGame.slots[i].bird = null;
                    birdSequence.Append(canvasGame.slots[newIndex].bird.tf.DOMove((Vector2)canvasGame.slots[newIndex].slotRect.position, FLY_ANIM_TIME));
                }
                newIndex++;
            }
        }
    }

    private void UpdateBirdData(int index, Bird bird)
    {
        undoQueue.Push(new Memento(branchCells, objList, canvasGame.slots, totalBird));
        bird.state = STATE.LANDED;
        canvasGame.slots[index].bird = bird;
        UpdateObjectState();
    }

    public bool CheckSlotAmount()
    {
        return GetSlotAmount() >= 3;
    }

    public int GetSlotAmount()
    {
        return canvasGame.slots.Where(slot => slot.bird != null).Select(slot => slot.bird).ToList().Count;
    }

    private COLOR CheckColor()
    {
        int[] counter = new int[9];
        foreach (var slot in canvasGame.slots)
        {
            if (slot.bird != null)
                counter[(int)slot.bird.color]++;
        }

        for (int i = 0; i < counter.Length; i++)
        {
            if (counter[i] >= 3) return (COLOR)i;
        }

        return COLOR.NONE;
    }
    private List<COLOR> CheckColor2()
    {
        int[] counter = new int[9];
        List<COLOR> colors = new List<COLOR>();
        foreach (var slot in canvasGame.slots)
        {
            if (slot.bird != null)
                counter[(int)slot.bird.color]++;
        }

        for (int i = 0; i < counter.Length; i++)
        {
            if (counter[i] >= 3)
            {
                colors.Add((COLOR)i);
            }
        }

        return colors;
    }

    private void UpdateSlot(List<COLOR> clList, bool undo = false)
    {
        if (CheckMoving()) return;
        if (clList.Count < 1) return;
        int birdCounter = 0;
        for (int k = 0; k < clList.Count; k++)
        {
            List<Bird> birdArr = new List<Bird>();
            COLOR color = clList[k];
            for (int i = 0; i < canvasGame.slots.Count; i++)
            {
                if (canvasGame.slots[i].bird != null && canvasGame.slots[i].bird.color == color)
                {
                    birdArr.Add(canvasGame.slots[i].bird);
                    canvasGame.slots[i].bird.state = STATE.FINISH;
                    canvasGame.slots[i].bird = null;
                    totalBird--;
                    birdCounter++;
                }
                if (birdCounter == 3) break;
            }
            for (int i = 0; i < birdArr.Count; i++)
            {
                if (!undo)
                {
                    if (i == 2)
                    {
                        birdSequence.Append(birdArr[i].tf.DOMoveY(canvasGame.slots[0].slotRect.position.y + 3, OBTAIN_ANIM_TIME).OnComplete(() =>
                        {
                            BirdObtainAnim(birdArr, color);
                        }));
                    }
                    else
                    {
                        birdSequence.Append(birdArr[i].tf.DOMoveY(canvasGame.slots[0].slotRect.position.y + 3, OBTAIN_ANIM_TIME));
                    }
                }
                else
                {
                    canvasGame.slots[i].bird.tf.position = canvasGame.finalPointRight.position;
                }
            }
            birdCounter = 0;
        }


        if (CheckDonePhase()) return;

        SortSlot();
        UpdateObjectState();
    }

    private void BirdObtainAnim(List<Bird> birdArr, COLOR cl)
    {
        for (int j = 0; j < birdArr.Count; j++)
        {
            if (j != 1)
            {
                birdSequence.Append(birdArr[j].tf.DOMove(birdArr[1].tf.position, OBTAIN_ANIM_TIME).SetEase(Ease.InBack).OnComplete(() =>
                {
                    Vector3 particlePos = birdArr[1].tf.position;
                    particlePos.z -= 5;
                    ParticlePool.Play(PoolController.Ins.featherExCloudColor[(int)cl], particlePos, Quaternion.identity);
                    birdArr[0].tf.position = canvasGame.finalPointRight.position;
                    birdArr[1].tf.position = canvasGame.finalPointRight.position;
                    birdArr[2].tf.position = canvasGame.finalPointRight.position;
                }));
            }
        }
    }

    private bool CheckDonePhase()
    {
        //if(totalBird == 3) UpdateSlot(CheckColor());
        if (totalBird <= 0)
        {
            TimerManager.Inst.WaitForTime(1f, () => CompletePhase());
            return true;
        }
        return false;
    }

    private void CompletePhase()
    {
        curPhase++;
        if (levels.Count <= curPhase)
        {
            Win();
        }
        else
        {
            OnAction = true;
            CanvasLoadingScreen canvas = UIManager.Ins.OpenUI<CanvasLoadingScreen>();
            TimerManager.Inst.WaitForTime(0.5f, () =>
            {
                SimplePool.CollectAll();
                InitLevel();
            });
            TimerManager.Inst.WaitForTime(1f, () => canvas.Close());
        }
    }

    private void Win()
    {
        dataIns.GameData.user.normalLevelIndex++;
        dataIns.Save();
        UIManager.Ins.OpenUI<CanvasWin>();
    }

    private void CheckLose(COLOR cl)
    {
        //bool check = true;
        //for(int i = 0; i < canvasGame.slots.Count; i++)
        //{
        //    if (canvasGame.slots[i].bird == null) 
        //    {
        //        check = false;
        //        break;
        //    } 
        //}
        //if (cl == COLOR.NONE && check)
        //{
        CanvasSaveMe cvSaveMe = UIManager.Ins.OpenUI<CanvasSaveMe>();
        cvSaveMe.CheckKeepPlay(CheckAddSlot());
        //}
    }

    public bool CheckAddSlot()
    {
        if (!DataManager.Ins.TutorialData.unlockDatas[(int)FEATURE.BOOSTER_EXTRA_SPACE].unlock) return false;
        return !useAddSlot;
    }
    public void AddSlot(bool isZero = true)
    {
        canvasGame.OnAddSlot();
        useAddSlot = true;
        if (!isZero) DataManager.Ins.GameData.user.boosterAdd--;
    }

    public bool CheckUndo()
    {
        if (!DataManager.Ins.TutorialData.unlockDatas[(int)FEATURE.BOOSTER_UNDO].unlock) return false;
        if (undoQueue.Count < 1) return false;
        return true;
    }

    public void Undo(bool isZero = true)
    {
        if (undoQueue.Count < 1 || OnAction) return;
        OnAction = false;
        DOTween.KillAll();
        Memento data = undoQueue.Pop();
        totalBird = data.totalBird;

        for (int i = 0; i < data.objMem.Count; i++)
        {
            objList[i].state = data.objMem[i].state;
            objList[i].stackList.Clear();
            objList[i].tf.localScale = Vector3.one;
            objList[i].SetObjectDirection();
            foreach (var color in data.objMem[i].stackList)
            {
                objList[i].stackList.Add(color);
            }
            if (objList[i].objectType != data.objMem[i].type)
            {
                switch (data.objMem[i].type)
                {
                    case TYPE.EGG:
                        objList[i].EggTrans();
                        break;
                    case TYPE.CAGE:
                        objList[i].CageTrans();
                        break;
                }
            }
            objList[i].linkId = data.objMem[i].linkId;
        }

        if (objList.Count > data.objMem.Count)
        {
            for (int i = objList.Count - 1; i > data.objMem.Count - 1; i--)
            {
                Objects obj = objList[i];
                objList.RemoveAt(i);
                SimplePool.Despawn(obj);
            }
        }

        for (int i = 0; i < data.slotMem.Count; i++)
        {
            canvasGame.slots[i].bird = data.slotMem[i].bird;
            if (data.slotMem[i].bird != null)
            {
                data.slotMem[i].bird.tf.position = (Vector2)canvasGame.slots[i].slotRect.position;
            }
        }

        int index = 0;
        foreach (var branchCell in branchCells)
        {
            foreach (var slot in branchCell.branchData.branchSlots)
            {
                slot.objId = data.objIdSort[index];
                index++;
            }
        }

        if (!isZero) DataManager.Ins.GameData.user.boosterUndo--;
        UpdateObjectState();
        UpdateSlot(CheckColor2(), true);
    }

    private COLOR GetMagnetColor()
    {
        COLOR randomColor = COLOR.NONE;
        int[] counter = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        for (int i = 0; i < objList.Count; i++)
        {
            if (objList[i].objectType == TYPE.BIRD && objList[i].state != STATE.LANDED && objList[i].state != STATE.FINISH)
            {
                counter[(int)objList[i].color]++;
            }
        }

        List<COLOR> colors = new List<COLOR>();
        for (int i = 0; i < counter.Length; i++)
        {
            if (counter[i] >= 3) colors.Add((COLOR)i);
        }

        if (colors.Count > 0)
        {
            int randomIndex = Random.Range(0, colors.Count);
            randomColor = colors[randomIndex];
        }

        return randomColor;
    }

    public bool CheckMagnet()
    {
        if (!DataManager.Ins.TutorialData.unlockDatas[(int)FEATURE.BOOSTER_MAGNET].unlock) return false;
        COLOR color = GetMagnetColor();
        if (color == COLOR.NONE || OnAction) return false;
        return true;
    }

    public void MagnetBooster(bool isZero = false)
    {
        if (!CheckMagnet()) return;
        COLOR color = GetMagnetColor();
        undoQueue.Push(new Memento(branchCells, objList, canvasGame.slots, totalBird));
        int counter = 0;
        for (int i = 0; i < objList.Count; i++)
        {
            if (objList[i].objectType == TYPE.BIRD && objList[i].state != STATE.LANDED && objList[i].state != STATE.FINISH && objList[i].color == color)
            {
                objList[i].tf.DOMove(canvasGame.finalPointRight.position, FLY_ANIM_TIME);
                objList[i].state = STATE.FINISH;
                totalBird--;
                counter++;
                if (counter == 3) break;
            }
        }
        if (!isZero) DataManager.Ins.GameData.user.boosterMagnet--;
        if (CheckDonePhase()) return;
        UpdateObjectState();
    }

    private List<Objects> GetBirdList()
    {
        List<Objects> tempObjList = new List<Objects>();
        for (int i = 0; i < objList.Count; i++)
        {
            if (objList[i].objectType == TYPE.BIRD && objList[i].state != STATE.LANDED && objList[i].state != STATE.FINISH)
            {
                tempObjList.Add(objList[i]);
            }
        }
        return tempObjList;
    }

    public bool CheckShuffle()
    {
        if (!DataManager.Ins.TutorialData.unlockDatas[(int)FEATURE.BOOSTER_SHUFFLE].unlock) return false;
        int counter = 0;
        for (int i = 0; i < objList.Count; i++)
        {
            if (objList[i].objectType == TYPE.BIRD && objList[i].state != STATE.LANDED && objList[i].state != STATE.FINISH)
            {
                counter++;
            }
        }
        if (counter < 2 || OnAction) return false;
        return true;
    }

    public void Shuffle(bool isZero = true)
    {
        if (!CheckShuffle()) return;
        undoQueue.Clear();
        List<Objects> objList = GetBirdList();
        while (objList.Count > 1)
        {
            int i1 = Random.Range(0, objList.Count);
            Objects obj1 = objList[i1];
            objList.RemoveAt(i1);
            int i2 = Random.Range(0, objList.Count);
            Objects obj2 = objList[i2];
            objList.RemoveAt(i2);

            //Get Obj2 Data
            Transform parent1 = obj2.tf.parent;
            DIRECTION dir1 = obj2.direction;
            BranchSlot slot1 = obj2.branchSlot;
            int slotId1 = obj2.slotId;
            int cellId1 = obj2.cellId;

            //Get Obj1 Data
            Transform parent2 = obj1.tf.parent;
            DIRECTION dir2 = obj1.direction;
            BranchSlot slot2 = obj1.branchSlot;
            int slotId2 = obj1.slotId;
            int cellId2 = obj1.cellId;

            obj1.SetUpTransformAndDirection(parent1, dir1);
            obj1.SetUpSlot(slot1, slotId1, cellId1);
            obj2.SetUpTransformAndDirection(parent2, dir2);
            obj2.SetUpSlot(slot2, slotId2, cellId2);
        }
        if (!isZero) DataManager.Ins.GameData.user.boosterShuffle--;
        SetupObjectLink();
        UpdateObjectState();
    }
}

[System.Serializable]
public class Memento
{
    public List<BranchObject> objMem = new List<BranchObject>();
    public List<SlotData> slotMem = new List<SlotData>();
    public List<int> objIdSort = new List<int>();
    public int totalBird;

    public Memento(List<BranchCell> branchCell, List<Objects> objList, List<Slot> slotList, int totalBird)
    {
        for (int i = 0; i < objList.Count; i++)
        {
            objMem.Add(new BranchObject(objList[i].color, objList[i].objectType, objList[i].direction, objList[i].state, objList[i].linkId, objList[i].stackList, objList[i].hasKey));
        }

        for (int i = 0; i < branchCell.Count; i++)
        {
            for (int j = 0; j < branchCell[i].branchData.branchSlots.Count; j++)
            {
                objIdSort.Add(branchCell[i].branchData.branchSlots[j].objId);
            }
        }

        for (int i = 0; i < slotList.Count; i++)
        {
            slotMem.Add(new(slotList[i].bird));
        }
        this.totalBird = totalBird;
    }
}
