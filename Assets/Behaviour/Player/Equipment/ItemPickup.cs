using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[AddComponentMenu("Flayer/Inventory/Item Pickup")]
public class ItemPickup : NetworkBehaviour, IUsable
{
    public ItemType itemType;
    public GameObject weaponPrefab;
    public bool overtaking = true;

    public void use(Inventory userInventory)
    {
        userInventory.Pickup(gameObject, overtaking);
    }
    /*
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
*/ //for some reason inventory handles this
}
