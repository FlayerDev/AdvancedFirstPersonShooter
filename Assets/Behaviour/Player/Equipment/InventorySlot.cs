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

    /// <summary> </summary>
    /// <param name="index"></param>
    /// <returns>transform.GetChild(index).gameObject</returns>
    public GameObject this[int index]
    {
        get => transform.GetChild(index).gameObject;
    }
    private void OnEnable()
    {
        if (transform.childCount != 0)
            transform.GetChild(0).gameObject.SetActive(true);
    }
    /// <summary>
    /// Attempts to enable another item within the same InventorySlot
    /// </summary>
    /// <param name="dir">Increment Direction</param>
    /// <returns>True if the subslot successfully increment index</returns>
    public bool TryIncrementIndex(bool dir)
    {
        if (subslots == 1) return false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Item>().ToggleActive(false);
            transform.GetChild(i).gameObject.SetActive(false);
        }
        activeSubSlot += dir ? 1 : 0;
        transform.GetChild(activeSubSlot).gameObject.SetActive(true);
        return true;
    }

    /// <summary>
    /// Sets active the object whilst notifying the Item component to set up model destruction if b == false
    /// </summary>
    /// <param name="b"></param>
    public void SetActive(bool b) {
        if (!b) foreach (Item item in GetComponentsInChildren<Item>()) item.ToggleActive(false);
        gameObject.SetActive(b); 
    }
}
 