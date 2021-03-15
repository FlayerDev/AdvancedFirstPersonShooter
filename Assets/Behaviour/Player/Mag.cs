using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[AddComponentMenu("Flayer/Combat/Mag")]
public class Mag : NetworkBehaviour
{
    public int Ammo { 
        get
        {
            return ammo;
        }
        set
        {
            CmdSetAmmo(value);
        }
    }
    [SerializeField, SyncVar] int ammo;
    public int InventoryCapacity = 90;
    public int Capacity = 30;
    public int ReloadAmount = 30;
    public float ReloadTime = 2500f;

    private void Start()
    {
        ammo = Capacity;
        //CmdSetAmmo(Capacity);
    }
    [Command]
    void CmdSetAmmo(int val)
    {
        ammo = val;
    }
}
