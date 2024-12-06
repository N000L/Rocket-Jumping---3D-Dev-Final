using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    public PlayerControllerScript playerScript;

    void Awake()
    {
        playerScript = FindAnyObjectByType<PlayerControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.x) > 1000 || Mathf.Abs(transform.position.y) > 1000
            || Mathf.Abs(transform.position.z) > 1000)
        {
            Destroy(this);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            playerScript.GetExplosion(transform.position);
            Debug.Log("this should go away");
            GameObject.Destroy(this.gameObject);
        }
    }
}
