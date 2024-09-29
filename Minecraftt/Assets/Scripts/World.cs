using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour{
    public readonly BlockType[] blockTypes = new BlockType[256];
    public void Start(){
        AssignBlocks();
    }

    public void AssignBlocks(){
        blockTypes[0] = new BlockType("Air", false, new int[6]{0,0,0,0,0,0});
        blockTypes[1] = new BlockType("Dirt", true, new int [6]{1099, 1099, 1099, 1099, 1099, 1099});
        blockTypes[2] = new BlockType("Grass_Block", true, new int[6]{1109,1109,1045,1099,1109,1109});
        blockTypes[3] = new BlockType("Cobblestone", true, new int[6]{1101, 1101, 1101, 1101, 1101, 1101});
        blockTypes[4] = new BlockType("Bedrock", true, new int[6]{1995, 1995, 1995, 1995, 1995, 1995});
    }

    public void GenerateChunk(int x, int y, int z){
        Chunk newChunk = new GameObject().AddComponent<Chunk>();
        newChunk.transform.SetParent(this.transform);
        newChunk.transform.position = new Vector3(x, y, z);
        
    }

    public class BlockType{
        public string blockName;
        public bool isSolid;
        public int[] textureID = new int[6]; //left, right, top, bottom, back, front  respectively
        public BlockType(string _blockName, bool _isSolid, int[] textures){
            blockName = _blockName;
            isSolid = _isSolid;
            textureID = textures;
        }
    }
}
