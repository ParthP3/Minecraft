using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace minecraft{

public class Toolbar : MonoBehaviour
{
    public RectTransform highlight;
    public ItemSlot[] itemSlots;
    public Sprite[] mySprites;
    public Player player;

    int slotIndex = 0;

    public void Start()
    {
        // temporarily setting random values in the toolbar
        itemSlots[0].itemID = 1;
        itemSlots[1].itemID = 2;
        itemSlots[2].itemID = 3;
        itemSlots[3].itemID = 4;
        itemSlots[4].itemID = 5;
        itemSlots[5].itemID = 4;
        itemSlots[6].itemID = 3;
        itemSlots[7].itemID = 2;
        itemSlots[8].itemID = 1;

        player = GameObject.Find("Player").GetComponent<Player>();
        mySprites = Resources.LoadAll <Sprite> ("Minecraft_icons"); 
        highlight = transform.Find("Highlight").GetComponent<RectTransform>();
        player.selectedBlockIndex = itemSlots[slotIndex].itemID;
        foreach(ItemSlot slot in itemSlots){
            slot.icon.sprite = mySprites[BlockTypes.blockTypes[slot.itemID].spriteIndex];
            slot.icon.enabled = true;
        }
    }

    public void Update(){
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll != 0){
            if(scroll > 0){
                slotIndex--;
            }else{
                slotIndex++;
            }
            if(slotIndex > itemSlots.Length - 1){
                slotIndex = 0;
            }
            if(slotIndex < 0){
                slotIndex = itemSlots.Length - 1;
            }
            player.selectedBlockIndex = itemSlots[slotIndex].itemID;
            highlight.position = itemSlots[slotIndex].icon.transform.position;
        }
        //number keys work too, sooooo
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            slotIndex = 0;
            player.selectedBlockIndex = itemSlots[slotIndex].itemID;
            highlight.position = itemSlots[slotIndex].icon.transform.position;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            slotIndex = 1;
            player.selectedBlockIndex = itemSlots[slotIndex].itemID;
            highlight.position = itemSlots[slotIndex].icon.transform.position;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            slotIndex = 2;
            player.selectedBlockIndex = itemSlots[slotIndex].itemID;
            highlight.position = itemSlots[slotIndex].icon.transform.position;
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)){
            slotIndex = 3;
            player.selectedBlockIndex = itemSlots[slotIndex].itemID;
            highlight.position = itemSlots[slotIndex].icon.transform.position;
        }
        if(Input.GetKeyDown(KeyCode.Alpha5)){
            slotIndex = 4;
            player.selectedBlockIndex = itemSlots[slotIndex].itemID;
            highlight.position = itemSlots[slotIndex].icon.transform.position;
        }
        if(Input.GetKeyDown(KeyCode.Alpha6)){
            slotIndex = 5;
            player.selectedBlockIndex = itemSlots[slotIndex].itemID;
            highlight.position = itemSlots[slotIndex].icon.transform.position;
        }
        if(Input.GetKeyDown(KeyCode.Alpha7)){
            slotIndex = 6;
            player.selectedBlockIndex = itemSlots[slotIndex].itemID;
            highlight.position = itemSlots[slotIndex].icon.transform.position;
        }
        if(Input.GetKeyDown(KeyCode.Alpha8)){
            slotIndex = 7;
            player.selectedBlockIndex = itemSlots[slotIndex].itemID;
            highlight.position = itemSlots[slotIndex].icon.transform.position;
        }
        if(Input.GetKeyDown(KeyCode.Alpha9)){
            slotIndex = 8;
            player.selectedBlockIndex = itemSlots[slotIndex].itemID;
            highlight.position = itemSlots[slotIndex].icon.transform.position;
        }
    }

}

[System.Serializable]
public class ItemSlot{
    public byte itemID;
    public Image icon;
}

}
