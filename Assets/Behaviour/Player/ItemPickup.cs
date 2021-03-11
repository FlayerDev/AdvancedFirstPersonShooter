using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemPickup : NetworkBehaviour
{
    public ItemType itemType;
    public GameObject weaponPrefab;

    [Command]
    public void CmdPickup(Inventory inventory, bool overtake_slot = true)
    {
        InventorySlot slot = null;
        for (int i = 0; i < inventory.inventorySlots.Length; i++)
        {
            if (inventory[i].itemType == itemType) slot = inventory[i];
        }
        if (slot.subslots > slot.transform.childCount)
        {
            var wepbuff = Instantiate(weaponPrefab, slot.transform);
            wepbuff.CopyComponent(GetComponent<Mag>());
            NetworkServer.Spawn(wepbuff);
        }
        else if (overtake_slot)
        {
            slot.drop();
            var wepbuff = Instantiate(weaponPrefab, slot.transform);
            wepbuff.CopyComponent(GetComponent<Mag>());
            NetworkServer.Spawn(wepbuff);
        }
    }
}
