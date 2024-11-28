using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

//Last updated 11/20 Nolan

public class PlayerController : MonoBehaviour
{

    /*
    To change player speed, change speed in the inspector.
    To change the camera, mess around the with the neck gameobject TRANSFORM and the camera gameobject transform.
    Do not change the look limits and the speed, those are for the camera staying in the right spot and not to clip 
    through the ground.

    */
    public List<BaseGun> weaponList;
    public float PLAYER_SPEED;
    public float JUMP_POWER;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask, layerMask;
    public float lookLimitX, lookLimitY, lookSpeed;
    public GameObject neck, cam, fpsMarker;
    public GameObject marker;

    
    private float MAX_DISTANCE, EXPLOSION_FORCE;
    private Vector3 movement = Vector3.zero;
    private Vector3 jumpMovement = Vector3.zero;
    private ExplosionScript explosionScript;
    private bool isGrounded;
    private CharacterController controller;
    private float rotationY;
    
    void Awake()
    {
        //disables cursor in game, press escape to get it back
        controller = GetComponent<CharacterController>();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        explosionScript = new ExplosionScript();
        explosionScript.character = controller;
        EXPLOSION_FORCE = weaponList[0].EXPLOSION_FORCE;
        MAX_DISTANCE = weaponList[0].MAX_DISTANCE;
    }


    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        MovePlayer();
        SpawnExplosion();
        explosionScript.ExplosionUpdate();
        ChangeCameraParent();
    }

    void MovePlayer()
    {
        //get movement vector. horizontal and vertical are things you can change in settings but it works with WASD
        //movement vector gets the hori and vert move vectors to get the full range of movment before being used
        movement.z = 0;
        movement.x = 0;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movement += transform.forward * vertical;
        movement += transform.right * horizontal;
    
        //checks if the player is actually moving
        if (movement.magnitude >= 0.1f)
        {
            //this is where the player actually moves
            controller.Move(movement.normalized * PLAYER_SPEED * Time.deltaTime);
        }

        Jump();
    }

    void MoveCamera()
    {
        //camera looking stuff, gets mouse axis and rotates the camera using said axis
        rotationY += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationY = Mathf.Clamp(rotationY, -lookLimitX, lookLimitY);
        float neckRotationY = Mathf.Clamp(rotationY, -lookLimitX, lookLimitY);
        //cam up down
        cam.transform.localRotation = Quaternion.Euler(rotationY, 0, 0);
        //neck up down
        neck.transform.localRotation = Quaternion.Euler(neckRotationY, 0, 0);
        //player left right
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    void Jump()
    {
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (!isGrounded)
        {
            jumpMovement.y +=  Time.deltaTime * Physics.gravity.y;
            
        }
        else
        {
            jumpMovement.y = -2;
        }

        if (Input.GetButton("Jump")  && isGrounded)
        {
            
            jumpMovement.y += Mathf.Sqrt(JUMP_POWER * -2f * Physics.gravity.y);
        }

        controller.Move(jumpMovement * Time.deltaTime);
    }

    void SpawnExplosion()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, layerMask))
            {
                Debug.Log("The ray hit at: "+hit.point);
                Debug.Log(hit.point - this.transform.position);
                Instantiate(marker, hit.point, Quaternion.identity);
                float dis = (hit.point - this.transform.position).magnitude;
                if (dis < MAX_DISTANCE)
                {
                    explosionScript.AddImpact(-(hit.point - this.transform.position), EXPLOSION_FORCE/(hit.point - this.transform.position).magnitude);
                }
                
            }

            //helps with player feel
            movement.y = 0;
        }
    }

    void ChangeCameraParent()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(cam.transform.parent);
            if (cam.transform.parent == fpsMarker.transform)
            {
                cam.transform.parent = neck.transform;
                cam.transform.position = neck.transform.position;
            }
            else
            {
                cam.transform.parent = fpsMarker.transform;
                cam.transform.position = fpsMarker.transform.position;
            }
        }
    }

}   
