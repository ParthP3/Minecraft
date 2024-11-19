using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

namespace minecraft{

public class DebugScreen : MonoBehaviour
{
    World world;
    Text text;
    float frameRate;
    float timer; 
    float halfWorldSizeInVoxels = VoxelData.WorldSizeInVoxels / 2f;
    float bottomY = -64;


    void Start()
    {
        world = GameObject.Find("World").GetComponent<World>();
        text = GetComponent<Text>();

    }
    // Update is called once per frame
    void Update()
    {
        string debugText = "Debug Screen\n";
        float x_coord = world.playerTransform.position.x;
        float y_coord = world.playerTransform.position.y;
        float z_coord = world.playerTransform.position.z;
        float x_angle = world.playerTransform.eulerAngles.x;
        float y_angle = world.playerTransform.eulerAngles.y;
        float z_angle = world.playerTransform.eulerAngles.z;
        debugText += "Player Position: X " + (x_coord - halfWorldSizeInVoxels)+ ", Y " + (y_coord - bottomY)+ ", Z " + (z_coord - halfWorldSizeInVoxels)+ "\n";
        debugText += "Player Rotation: " + x_angle + ", " + y_angle + ", " + z_angle + "\n";
        debugText += "Chunk: " + HelperFunctions.GetChunkCoordFromGlobalPos(world.playerTransform.position).x+", "+ HelperFunctions.GetChunkCoordFromGlobalPos(world.playerTransform.position).z  + "\n";
        int facing;
        if(x_angle > 45 && x_angle < 135){
            facing = 0;
        }
        else if(x_angle < 315 && x_angle > 225){
            facing = 1;
        }
        else if(y_angle > 45 && y_angle < 135){
            facing = 2;
        }
        else if(y_angle < 315 && y_angle > 225){
            facing = 3;
        }
        else if(z_angle > 45 && z_angle < 135){
            facing = 4;
        }
        else if(z_angle < 315 && z_angle > 225){
            facing = 5;
        }
        else{
            facing = 0;
        }
        string facingString = "";
        switch(facing){
            case 0:
                facingString = "North";
                break;
            case 1:
                facingString = "South";
                break;
            case 2:
                facingString = "East";
                break;
            case 3:
                facingString = "West";
                break;
            case 4:
                facingString = "Up";
                break;
            case 5:
                facingString = "Down";
                break;
        }
        debugText += "Facing: " + facingString + "\n";
        debugText += "Render Distance: " + PlayerSettings.RenderDistance + "\n";
        if(timer > 1f){
            frameRate = (int)(1f/Time.unscaledDeltaTime);
            timer = 0;
        }
        else timer += Time.deltaTime;

        debugText += "FPS: " + frameRate + "\n";
        text.text = debugText;
    }
}

}