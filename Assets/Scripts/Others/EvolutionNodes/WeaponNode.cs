using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponNode", menuName = "Weapon/WeaponNode")]
public class WeaponNode : EvolutionNode
{
    public GameObject weaponPrefab;
    public List<WeaponNode> evolutions;

    public override GameObject GetPrefab()
    {
        return weaponPrefab;
    }
}
