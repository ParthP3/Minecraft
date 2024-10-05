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
    int vertexIndex = 0;
    public byte [,,] voxelMap;
    public World world;
    private bool _isActive;
    public bool isVoxelMapPopulated = false;

    public Chunk(ChunkCoord _coord, World _world, bool generateOnLoad){
        world = _world;
        coord = _coord;
        isActive = true;
        ChunkWidth = VoxelData.ChunkWidth;
        ChunkHeight = VoxelData.ChunkHeight;
        voxelMap = new byte[ChunkWidth, ChunkHeight, ChunkWidth];
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * ChunkWidth, 0f, coord.z * ChunkWidth);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

        if (generateOnLoad){
            Init();
        }
        
    }

    public void Init(){
        PopulateVoxelMap();
        RenderChunk();
    }

    public bool isActive{
        get {return _isActive;}
        set {
            _isActive = value;
            if(chunkObject != null){
                chunkObject.SetActive(value);
            }
        }
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
        isVoxelMapPopulated = true;
    }

    public void RenderChunk(){
        UpdateChunk();
    }

    void UpdateChunk(){
        ClearMeshData();
        for(int y = 0; y<ChunkHeight; y++){
            for(int x = 0; x<ChunkWidth; x++){
                for(int z = 0; z<ChunkWidth; z++){
                    if(world.blockTypes[voxelMap[x,y,z]].isSolid)
                        UpdateMeshData(new Vector3(x,y,z), voxelMap[x,y,z]);
                }
            }
        }
        CreateMesh();
    }

    void ClearMeshData(){
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        vertexIndex = 0;
    }

    public void EditVoxel(Vector3 pos, byte newID){
        int xCheck = (int)(pos.x);
        int yCheck = (int)(pos.y);
        int zCheck = (int)(pos.z);
        xCheck -= (int)(chunkObject.transform.position.x);
        zCheck -= (int)(chunkObject.transform.position.z);
        voxelMap[xCheck, yCheck, zCheck] = newID;
        
        UpdateChunk();
        UpdateSurroundingVoxels(xCheck, yCheck, zCheck);

    }

    void UpdateSurroundingVoxels(int x, int y, int z){
        Vector3 thisVoxel = new Vector3(x,y,z);
        for(int p = 0; p<6; p++){
            Vector3 currentVoxel = thisVoxel + VoxelData.faceChecks[p];
            if(!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z)){
                world.GetChunkFromVector3(currentVoxel + position).UpdateChunk();
            }
        }
    }

    bool IsVoxelInChunk(int x, int y, int z){
        return !(x<0 || x>=ChunkWidth || y<0 || y>=ChunkHeight || z<0 || z>=ChunkWidth);
    }
    

    public bool CheckVoxel(Vector3 pos){
        int x = (int)(pos.x);
        int y = (int)(pos.y);
        int z = (int)(pos.z);
        if(!IsVoxelInChunk(x,y,z)){return !world.CheckForVoxel(pos.x + position.x, pos.y+position.y, pos.z + position.z);}
        return !world.blockTypes[voxelMap[x,y,z]].isSolid;
    }

    

    void UpdateMeshData(Vector3 blockPosition, int blockID){
        //For each face of the block
        for(int i = 0; i<6; i++){
            if(CheckVoxel(blockPosition + VoxelData.faceChecks[i])){
                // For each vertex of the face 
                for(int j=0; j<6; j++){
                    vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[i, VoxelData.TriangleVertexIndices[j]]]+blockPosition);
                    triangles.Add(vertexIndex);
                    vertexIndex++;
                }
                int textureGridIndex = world.blockTypes[blockID].textureID[i];
                Vector2 texturePosition = GetTextureOnAtlas(textureGridIndex);
                //For each vertex of the face
                for(int j=0; j<6; j++){
                    uvs.Add(texturePosition + VoxelData.voxelUvs[VoxelData.TriangleVertexIndices[j]]);
                }         
            }            
        }
    }

    void CreateMesh(){
        Mesh mesh = new Mesh ();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    Vector2 GetTextureOnAtlas(int id){
        float x_textures = VoxelData.x_textures;
        float y_textures = VoxelData.y_textures;
        return new Vector2((float)((int)(id % x_textures) / (float)x_textures), Mathf.Floor(id / x_textures) / (float)y_textures);
    }

    public byte GetVoxelFromGlobalPosition( Vector3 pos){
        int xCheck = (int)(pos.x);
        int yCheck = (int)(pos.y);
        int zCheck = (int)(pos.z);
        xCheck -= (int)(chunkObject.transform.position.x);
        zCheck -= (int)(chunkObject.transform.position.z);

        return voxelMap[xCheck, yCheck, zCheck];
    }
}

public class ChunkCoord{
    public int x;
    public int z;

    public ChunkCoord(){
        x = 0;
        z = 0;
    }
    public ChunkCoord(int _x, int _z){
        x = _x;
        z = _z;
    }
    public ChunkCoord(Vector3 pos){
        int xCheck = (int)(pos.x);
        int zCheck = (int)(pos.z);
        x = xCheck / VoxelData.ChunkWidth;
        z = zCheck / VoxelData.ChunkWidth;
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
