using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace minecraft{
public static class HelperFunctions{
    public static Vector2 GetTextureOnAtlas(int id){
        float x_textures = VoxelData.x_textures;
        float y_textures = VoxelData.y_textures;
        return new Vector2((float)((int)(id % x_textures) / (float)x_textures), Mathf.Floor(id / x_textures) / (float)y_textures);
    }

    public static ChunkCoord GetChunkCoordFromGlobalPos(Vector3 pos){
        int x = (int)(pos.x/VoxelData.ChunkWidth);
        int z = (int)(pos.z/VoxelData.ChunkWidth);
        return new ChunkCoord(x,z);
    }

    public static bool IsVoxelInChunk(int x, int y, int z){
        return !(x<0 || x>=VoxelData.ChunkWidth || y<0 || y>=VoxelData.ChunkHeight || z<0 || z>=VoxelData.ChunkWidth);
    }

    public static bool IsChunkInWorld(ChunkCoord coord){
        return (coord.x >= 0 && coord.x < VoxelData.WorldSizeInChunks && coord.z >= 0 && coord.z < VoxelData.WorldSizeInChunks);
    }

    public static bool IsVoxelInWorld(Vector3 pos){
        return (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels);
    }
}
}