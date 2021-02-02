using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Flayer/Combat/Mag")]
public class Mag : MonoBehaviour
{
    public int ammo;
    public int InventoryCapacity = 90;
    public int Capacity = 30;
    public int ReloadAmount = 30;
    public float ReloadTime = 2500f;
}
