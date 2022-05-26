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

    //todo ��� ������� ������ ������ �������� (�����, ���, ����������, ��������, ������)
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
}
