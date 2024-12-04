using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//discussions.unity.com/t/force-on-character-controller-knockback/40743/3

//The script that is called to do the math for the explosion
//Sectioned off as its a little tricky to deal with

public class ExplosionScript
{
    float mass = 3.0F; // defines the character mass
    Vector3 impact = Vector3.zero;
    public CharacterController character;
    // Use this for initialization
    
    // Update is called once per frame
    public void ExplosionUpdate () 
    {
        // apply the impact force:
        if (impact.magnitude > 0.2F)
        {
            character.Move(impact * Time.deltaTime);
        }
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 10*Time.deltaTime);
    }

    // call this function to add an impact force:
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) 
        {
            dir.y = -dir.y; // reflect down force on the ground
        }
        impact += (dir.normalized * force / mass);
    }
    
}
