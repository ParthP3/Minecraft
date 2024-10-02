using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World : MonoBehaviour{
    public int seed = 11;
    public int seedOffset;

    public Transform player;
    public Vector3 spawnPosition;
    public Material material;
    public readonly BlockType[] blockTypes = new BlockType[256];
    Chunk[][] chunks;
    List<ChunkCoord> activeChunks = new List<ChunkCoord>(); //Size of this list doesnt exceed RenderDistance*RenderDistance*4
    ChunkCoord playerLastChunkCoord;
    public void Start(){
        UnityEngine.Random.InitState(seed);
        seedOffset = UnityEngine.Random.Range(1,10000);
        //setting player to main camera's transform
        player = Camera.main.transform;
        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks*VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight+2f, (VoxelData.WorldSizeInChunks*VoxelData.ChunkWidth) / 2f);
        CreateBlockTypeData();
        GenerateWorld();
        
    }

    public void Update(){
        if(!GetChunkCoordFromVector3(player.position).Equals(playerLastChunkCoord)){
            CheckRenderDistance();
            playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
        }
    }

    public void CreateBlockTypeData(){
        blockTypes[0] = new BlockType("Air", false, new int[6]{0,0,0,0,0,0});
        blockTypes[1] = new BlockType("Dirt", true, new int [6]{1099, 1099, 1099, 1099, 1099, 1099});
        blockTypes[2] = new BlockType("Grass_Block", true, new int[6]{1109,1109,1045,1099,1109,1109});
        blockTypes[3] = new BlockType("Cobblestone", true, new int[6]{1101, 1101, 1101, 1101, 1101, 1101});
        blockTypes[4] = new BlockType("Bedrock", true, new int[6]{1995, 1995, 1995, 1995, 1995, 1995});
    }

    void GenerateWorld(){
        chunks = new Chunk[VoxelData.WorldSizeInChunks][];
        for(int i = 0; i<VoxelData.WorldSizeInChunks; i++){
            chunks[i] = new Chunk[VoxelData.WorldSizeInChunks];
        }
        for(int x = Math.Max(0, VoxelData.WorldSizeInChunks/2 - PlayerSettings.RenderDistance); x<Math.Min(VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks/2+PlayerSettings.RenderDistance); x++){
            for(int z = Math.Max(0, VoxelData.WorldSizeInChunks/2 - PlayerSettings.RenderDistance); z<Math.Min(VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks/2+PlayerSettings.RenderDistance); z++){
                CreateNewChunk(x,z);
            }
        }
        player.position = spawnPosition;
    }

    ChunkCoord GetChunkCoordFromVector3(Vector3 pos){
        int x = Mathf.FloorToInt(pos.x/VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z/VoxelData.ChunkWidth);
        return new ChunkCoord(x,z);
    }

    void CheckRenderDistance(){
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);
        for(int x = coord.x-PlayerSettings.RenderDistance; x<coord.x + PlayerSettings.RenderDistance; x++){
            for(int z = coord.z-PlayerSettings.RenderDistance; z<coord.z + PlayerSettings.RenderDistance; z++){
                if(IsChunkInWorld(new ChunkCoord(x,z))){
                    if(chunks[x][z] == null){
                        CreateNewChunk(x,z);
                    }
                    else if(!chunks[x][z].isActive){
                        chunks[x][z].isActive = true;
                        activeChunks.Add(new ChunkCoord(x,z));
                    }
                }
            }
        }
        for(int i =0; i<activeChunks.Count; i++){
            if((Mathf.Abs(activeChunks[i].x - coord.x) > PlayerSettings.RenderDistance) || (Mathf.Abs(activeChunks[i].z - coord.z) > PlayerSettings.RenderDistance)){
                chunks[activeChunks[i].x][activeChunks[i].z].isActive = false;
                chunks[activeChunks[i].x][activeChunks[i].z] = null;
                activeChunks.RemoveAt(i);
                i--;
            }
        }

    }

    public byte GetBlock(Vector3 pos){
        int yPos = Mathf.FloorToInt(pos.y);
        //Immutable Pass for if the block is outside the world or at bedrock level
        if(!IsVoxelInWorld(pos)){
            return 0;
        }
        if(pos.y==0){
            return 4;
        }
        // Basic terrain pass
        int terrainHeight = Mathf.FloorToInt(Noise.Get2DPerlin(new Vector2(pos.x, pos.z), seedOffset, 0.5f)*VoxelData.ChunkHeight);
        if(yPos <= terrainHeight){
            return 3;
        }
        else{
            return 0;
        }
    }

    void CreateNewChunk(int x, int z){
        chunks[x][z] = new Chunk(new ChunkCoord(x,z), this);
        activeChunks.Add(new ChunkCoord(x,z));
    }
    
    bool IsChunkInWorld(ChunkCoord coord){
        return (coord.x >= 0 && coord.x < VoxelData.WorldSizeInChunks && coord.z >= 0 && coord.z < VoxelData.WorldSizeInChunks);
    }

    bool IsVoxelInWorld(Vector3 pos){
        return (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels);
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
