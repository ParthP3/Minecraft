using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour{
    public static readonly BlockType[] blockTypes = new BlockType[256];
    public void Start(){
        blockTypes[0] = new BlockType("Air", false);
        blockTypes[1] = new BlockType("Grass_Block", true);
        blockTypes[1].textureID = new int[6]{1173,1173,1173,1173,1163,1109};
    }

    public class BlockType{
        public string blockName;
        public bool isSolid;
        public int[] textureID = new int[6]; //back, front,  right, left,bottom, top  respectively
        public BlockType(string _blockName, bool _isSolid){
            blockName = _blockName;
            isSolid = _isSolid;
        }
    }
}
