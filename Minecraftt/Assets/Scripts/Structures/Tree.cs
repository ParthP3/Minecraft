using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using minecraft;

namespace minecraft.structures{
public class Minecraft_Tree{
    public List<BlockInfo> MakeTree(Vector3 pos, int treeType, int seedOffSet, ref TreeTypeList treeTypeList){
        List<BlockInfo> treeBlocksList = new List<BlockInfo>();
        for(int i = 0; i<treeTypeList.treeTypeList[treeType].barkHeight; i++){
            BlockInfo blockInfo = new BlockInfo(treeTypeList.treeTypeList[treeType].barkBlock, pos+Vector3.up*i);
            treeBlocksList.Add(blockInfo);
        }
        for(int i = 0; i< treeTypeList.treeTypeList[treeType].leaves.Count; i++){
            BlockInfo blockInfo = new BlockInfo(treeTypeList.treeTypeList[treeType].leafBlock, pos+treeTypeList.treeTypeList[treeType].leaves[i]);
            treeBlocksList.Add(blockInfo);
        }
        return treeBlocksList;
    }
}

public class TreeTypeList{
    public List<TreeAttributes> treeTypeList = new List<TreeAttributes>{new TreeAttributes(0, 5, 4, 8, 6, 7)};
    public TreeTypeList(){
        treeTypeList[0].leafPattern = new int[4]{3,4,3,2};
        treeTypeList[0].leaves = new List<Vector3>();
        for(int i=0; i<treeTypeList[0].leavesEnd-treeTypeList[0].leavesStart; i++){
            for(int j=-treeTypeList[0].leafPattern[i];  j<=treeTypeList[0].leafPattern[i]; j++){
                for(int k=-treeTypeList[0].leafPattern[i];  k<=treeTypeList[0].leafPattern[i]; k++){
                    if(!(j==0 && k==0)) treeTypeList[0].leaves.Add(new Vector3(j,i+treeTypeList[0].leavesStart,k));
                }
            }
        }
    }
}

public class TreeAttributes{
    public int treeType;
    public int barkHeight;
    public int barkWidth;
    public int leavesStart;
    public int leavesEnd;
    public int[] leafPattern;
    public byte barkBlock;
    public byte leafBlock;
    public List<Vector3> leaves;

    public TreeAttributes(int _treeType, int _barkHeight, int _leavesStart, int _leavesEnd, byte _barkBlock = 6, byte _leafBlock = 7){
        treeType = _treeType;
        barkHeight = _barkHeight;
        leavesStart = _leavesStart;
        leavesEnd = _leavesEnd;
        barkBlock = _barkBlock;
        leafBlock = _leafBlock;
    }
}

public enum TreeEnum{
    oak,
    birch,
    acacia,
    spruce,
    cherry,
    jungle,
    dark_oak,
    mangrove,
    azalia,
    crimson,
}
}