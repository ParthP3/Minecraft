using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

namespace minecraft{
    public class Game : MonoBehaviour{
        World world;
        Player player;
        public bool GeneratedSpawn = false;
        
        public void Start(){
            world = GameObject.Find("World").GetComponent<World>();
            player = GameObject.Find("Player").GetComponent<Player>();
            
        }

        public void Update(){
            if(!GeneratedSpawn){
                world.GenerateSpawn();
                GeneratedSpawn = true;
                Thread chunkCreationThread = new Thread(new ThreadStart(world.CreateNewChunks));
                chunkCreationThread.Start();
            }
            world.UpdateChunks();
            world.CreateStructures();
            world.RenderChunks();
            
            player.getInput();
            player.processInput();
            return;
        }
    }
}