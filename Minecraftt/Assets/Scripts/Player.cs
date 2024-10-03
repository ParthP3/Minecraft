using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float mouseX;
    public float mouseY;
    public Vector3 velocity;

    public Transform playerCam;
    private World world;


    public Transform playerHand;
    public Transform highlightBlock;
    public float interactRange;
    public float checkIncrement;
    public float checkIncrementSide;
    public float maxAngle;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public float jumpForce;
    public float playerSpeed;
    public float gravity;
    public float verticalMomentum;
    public bool jumpRequest;
    public bool isGrounded;
    public bool isSprinting;
    public bool keepSprint;
    public bool isCrouched;

    public float sensitivity;
    public float sprintSpeed;
    public float normalSpeed;
    public float crouchSpeed;
    public float playerWidth;
    public float playerHeight;
    public float playerReach;
    public float playerReachSide;
    public float playerHeightCrouch;
    public float playerHeightNormal;
    public float playerHeightSprint;
    public float playerHeightJump;
    public float playerHeightFall;
    public float playerHeightStand;
    public float playerHeightClimb;
    public float climbSpeed;
    public float climbJumpForce;
    public float climbFallMultiplier;
    public float climbLowJumpMultiplier;
    public float climbGravity;

    public void SetValues(){
        horizontal = 0;
        vertical = 0;
        mouseX = 0;
        mouseY = 0;
        velocity = Vector3.zero;
        playerSpeed = 5;
        gravity = -5.8f;
        verticalMomentum = 0;
        jumpRequest = false;
        isGrounded = false;
        isSprinting = false;
        keepSprint = false;
        isCrouched = false;

        sensitivity = 2;
        sprintSpeed = 10;
        normalSpeed = 5;
        crouchSpeed = 2;
        jumpForce = 4.3f;
        fallMultiplier = 2.5f;
        lowJumpMultiplier = 2f;
        interactRange = 8;
        checkIncrement = 0.1f;
        checkIncrementSide = 0.1f;
        maxAngle = 50;
        playerWidth = 0.3f;
        playerHeight = 1.8f;
        playerReach = 3;
        playerReachSide = 2;
        playerHeightCrouch = 1.5f;
        playerHeightNormal = 2;
        playerHeightSprint = 2.5f;
        playerHeightJump = 2.5f;
        playerHeightFall = 2.5f;
        playerHeightStand = 2;
        playerHeightClimb = 2;
        climbSpeed = 2;
        climbJumpForce = 5;
        climbFallMultiplier = 2.5f;
        climbLowJumpMultiplier = 2f;
        climbGravity = 9.8f;
    }
    
    private void Start(){
        SetValues();
        world = GameObject.Find("World").GetComponent<World>();
        playerCam = GameObject.Find("Main Camera").transform;
    }

    private void Update(){
        GetPlayerInput();
        CalculateVelocity();
        transform.Rotate(Vector3.up * mouseX * sensitivity);
        playerCam.Rotate(Vector3.right * -mouseY * sensitivity);
        transform.Translate(velocity, Space.World);
        translatePlayerCamToPlayerHeight();
        
    }

    private void GetPlayerInput(){
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical"); //Vertical not to be confused with up and down. vertical is in z axis, up and down is in y axis
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        if(Input.GetButtonDown("Fire3")){
            isCrouched = true;
            isSprinting = false;
            jumpRequest = false;
        }
        if(Input.GetButtonUp("Fire3")){
            isCrouched = false;
        }

        if(!isCrouched){ // Player can only sprint or jump when not crouched
            if(isGrounded && Input.GetButtonDown("Jump")){
                jumpRequest = true;
            }

            if(Input.GetButtonDown("Fire1")){
                isSprinting = true;
            }
            if(!keepSprint){
                if(Input.GetButtonUp("Fire1")){
                    isSprinting = false;
                }
            }
            else{
                // stop sprinting only if player stops moving forward
                if(vertical == 0){
                    isSprinting = false;
                }
            }
        }
        
    }

    private void CalculateVelocity(){
        if(verticalMomentum > gravity){
            verticalMomentum += Time.fixedDeltaTime * gravity;
        }

        if(isCrouched){
            playerHeight = playerHeightCrouch;
        }
        if(!isCrouched){
            playerHeight = playerHeightNormal;
        }

        if(isSprinting){
            velocity = (transform.forward * vertical + transform.right * horizontal) * Time.deltaTime*sprintSpeed;
        }
        else{
            velocity = (transform.forward * vertical + transform.right * horizontal) * Time.deltaTime*playerSpeed;
        }
        if(jumpRequest){
            verticalMomentum = jumpForce;
            jumpRequest = false;
        }

        // Up and Down Momentum
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        
        if(velocity.y < 0){
            velocity.y = checkDownSpeed(velocity.y);
        }
        else if(velocity.y > 0){
            velocity.y = checkUpSpeed(velocity.y);
        }
        if(velocity.z > 0 ){
            velocity.z = checkFrontSpeed(velocity.z);
            if(isCrouched){
                velocity.z = checkFrontFall(velocity.z);
            }
        }
        else if(velocity.z < 0){
            velocity.z = checkBackSpeed(velocity.z);
            if(isCrouched){
                velocity.z = checkBackFall(velocity.z);
            }
        }
        if(velocity.x > 0){
            velocity.x = checkRightSpeed(velocity.x);
            if(isCrouched){
                velocity.x = checkRightFall(velocity.x);
            }
        }
        else if(velocity.x < 0){
            velocity.x = checkLeftSpeed(velocity.x);
            if(isCrouched){
                velocity.x = checkLeftFall(velocity.x);
            }
        }
    }

    private void translatePlayerCamToPlayerHeight(){ // Make the camera movement on height change not too sudden
        if(playerCam.position.y < transform.position.y + playerHeight){
            playerCam.position = new Vector3(playerCam.position.x, playerCam.position.y + 0.1f, playerCam.position.z);
        }
        if(playerCam.position.y > transform.position.y + playerHeight){
            playerCam.position = new Vector3(playerCam.position.x, playerCam.position.y - 0.1f, playerCam.position.z);
        }
    }

    private float checkDownSpeed (float downSpeed){
        if ((world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth) && 
            !world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + downSpeed + 1, transform.position.z - playerWidth)) ||
            (world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth) &&
            !world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + downSpeed + 1, transform.position.z - playerWidth) ) ||
            (world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth) &&
            !world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + downSpeed + 1, transform.position.z + playerWidth)) ||
            (world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth) &&
            !world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + downSpeed + 1, transform.position.z + playerWidth))){
            isGrounded = true;
            return 0;
        }
        else{
            isGrounded = false;
            return downSpeed;
        }
    }

    private float checkUpSpeed (float upSpeed){
        if (world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + playerHeight + 0.2f + upSpeed, transform.position.z - playerWidth) ||
            world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + playerHeight + 0.2f + upSpeed, transform.position.z - playerWidth) ||
            world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + playerHeight + 0.2f + upSpeed, transform.position.z + playerWidth) ||
            world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + playerHeight + 0.2f + upSpeed, transform.position.z + playerWidth)){
            return 0;
        }
        else{
            isGrounded = false;
            return upSpeed;
        }
    }

    private float checkFrontSpeed(float frontSpeed){
        if (world.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z + playerWidth + frontSpeed) ||
            world.CheckForVoxel(transform.position.x, transform.position.y + 1, transform.position.z + playerWidth + frontSpeed)){
            return 0;
        }
        else{
            return frontSpeed;
        }
    }

    private float checkBackSpeed(float backSpeed){
        if (world.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z - playerWidth + backSpeed) ||
            world.CheckForVoxel(transform.position.x, transform.position.y + 1, transform.position.z - playerWidth + backSpeed)){
            return 0;
        }
        else{
            return backSpeed;
        }
    }

    private float checkLeftSpeed(float leftSpeed){
        if (world.CheckForVoxel(transform.position.x - playerWidth + leftSpeed, transform.position.y, transform.position.z) ||
            world.CheckForVoxel(transform.position.x - playerWidth + leftSpeed, transform.position.y + 1, transform.position.z)){
            return 0;
        }
        else{
            return leftSpeed;
        }
    }

    private float checkRightSpeed(float rightSpeed){
        if (world.CheckForVoxel(transform.position.x + playerWidth + rightSpeed, transform.position.y, transform.position.z) ||
            world.CheckForVoxel(transform.position.x + playerWidth + rightSpeed, transform.position.y + 1, transform.position.z)){
            return 0;
        }
        else{
            return rightSpeed;
        }
    }

    private float checkFrontFall(float frontSpeed){ // Check if the block beneath the player would be solid if the player moves forward
        if (world.CheckForVoxel(transform.position.x, transform.position.y - 1, transform.position.z + playerWidth + frontSpeed)){
            return frontSpeed;
        }
        else{
            return 0;
        }
    }

    private float checkBackFall(float backSpeed){
        if (world.CheckForVoxel(transform.position.x, transform.position.y - 1, transform.position.z - playerWidth + backSpeed)){
            return backSpeed;
        }
        else{
            return 0;
        }
    }

    private float checkLeftFall(float leftSpeed){
        if (world.CheckForVoxel(transform.position.x - playerWidth + leftSpeed, transform.position.y - 1, transform.position.z)){
            return leftSpeed;
        }
        else{
            return 0;
        }
    }

    private float checkRightFall(float rightSpeed){
        if (world.CheckForVoxel(transform.position.x + playerWidth + rightSpeed, transform.position.y - 1, transform.position.z)){
            return rightSpeed;
        }
        else{
            return 0;
        }
    }


}

