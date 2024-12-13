using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

//Last updated 11/20 Nolan
//for weapon looking at mouse https://discussions.unity.com/t/rotating-an-object-to-face-the-mouse-location/390531/2

public class PlayerControllerScript : MonoBehaviour
{

    /*
    To change player speed, change speed in the inspector.
    To change the camera, mess around the with the neck gameobject TRANSFORM and the camera gameobject transform.
    Do not change the look limits and the speed, those are for the camera staying in the right spot and not to clip 
    through the ground.

    Also to add a weapon go into the inspector and drop the weapon object manually into Weapon List
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
    public Transform GunObjectLocation;

    
    private float maxDistance, explosionForce, reloadTime, projectileSpeed;
    private bool isHitScan;
    private Vector3 movement = Vector3.zero;
    private Vector3 jumpMovement = Vector3.zero;
    private ExplosionScript explosionScript;
    private bool isGrounded;
    private CharacterController controller;
    private float rotationY;
    private GameObject currentWeaponModel;
    private GameObject weaponProjectile;
    private bool canShoot = true;
    private int currentWeapon = 0;
    
    void Awake()
    {
        //disables cursor in game, press escape to get it back
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        explosionScript = new ExplosionScript();
        explosionScript.character = controller;
        InstantiateWeapon();
    }


    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        MovePlayer();
        explosionScript.ExplosionUpdate();
        ChangeCameraParent();
        ChangeWeapon();
        ShootWeapon();
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
        GunObjectLocation.localRotation = Quaternion.Euler(rotationY, 0, 0);
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
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, layerMask))
        {
            Debug.Log("The ray hit at: "+hit.point);
            Debug.Log(hit.point - this.transform.position);
            Instantiate(marker, hit.point, Quaternion.identity);
            float dis = (hit.point - this.transform.position).magnitude;
            if (dis < maxDistance)
            {
                if (isHitScan)
                {
                    explosionScript.AddImpact(-(hit.point - this.transform.position), explosionForce/(hit.point - this.transform.position).magnitude);
                }
                else
                {
                    GameObject bullet = Instantiate(weaponProjectile, GunObjectLocation.transform.position, Quaternion.identity);
                    Rigidbody rb = bullet.GetComponent<Rigidbody>();
                    rb.AddForce((hit.point-this.transform.position) * projectileSpeed, ForceMode.Force);
                }
            }
        }
        //helps with player feel
        movement.y = 0;
    }

    void ShootWeapon()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            SpawnExplosion();
            canShoot = false;
            StartCoroutine(ReloadTimer());
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

    void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentWeapon < weaponList.Count-1)
            {
                currentWeapon += 1;
            }
            else
            {
                currentWeapon = 0;
            }
            InstantiateWeapon();
        }
    }

    void InstantiateWeapon()
    {
        GameObject[] toDestroy = GameObject.FindGameObjectsWithTag("Weapon");
        foreach(GameObject go in toDestroy)
        Destroy(go);

        explosionForce = weaponList[currentWeapon].EXPLOSION_FORCE;
        maxDistance = weaponList[currentWeapon].MAX_DISTANCE;
        reloadTime = weaponList[currentWeapon].RELOAD_TIME;
        isHitScan = weaponList[currentWeapon].IS_HITSCAN;
        projectileSpeed = weaponList[currentWeapon].PROJECTILE_SPEED;
        weaponProjectile = weaponList[currentWeapon].PROJECTILE;
        currentWeaponModel = Instantiate(weaponList[currentWeapon].WEAPON_MODEL, transform.position, Quaternion.identity);
        currentWeaponModel.transform.position = GunObjectLocation.position;
        currentWeaponModel.transform.rotation = GunObjectLocation.rotation;
        currentWeaponModel.transform.parent = GunObjectLocation;
    }

    IEnumerator ReloadTimer()
    {
        yield return new WaitForSeconds(reloadTime);
        canShoot = true;
    }

    public void GetExplosion(Vector3 dir)
    {
        explosionScript.AddImpact(-(dir - this.transform.position),explosionForce/(dir - this.transform.position).magnitude);
    }

}   
