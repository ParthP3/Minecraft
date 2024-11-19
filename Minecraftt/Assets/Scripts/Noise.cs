using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace minecraft{

public static class Noise {
    public static float Get2DPerlin(Vector2 pos, float offset, float scale){
        return Mathf.PerlinNoise((pos.x+0.1f)/VoxelData.ChunkWidth*scale+offset, (pos.y+0.1f)/VoxelData.ChunkWidth*scale+offset);
    }

    public static bool Get3DPerlin (Vector3 pos, float offset, float scale, float threshold){
        float x = (pos.x + offset + 0.1f) * scale;
        float y = (pos.y + offset + 0.1f) * scale;
        float z = (pos.z + offset + 0.1f) * scale;

        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zy = Mathf.PerlinNoise(z, y);
        float zx = Mathf.PerlinNoise(z, x);

        return (xy + yz + xz + yx + zy + zx)/6f > threshold;
    }
}
   
}
    