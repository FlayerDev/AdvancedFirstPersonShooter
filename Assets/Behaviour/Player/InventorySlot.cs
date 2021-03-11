using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemType itemType;
    [Range(1,9)] public int subslots = 1;
}
 