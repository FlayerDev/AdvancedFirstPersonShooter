using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class InventorySlot : MonoBehaviour
{
    public ItemType itemType;
    public bool AllowDrop = true;
    [Range(1,9)] public int subslots = 1;

    public int activeSubSlot = 0;

    public GameObject this[int index]
    {
        get => transform.GetChild(index-1).gameObject;
    }

    public bool IncrementIndex(bool dir)
    {
        if (subslots == 1) return false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        activeSubSlot += dir ? 1 : 0;
        transform.GetChild(activeSubSlot).gameObject.SetActive(true);
        return true;
    }

    public void SetActive(bool b) => gameObject.SetActive(b);
}
 