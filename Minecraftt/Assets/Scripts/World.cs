using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using minecraft.structures;

namespace minecraft{

public class World : MonoBehaviour{
    public int seed = 3453411;
    public int seedOffset;

    public BiomeAttributes biomeAttributes;
    public Transform playerTransform;
    public Vector3 spawnPosition;
    public Material material;
    public Material transparentMaterial;
    public Structures structures;

    ChunkCoord playerLastChunkCoord;

    Chunk[][] chunks;
    List<ChunkCoord> activeChunks = new List<ChunkCoord>();

    public Dictionary<(int x, int z), List<BlockInfo>> structureModifications = new Dictionary<(int x, int z), List<BlockInfo>>();
    public Queue<ChunkCoord> chunksToCreate = new Queue<ChunkCoord>();
    public Queue<ChunkCoord> chunksToRender = new Queue<ChunkCoord>();

    
    public void Start(){
        UnityEngine.Random.InitState(seed);
        seedOffset = UnityEngine.Random.Range(1,10000);
        playerTransform = GameObject.Find("Player").transform;
        playerLastChunkCoord = HelperFunctions.GetChunkCoordFromGlobalPos(playerTransform.position);
        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks*VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight+0.2f, (VoxelData.WorldSizeInChunks*VoxelData.ChunkWidth) / 2f);
        biomeAttributes.AssignLodeValues();
        structures = new Structures(this);
    }

    public void CreateNewChunks(){
        while(true){
            if(chunksToCreate.Count > 0){
                ChunkCoord toCreate = chunksToCreate.Dequeue();
                chunks[toCreate.x][toCreate.z].PopulateVoxelMap();
                chunksToRender.Enqueue(toCreate);
            }
        }
    }

    public void RenderChunks(){
        if(chunksToRender.Count > 0){
            ChunkCoord toCreate = chunksToRender.Dequeue();
            chunks[toCreate.x][toCreate.z].RenderChunk();
        }
    }

    public void UpdateChunks(){
        if(!HelperFunctions.GetChunkCoordFromGlobalPos(playerTransform.position).Equals(playerLastChunkCoord)){
            ChunkCoord coord = HelperFunctions.GetChunkCoordFromGlobalPos(playerTransform.position);
            for(int x = coord.x-PlayerSettings.RenderDistance; x<coord.x + PlayerSettings.RenderDistance; x++){
                for(int z = coord.z-PlayerSettings.RenderDistance; z<coord.z + PlayerSettings.RenderDistance; z++){
                    if(HelperFunctions.IsChunkInWorld(new ChunkCoord(x,z))){
                        if(chunks[x][z] == null){
                            chunks[x][z] = new Chunk(new ChunkCoord(x, z), this, false);
                            chunksToCreate.Enqueue(new ChunkCoord(x, z));
                        }
                        else{
                            if(!chunks[x][z].isActive){
                                chunks[x][z].isActive = true;
                            }
                        }
                        activeChunks.Add(new ChunkCoord(x,z));
                    }
                }
            }
            for(int i =0; i<activeChunks.Count; i++){
                if((Mathf.Abs(activeChunks[i].x - coord.x) > PlayerSettings.RenderDistance) || (Mathf.Abs(activeChunks[i].z - coord.z) > PlayerSettings.RenderDistance)){
                    chunks[activeChunks[i].x][activeChunks[i].z].isActive = false;
                    activeChunks.RemoveAt(i);
                    i--;
                }
            }
            playerLastChunkCoord = HelperFunctions.GetChunkCoordFromGlobalPos(playerTransform.position);
        }
    }


    public void GenerateSpawn(){
        chunks = new Chunk[VoxelData.WorldSizeInChunks][];
        for(int i = 0; i<VoxelData.WorldSizeInChunks; i++){
            chunks[i] = new Chunk[VoxelData.WorldSizeInChunks];
        }
        for(int x = Math.Max(0, VoxelData.WorldSizeInChunks/2 - PlayerSettings.RenderDistance); x<Math.Min(VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks/2+PlayerSettings.RenderDistance); x++){
            for(int z = Math.Max(0, VoxelData.WorldSizeInChunks/2 - PlayerSettings.RenderDistance); z<Math.Min(VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks/2+PlayerSettings.RenderDistance); z++){
                chunks[x][z] = new Chunk(new ChunkCoord(x,z), this, true);
                activeChunks.Add(new ChunkCoord(x,z));
            }
        }
        playerTransform.position = spawnPosition;
    }

    public void CreateStructures(){
        ChunkCoord coord = HelperFunctions.GetChunkCoordFromGlobalPos(playerTransform.position);
        foreach(KeyValuePair<(int x, int z), List<BlockInfo>> Elem in structureModifications){
            if(chunks[Elem.Key.x][Elem.Key.z] != null && chunks[Elem.Key.x][Elem.Key.z].isVoxelMapPopulated){
                ChunkCoord strucCoord = new ChunkCoord(Elem.Key.x, Elem.Key.z);
                chunks[Elem.Key.x][Elem.Key.z].ModifyVoxelMap(Elem.Value);
                if(!(chunksToRender.Contains(strucCoord)) && (Mathf.Abs(strucCoord.x-coord.x) <= PlayerSettings.RenderDistance) && (Mathf.Abs(strucCoord.z-coord.z) <= PlayerSettings.RenderDistance))
                    chunksToRender.Enqueue(strucCoord);
                structureModifications.Remove(Elem.Key);
                 break;
            }
        }
    }

    public byte GetBlock(Vector3 pos){
        int yPos = (int)(pos.y);
        //Immutable Pass for if the block is outside the world or at bedrock level
        if(!HelperFunctions.IsVoxelInWorld(pos)){
            return 0;
        }
        if(pos.y==0){
            return 4;
        }
   
        // Basic terrain pass
        int terrainHeight = (int)(biomeAttributes.solidGroundHeight+Noise.Get2DPerlin(new Vector2(pos.x, pos.z), seedOffset, biomeAttributes.terrainScale)*biomeAttributes.maxHeightFromSolidGround);
        byte voxelValue = 0;
        if(yPos == terrainHeight){
            voxelValue = 2;
        }
        else if(yPos < terrainHeight && yPos > biomeAttributes.solidGroundHeight){
            voxelValue = 1;
        }
        else if(yPos <= biomeAttributes.solidGroundHeight){
            voxelValue = 3;
        }
        else{
            return 0;
        }

        // Second pass adds ores and caves
        if(voxelValue == 3){
            for(int i = 0; i<biomeAttributes.lodes.Length; i++){
                if(yPos > biomeAttributes.lodes[i].minHeight && yPos < biomeAttributes.lodes[i].maxHeight){
                    if(Noise.Get3DPerlin(pos, biomeAttributes.lodes[i].noiseOffset, biomeAttributes.lodes[i].scale, biomeAttributes.lodes[i].threshold)){
                        voxelValue = biomeAttributes.lodes[i].blockID;
                    }
                }
            }
        }

        // Third pass adds trees
        if(yPos == terrainHeight && (voxelValue == 2 || voxelValue == 1)){
            if(Noise.Get2DPerlin(new Vector2(pos.x, pos.z), seedOffset, biomeAttributes.treeZoneScale) > biomeAttributes.treeZoneThreshold){
                if(Noise.Get2DPerlin(new Vector2(pos.x, pos.z), seedOffset, biomeAttributes.treePlacementScale) > biomeAttributes.treePlacementThreshold){
                    structures.MakeTree(pos, (int)TreeEnum.oak, seedOffset);
                }
            }
        }
        return voxelValue;
    }

    public bool CheckForVoxel(float _x, float _y, float _z){
        Vector3 pos = new Vector3(_x, _y, _z);
        ChunkCoord thisChunk = new ChunkCoord(pos);
        if(!HelperFunctions.IsVoxelInWorld(pos)){
            return false;
        }
        if(chunks[thisChunk.x][thisChunk.z] != null && chunks[thisChunk.x][thisChunk.z].isVoxelMapPopulated){
            return BlockTypes.blockTypes[chunks[thisChunk.x][thisChunk.z].GetVoxelFromGlobalPosition(pos)].isSolid;
        }

        return true;

    }

    public bool checkTransparent(float _x, float _y, float _z){
        Vector3 pos = new Vector3(_x, _y, _z);
        ChunkCoord thisChunk = new ChunkCoord(pos);
        if(!HelperFunctions.IsVoxelInWorld(pos)){
            return true;
        }
        if(chunks[thisChunk.x][thisChunk.z] != null && chunks[thisChunk.x][thisChunk.z].isVoxelMapPopulated){
            return BlockTypes.blockTypes[chunks[thisChunk.x][thisChunk.z].GetVoxelFromGlobalPosition(pos)].isTransparent;
        }

        return true; // temporary, should probably be GetBlock(pos).isTransparent
    }

    public Chunk GetChunkFromVector3(Vector3 pos){
        int x = (int)(pos.x/VoxelData.ChunkWidth);
        int z = (int)(pos.z/VoxelData.ChunkWidth);
        return chunks[x][z];
    } 
}
}




