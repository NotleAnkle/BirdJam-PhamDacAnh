//using Sirenix.OdinInspector;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ObjectEditor : MonoBehaviour
//{
//    public int cellId;
//    public int slotId;
//    public Objects obj;
//    public LevelEditor lvEditor;

//    [Button(ButtonSizes.Medium), GUIColor(1, 0.6f, 0.4f)]
//    public void RemoveObject()
//    {
//        BranchSlot slot = lvEditor.branchDatas[cellId].branchSlots[slotId];
//        slot.obj.type = TYPE.NONE;
//        lvEditor.branchCells[cellId].gameplayObjs.Clear();
//        DestroyImmediate(gameObject);
//    }

//    public void AssignScript()
//    {
//        BranchSlot slot = lvEditor.branchDatas[cellId].branchSlots[slotId];
//        slot.objScript = gameObject.GetComponent<Objects>();
//    }
//}
