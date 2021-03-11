using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemType weaponType;
    public GameObject weaponPrefab;

    public void pickup(Inventory inventory, bool overtake_slot = true)
    {
        Transform slot = inventory.transform.GetChild((int)weaponType);
        if (slot.childCount < 1) Instantiate(weaponPrefab,slot).CopyComponent(GetComponent<Mag>());
        else if (overtake_slot)
        {
            slot.GetChild(0).GetComponent<Item>().drop();
            Instantiate(weaponPrefab, slot).CopyComponent(GetComponent<Mag>());
        }
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
    }
}
