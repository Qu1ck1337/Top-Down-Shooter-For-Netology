using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum WeaponType : byte
    {
        Pistol, 
        Rifle,
        Shotgun
    }

    //todo для каждого стейта делать действие (Стоим, идём, преследуем, стреляем, дерёмся)
    public enum EnemyStateType : byte
    {
        Idle,
        Patrolling,
        Pursuit,
        Shoot,
        Punch,
        PickUpWeapon,
        DropWeapon
    }

    public enum PlayerActionType : byte
    {
        PickUpWeapon,
        Shoot,
        DropWeapon,
        ReloadedWeapon
    }
}
