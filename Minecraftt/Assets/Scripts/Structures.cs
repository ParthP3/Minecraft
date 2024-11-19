using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using minecraft.structures;

namespace minecraft{

public class Structures{
    World world;
    TreeTypeList treeTypeList;

    public Structures(World _world){
        world = _world;
        treeTypeList = new TreeTypeList();
    }

    public void PopulateChunks(ref List<BlockInfo> StructureBlockList){
        foreach(BlockInfo mod in StructureBlockList){
            ChunkCoord coord = HelperFunctions.GetChunkCoordFromGlobalPos(mod.pos);
            if(HelperFunctions.IsChunkInWorld(coord)){
                if(world.structureModifications.ContainsKey((coord.x, coord.z))){
                    world.structureModifications[(coord.x, coord.z)].Add(mod);
                }
                else{
                    world.structureModifications.Add((coord.x, coord.z), new List<BlockInfo>());
                    world.structureModifications[(coord.x, coord.z)].Add(mod);
                }
            }
        }
    }

    public void MakeTree(Vector3 pos, int treeType, int seedOffSet){
        List<BlockInfo> TreeList = new List<BlockInfo>();
        Minecraft_Tree tree = new Minecraft_Tree();
        TreeList = tree.MakeTree(pos, treeType, seedOffSet, ref treeTypeList);
        PopulateChunks(ref TreeList);
    }
}

public class BlockInfo{
    public byte BlockID;
    public Vector3 pos;
    public BlockInfo(){
        BlockID = 0;
        pos = new Vector3(0,0,0);
    }
    public BlockInfo(byte _blockID, Vector3 _coord){
        BlockID = _blockID;
        pos = _coord;
    }
}
}