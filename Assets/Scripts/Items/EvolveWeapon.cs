using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EvolveWeaponItem", menuName = "Inventory/Items/EvolveWeapon")]
public class EvolveWeapon : Item
{
    private GameObject player;
    private PlayerWeaponManager man;

    public override void Initialize(GameObject player)
    {
        this.player = player;
        man = this.player.GetComponent<PlayerWeaponManager>();
    }

    public override void Use()
    {
        man.ShowEvolutionChoices();
    }
}
