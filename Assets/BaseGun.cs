using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]
public class BaseGun : ScriptableObject
{
    public float EXPLOSION_FORCE;
    public float MAX_DISTANCE;
}
