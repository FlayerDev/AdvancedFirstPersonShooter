using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemPickup : NetworkBehaviour
{
    public ItemType itemType;
    public GameObject weaponPrefab;

    [Command]
    public void CmdPickup(GameObject slotObj, bool overtake_slot)
    {
        InventorySlot slot = slotObj.GetComponent<InventorySlot>();
        if (slot.subslots > slot.transform.childCount)
        {
            var wepbuff = Instantiate(weaponPrefab, slot.transform);
            wepbuff.CopyComponent(GetComponent<Mag>());
            NetworkServer.Spawn(wepbuff);
        }
        else if (overtake_slot)
        {
            //slot.drop();
            var wepbuff = Instantiate(weaponPrefab, slot.transform);
            wepbuff.CopyComponent(GetComponent<Mag>());
            NetworkServer.Spawn(wepbuff);
        }
    }
}
