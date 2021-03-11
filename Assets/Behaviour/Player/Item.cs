﻿using UnityEngine;
using Mirror;

public class Item : MonoBehaviour
{
    public ItemType itemType;
    public GameObject pickupPrefab;


    [Command]
    public void CmdDrop()
    {
        var drop_item = Instantiate<GameObject>(pickupPrefab, transform.position, Quaternion.identity);
        drop_item.CopyComponent(GetComponent<Mag>());
        drop_item.GetComponent<ItemPickup>().itemType = itemType;
        NetworkServer.Spawn(drop_item);
    }
    /*
    public void pickup(Inventory inventory, bool overtake_slot = true)
    {
        Transform slot = inventory.transform.GetChild((int)itemType);
        if (slot.childCount < 1) gameObject.transform.parent = slot;
        else if (overtake_slot) { 
            slot.GetChild(0).GetComponent<Item>().drop();
            gameObject.transform.parent = slot;
        }
        objCollider.enabled = false;
        if (RigidBodyOnDrop) Destroy(gameObject.GetComponent<Rigidbody>());
        if (DisableOnDrop.Length > 0) foreach (MonoBehaviour obj in DisableOnDrop) obj.enabled = true;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
    }
    */
    //public void use(GameObject user) => pickup(user.GetComponent<Inventory>());
}
public enum ItemType
{
    Melee,
    Main,
    Secondary,
    Utility
}
