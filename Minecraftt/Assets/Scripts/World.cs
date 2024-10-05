using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World : MonoBehaviour{
    public int seed = 3453411;
    public int seedOffset;

    public BiomeAttributes biomeAttributes;
    public Transform playerTransform;
    public Vector3 spawnPosition;
    public Material material;
    public Material transparentMaterial;

    Chunk[][] chunks;
    List<ChunkCoord> activeChunks = new List<ChunkCoord>(); //Size of this list doesnt exceed RenderDistance*RenderDistance*4
    ChunkCoord playerLastChunkCoord;
    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();
    private bool  isCreatingChunks;

    public GameObject debugScreen;

    public void Start(){
        UnityEngine.Random.InitState(seed);
        seedOffset = UnityEngine.Random.Range(1,10000);
        playerTransform = GameObject.Find("Player").transform;
        playerLastChunkCoord = GetChunkCoordFromVector3(playerTransform.position);
        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks*VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight+0.2f, (VoxelData.WorldSizeInChunks*VoxelData.ChunkWidth) / 2f);
        biomeAttributes.AssignLodeValues();
        //toolbar.CreateToolbar();
        GenerateWorld();
        debugScreen.SetActive(false);
        
    }

    public void Update(){
        if(!GetChunkCoordFromVector3(playerTransform.position).Equals(playerLastChunkCoord)){
            CheckRenderDistance();
            playerLastChunkCoord = GetChunkCoordFromVector3(playerTransform.position);
        }

        if(!isCreatingChunks && chunksToCreate.Count > 0){
            StartCoroutine("CreateChunks");
        }

        if (Input.GetKeyDown(KeyCode.F3)){
            debugScreen.SetActive(!debugScreen.activeSelf);
        }
    }

    void GenerateWorld(){
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

    // IEnumerator allows the function to be paused and resumed at will
    IEnumerator CreateChunks(){
        isCreatingChunks = true;
        while(chunksToCreate.Count > 0){
            int popPosition = chunksToCreate.Count - 1;
            chunks[chunksToCreate[popPosition].x][chunksToCreate[popPosition].z].Init();
            chunksToCreate.RemoveAt(popPosition);
            yield return null;
        }

        isCreatingChunks = false;
    }



    void CheckRenderDistance(){
        ChunkCoord coord = GetChunkCoordFromVector3(playerTransform.position);
        for(int x = coord.x-PlayerSettings.RenderDistance; x<coord.x + PlayerSettings.RenderDistance; x++){
            for(int z = coord.z-PlayerSettings.RenderDistance; z<coord.z + PlayerSettings.RenderDistance; z++){
                if(IsChunkInWorld(new ChunkCoord(x,z))){
                    if(chunks[x][z] == null){
                        chunks[x][z] = new Chunk(new ChunkCoord(x,z), this, false);
                        chunksToCreate.Add(new ChunkCoord(x, z));
                    }
                    else if(!chunks[x][z].isActive){
                        chunks[x][z].isActive = true;
                        
                    }
                    activeChunks.Add(new ChunkCoord(x,z));
                }
            }
        }
        for(int i =0; i<activeChunks.Count; i++){
            if((Mathf.Abs(activeChunks[i].x - coord.x) > PlayerSettings.RenderDistance) || (Mathf.Abs(activeChunks[i].z - coord.z) > PlayerSettings.RenderDistance)){
                chunks[activeChunks[i].x][activeChunks[i].z].isActive = false;
                ///chunks[activeChunks[i].x][activeChunks[i].z] = null;
                activeChunks.RemoveAt(i);
                i--;
            }
        }

    }

    public bool CheckForVoxel(float _x, float _y, float _z){
        Vector3 pos = new Vector3(_x, _y, _z);
        ChunkCoord thisChunk = new ChunkCoord(pos);
        if(!IsVoxelInWorld(pos)){
            return false;
        }
        if(chunks[thisChunk.x][thisChunk.z] != null && chunks[thisChunk.x][thisChunk.z].isVoxelMapPopulated){
            return BlockTypes.blockTypes[chunks[thisChunk.x][thisChunk.z].GetVoxelFromGlobalPosition(pos)].isSolid;
        }

        return BlockTypes.blockTypes[GetBlock(pos)].isSolid;

    }

    public bool checkTransparent(float _x, float _y, float _z){
        Vector3 pos = new Vector3(_x, _y, _z);
        ChunkCoord thisChunk = new ChunkCoord(pos);
        if(!IsVoxelInWorld(pos)){
            return false;
        }
        if(chunks[thisChunk.x][thisChunk.z] != null && chunks[thisChunk.x][thisChunk.z].isVoxelMapPopulated){
            return BlockTypes.blockTypes[chunks[thisChunk.x][thisChunk.z].GetVoxelFromGlobalPosition(pos)].isTransparent;
        }

        return BlockTypes.blockTypes[GetBlock(pos)].isTransparent;
    }

    public byte GetBlock(Vector3 pos){
        int yPos = (int)(pos.y);
        //Immutable Pass for if the block is outside the world or at bedrock level
        if(!IsVoxelInWorld(pos)){
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

        // Second pass
        if(voxelValue == 3){
            for(int i = 0; i<biomeAttributes.lodes.Length; i++){
                if(yPos > biomeAttributes.lodes[i].minHeight && yPos < biomeAttributes.lodes[i].maxHeight){
                    if(Noise.Get3DPerlin(pos, biomeAttributes.lodes[i].noiseOffset, biomeAttributes.lodes[i].scale, biomeAttributes.lodes[i].threshold)){
                        voxelValue = biomeAttributes.lodes[i].blockID;
                    }
                }
            }
        }
        return voxelValue;
    }


    
    public bool IsChunkInWorld(ChunkCoord coord){
        return (coord.x >= 0 && coord.x < VoxelData.WorldSizeInChunks && coord.z >= 0 && coord.z < VoxelData.WorldSizeInChunks);
    }

    public bool IsVoxelInWorld(Vector3 pos){
        return (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels);
    }

    public Chunk GetChunkFromVector3(Vector3 pos){
        int x = (int)(pos.x/VoxelData.ChunkWidth);
        int z = (int)(pos.z/VoxelData.ChunkWidth);
        return chunks[x][z];
    }

    public ChunkCoord GetChunkCoordFromVector3(Vector3 pos){
        int x = (int)(pos.x/VoxelData.ChunkWidth);
        int z = (int)(pos.z/VoxelData.ChunkWidth);
        return new ChunkCoord(x,z);
    }


    
}




