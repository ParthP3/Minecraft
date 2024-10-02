using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {
    public static float Get2DPerlin(Vector2 pos, float offset, float scale){
        return Mathf.PerlinNoise((pos.x + offset) / scale, (pos.y + offset) / scale);
    }
    public static float Get3DPerlin(Vector3 pos, float offset, float scale){
        return Mathf.PerlinNoise((pos.x + offset) / scale, (pos.y + offset) / scale + (pos.z + offset) / scale);
    }
    public static float Get2DPerlin(Vector2 pos, float offset, float scale, int octaves, float persistance, float lacunarity){
        float noiseValue = 0;
        float amplitude = 1;
        float frequency = 1;
        float maxValue = 0;
        for(int i = 0; i<octaves; i++){
            noiseValue += amplitude * Mathf.PerlinNoise((pos.x + offset) / scale * frequency, (pos.y + offset) / scale * frequency);
            maxValue += amplitude;
            amplitude *= persistance;
            frequency *= lacunarity;
        }
        return noiseValue/maxValue;
    }
    public static float Get3DPerlin(Vector3 pos, float offset, float scale, int octaves, float persistance, float lacunarity){
        float noiseValue = 0;
        float amplitude = 1;
        float frequency = 1;
        float maxValue = 0;
        for(int i = 0; i<octaves; i++){
            noiseValue += amplitude * Mathf.PerlinNoise((pos.x + offset) / scale * frequency, (pos.y + offset) / scale * frequency + (pos.z + offset) / scale * frequency);
            maxValue += amplitude;
            amplitude *= persistance;
            frequency *= lacunarity;
        }
        return noiseValue/maxValue;
    }
}
    