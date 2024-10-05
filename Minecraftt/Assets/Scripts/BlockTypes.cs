using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class BlockTypes{
    public static List <BlockType> blockTypes = new List<BlockType>(){
        new BlockType(0, "air", false, true, new int[6]{0,0,0,0,0,0}, 1),                                
        new BlockType(1, "dirt", true, false, new int [6]{1099, 1099, 1099, 1099, 1099, 1099}, 3),
        new BlockType(2, "grass", true, false, new int[6]{1109,1109,1045,1099,1109,1109}, 2),
        new BlockType(3, "cobblestone", true, false, new int[6]{1101, 1101, 1101, 1101, 1101, 1101}, 1),
        new BlockType(4, "bedrock", true, false, new int[6]{1995, 1995, 1995, 1995, 1995, 1995}, 7),
        new BlockType(5, "glass", true, true, new int[6]{1048, 1048, 1048, 1048, 1048, 1048}, 21),
    };
}

public class BlockType{
    public byte id;
    public string blockName;
    public bool isSolid;
    public bool isTransparent;
    public int spriteIndex;
    public int[] textureID = new int[6]; //left, right, top, bottom, back, front  respectively
    public BlockType(byte _id, string _blockName, bool _isSolid, bool _isTransparent, int[] textures, int _spriteIndex){
        id = _id;
        blockName = _blockName;
        isSolid = _isSolid;
        isTransparent = _isTransparent;
        textureID = textures;
        spriteIndex = _spriteIndex;
    }
}

public enum BlockEnum{
    air,
    dirt,
    grass,
    cobblestone,
    bedrock
}