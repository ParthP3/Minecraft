using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="BiomeAttributes", menuName = "MinecraftData/BiomeAttribute")]
public class BiomeAttributes : ScriptableObject {

    public string biomeName;
    public int solidGroundHeight;
    public int maxHeightFromSolidGround;
    public float terrainScale;
    public Lode[] lodes;

    public void AssignLodeValues(){
        lodes = new Lode[5];
        lodes[0] = new Lode("Air", (byte)BlockEnum.Air, 2, 255, 0.1f, 0.53f, 23);
        lodes[1] = new Lode("Dirt", (byte)BlockEnum.Dirt, 0, 255, 0.2f, 0.6f, 500);
        lodes[2] = new Lode("Grass_Block", (byte)BlockEnum.Grass_Block, solidGroundHeight, 255, 0.1f, 0.6f, 255);
        lodes[3] = new Lode("Cobblestone", (byte)BlockEnum.Cobblestone, 1, 50, 0.2f, 0.6f, 255);
        lodes[4] = new Lode("Bedrock", (byte)BlockEnum.Bedrock, 0, 2, 10, 0.1f, 255);
    }

}

public class Lode {
    public string lodeName;
    public byte blockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;

    public Lode(string name, byte ID, int min, int max, float s, float t, float b){
        lodeName = name;
        blockID = ID;
        minHeight = min;
        maxHeight = max;
        scale = s;
        threshold = t;
        noiseOffset = b;
    }
}

public enum BiomeType {
    Desert,
    Forest,
    Plains,
    Snowy,
    Ocean
}