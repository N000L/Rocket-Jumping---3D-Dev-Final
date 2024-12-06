using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//This is the script that allows you to make new weapons that are easy to edit
//The parameters are the ones shown below that you can tweek in the INSPECTOR!, 
//dont mess with the code here like setting a value in script or else it will break.


[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]
public class BaseGun : ScriptableObject
{
    public float EXPLOSION_FORCE;
    public float MAX_DISTANCE;
    public float RELOAD_TIME;
    public float PROJECTILE_SPEED;

    public GameObject WEAPON_MODEL;

    public GameObject PROJECTILE;

    public bool IS_HITSCAN;

}
