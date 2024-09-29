using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    int vertexIndex = 0;
    List <Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    byte [,,] voxelMap = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];
    World world;

    void Start(){
        world = GameObject.Find("World").GetComponent<World>();
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();

    }

    void PopulateVoxelMap(){
        for(int y = 0; y<VoxelData.chunkHeight; y++){
            for(int x = 0; x<VoxelData.chunkWidth; x++){
                for(int z = 0; z<VoxelData.chunkWidth; z++){
                    voxelMap[x,y,z] = 1;
                }
            }
        }
    }


    void CreateMeshData(){
        for(int y = 0; y<VoxelData.chunkHeight; y++){
            for(int x = 0; x<VoxelData.chunkWidth; x++){
                for(int z = 0; z<VoxelData.chunkWidth; z++){
                    AddVoxelDataToChunk(new Vector3(x,y,z), voxelMap[x,y,z]);
                }
            }
        }
    }


    bool CheckVoxel(Vector3 pos){
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);
        if(x<0 || x>=VoxelData.chunkWidth || y<0 || y>=VoxelData.chunkHeight || z<0 || z>=VoxelData.chunkWidth){
            return false;
        }
        return world.blockTypes[voxelMap[x,y,z]].isSolid;
    }


    void AddVoxelDataToChunk(Vector3 chunkPosition, int blockID){
        //For each face of the voxel
        for(int i = 0; i<6; i++){
            if(!CheckVoxel(chunkPosition + VoxelData.faceChecks[i])){
                for(int j=0; j<6; j++){
                    vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[i, VoxelData.TriangleVertexIndices[j]]]+chunkPosition);
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
        return new Vector2((float)(id % x_textures) / (float)x_textures, Mathf.Floor(id / x_textures) / (float)y_textures);
    }
}
