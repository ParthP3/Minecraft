using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRenderer
{
    Chunk chunk;
    int vertexIndex = 0;
    public ChunkRenderer(Chunk _chunk){
        chunk = _chunk;
    }

    public void RenderChunk(){
        CreateMeshData();
        CreateMesh();
    }

    void CreateMeshData(){
        for(int y = 0; y<chunk.ChunkHeight; y++){
            for(int x = 0; x<chunk.ChunkWidth; x++){
                for(int z = 0; z<chunk.ChunkWidth; z++){
                    AddVoxelDataToChunk(new Vector3(x,y,z), chunk.voxelMap[x,y,z]);
                }
            }
        }
    }

    bool IsVoxelInChunk(int x, int y, int z){
        return !(x<0 || x>=chunk.ChunkWidth || y<0 || y>=chunk.ChunkHeight || z<0 || z>=chunk.ChunkWidth);
    }
    

    bool CheckVoxel(Vector3 pos){
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);
        if(!IsVoxelInChunk(x,y,z)){return !chunk.world.blockTypes[chunk.world.GetBlock(pos+chunk.position)].isSolid;}
        return !chunk.world.blockTypes[chunk.voxelMap[x,y,z]].isSolid;
    }

    void AddVoxelDataToChunk(Vector3 blockPosition, int blockID){
        //For each face of the block
        for(int i = 0; i<6; i++){
            if(CheckVoxel(blockPosition + VoxelData.faceChecks[i])){
                // For each vertex of the face 
                for(int j=0; j<6; j++){
                    chunk.vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[i, VoxelData.TriangleVertexIndices[j]]]+blockPosition);
                    chunk.triangles.Add(vertexIndex);
                    vertexIndex++;
                }
                int textureGridIndex = chunk.world.blockTypes[blockID].textureID[i];
                Vector2 texturePosition = GetTextureOnAtlas(textureGridIndex);
                //For each vertex of the face
                for(int j=0; j<6; j++){
                    chunk.uvs.Add(texturePosition + VoxelData.voxelUvs[VoxelData.TriangleVertexIndices[j]]);
                }         
            }            
        }
    }

    void CreateMesh(){
        Mesh mesh = new Mesh ();
        mesh.vertices = chunk.vertices.ToArray();
        mesh.triangles = chunk.triangles.ToArray();
        mesh.uv = chunk.uvs.ToArray();
        mesh.RecalculateNormals();
        chunk.meshFilter.mesh = mesh;
    }

    Vector2 GetTextureOnAtlas(int id){
        float x_textures = VoxelData.x_textures;
        float y_textures = VoxelData.y_textures;
        return new Vector2((float)((int)(id % x_textures) / (float)x_textures), Mathf.Floor(id / x_textures) / (float)y_textures);
    }
}