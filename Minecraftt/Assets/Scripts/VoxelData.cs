using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace minecraft{

public static class VoxelData
{
    // Texture atlas information
    public static readonly float x_textures = 64;
    public static readonly float y_textures = 32;

    // World Size Information
    public static readonly int ChunkWidth = 16;
    public static readonly int ChunkHeight = 128;
    public static readonly int WorldSizeInChunks = 50; //Equal to render distance for now for testing
    public static int WorldSizeInVoxels  {
        get {return WorldSizeInChunks * ChunkWidth;}
    }

    // Voxel Information
    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 1, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1),
        new Vector3(1, 1, 1),
        new Vector3(0, 1, 1)
    };

    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        new Vector3(0, 0, -1),
        new Vector3(0, 0, 1),
        new Vector3(0, 1, 0),
        new Vector3(0, -1, 0),
        new Vector3(-1, 0, 0),
        new Vector3(1, 0, 0)
    };
    public static readonly int[] TriangleVertexIndices = new int[6] {0, 1, 2, 2, 1, 3};
    public static readonly int[,] voxelTris = new int[6, 4] {
        {0, 3, 1, 2},
        {5, 6, 4, 7},
        {3, 7, 2, 6},
        {1, 5, 0, 4},
        {4, 7, 0, 3},
        {1, 2, 5, 6}
    } ;

    public static readonly Vector2[] voxelUvs = new Vector2[4]
    {
        new Vector2(0f/x_textures, 0f/y_textures),
        new Vector2(0f/x_textures, 1f/y_textures),
        new Vector2(1f/x_textures, 0f/y_textures),
        new Vector2(1f/x_textures, 1f/y_textures)
    };
}
}