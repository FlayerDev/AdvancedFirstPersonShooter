using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[AddComponentMenu("Flayer/Combat/Mag")]
public class Mag : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSetAmmo))] public int Ammo = 30;
    public int InventoryCapacity = 90;
    public int Capacity = 30;
    public int ReloadAmount = 30;
    public float ReloadTime = 2500f;

    void OnSetAmmo(int old, int n)
    {
        Debug.Log($"ammo: {Ammo}");
        Debug.Log($"old: {old}");
        Debug.Log($"new: {n}");
    }
    [Command(ignoreAuthority = true)]
    public void CmdSetAmmo(int val)
    {
        Debug.Log(val);
        Ammo = val;
    }
}
