using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum WeaponType : byte
    {
        Pistol, 
        Rifle,
        Shootgun
    }

    public enum EnemyType : byte
    {
        Weakling,
        Fat
    }

    public enum EnemyStateType : byte
    {
        Idle,
        Patrolling,
        Pursuit
    }
}
