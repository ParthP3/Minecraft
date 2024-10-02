using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public ChunkCoord coord;
    public int ChunkWidth;
    public int ChunkHeight;
    public GameObject chunkObject;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public List <Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uvs = new List<Vector2>();
    public byte [,,] voxelMap;
    public World world;
    ChunkRenderer chunkRenderer;

    public Chunk(ChunkCoord _coord, World _world){
        world = _world;
        coord = _coord;
        ChunkWidth = VoxelData.ChunkWidth;
        ChunkHeight = VoxelData.ChunkHeight;
        voxelMap = new byte[ChunkWidth, ChunkHeight, ChunkWidth];
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(_coord.x * ChunkWidth, 0f, _coord.z * ChunkWidth);
        chunkObject.name = "Chunk " + _coord.x + ", " + _coord.z;

        PopulateVoxelMap();
        chunkRenderer = new ChunkRenderer(this);
        chunkRenderer.RenderChunk();

    }

    public bool isActive{
        get {return chunkObject.activeSelf;}
        set {chunkObject.SetActive(value);}
    }

    public Vector3 position{
        get {return chunkObject.transform.position;}
    }

    void PopulateVoxelMap(){
        for(int y = 0; y<ChunkHeight; y++){
            for(int x = 0; x<ChunkWidth; x++){
                for(int z = 0; z<ChunkWidth; z++){
                    voxelMap[x,y,z] = world.GetBlock(new Vector3(x,y,z) + position);
                }
            }
        }
    }
}

public class ChunkCoord{
    public int x;
    public int z;
    public ChunkCoord(int _x, int _z){
        x = _x;
        z = _z;
    }
    public bool Equals(ChunkCoord other){
        if(other == null){
            return false;
        }
        else if(other.x == x && other.z == z){
            return true;
        }
        else return false;
    }
}
