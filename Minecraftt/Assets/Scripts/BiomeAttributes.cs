using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace minecraft{
[CreateAssetMenu(fileName="BiomeAttributes", menuName = "MinecraftData/BiomeAttribute")]
public class BiomeAttributes : ScriptableObject {

    public string biomeName;
    public int solidGroundHeight;
    public int maxHeightFromSolidGround;
    public float terrainScale;
    public Lode[] lodes;

    // Trees
    public float treeZoneScale = 1.3f;
    public float treeZoneThreshold = 0.6f;
    public float treePlacementScale = 15f;
    public float treePlacementThreshold = 0.8f;

    public int maxTreeHeight = 12;
    public int minTreeHeight = 5;

    public void AssignLodeValues(){
        lodes = new Lode[5];
        lodes[0] = new Lode("air", (byte)BlockEnum.air, 2, 255, 0.08f, 0.49f, 23);
        lodes[1] = new Lode("dirt", (byte)BlockEnum.dirt, 0, 255, 0.2f, 0.6f, 500);
        lodes[2] = new Lode("grass", (byte)BlockEnum.grass, solidGroundHeight, 255, 0.1f, 0.6f, 255);
        lodes[3] = new Lode("cobblestone", (byte)BlockEnum.cobblestone, 1, 50, 0.2f, 0.6f, 255);
        lodes[4] = new Lode("bedrock", (byte)BlockEnum.bedrock, 0, 2, 10, 0.1f, 255);
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
}