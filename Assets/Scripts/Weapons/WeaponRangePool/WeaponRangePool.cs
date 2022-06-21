using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponRangePool", menuName = "Configurations/WeaponRangePool")]
public class WeaponRangePool : ScriptableObject
{
    [SerializeField, Tooltip("Выбор оружия для врага")]
    private List<SimpleWeapon> _weapons = new List<SimpleWeapon>();
    public int Count => _weapons.Count;

    public SimpleWeapon GetRandomWeapon()
    {
        return _weapons[Random.Range(0, _weapons.Count)];
    }
}
