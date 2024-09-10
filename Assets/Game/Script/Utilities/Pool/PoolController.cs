using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PoolController : Singleton<PoolController>
{
    //#region OBJECT
    public Transform branchCellHolder;
    public Transform objectHolder;

    public BranchCell branchCell;
    public Objects birds3D;
    public Objects eggs3D;
    public Objects house3D;
    public Objects cage3D;
    public List<Branch3d> branchs;

    //#endregion

    #region PARTICLE
    public Transform particleHolder;

    public ParticleSystem featherExCloud;
    public ParticleSystem featherExSpark;
    public List<ParticleSystem> featherExCloudColor;

    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        branchCellHolder = LevelManager.Ins.canvasGame.mainContent;
        OnInit();
    }

    public void OnInit()
    {
        SimplePool.Preload(branchCell, 8, branchCellHolder);
        SimplePool.Preload(birds3D, 8, objectHolder);
        SimplePool.Preload(eggs3D, 2, objectHolder);
        SimplePool.Preload(house3D, 2, objectHolder);
        SimplePool.Preload(cage3D, 2, objectHolder);
        for(int i = 0; i < branchs.Count; i++)
        {
            SimplePool.Preload(branchs[i], 15, objectHolder);
        }

        ParticlePool.Preload(featherExCloud, 10, particleHolder);
        ParticlePool.Preload(featherExSpark, 10, particleHolder);
        for (int i = 0; i < featherExCloudColor.Count; i++)
        {
            ParticlePool.Preload(featherExCloudColor[i], 5, objectHolder);
        }
    }
}