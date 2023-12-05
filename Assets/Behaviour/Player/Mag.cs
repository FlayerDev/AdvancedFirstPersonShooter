using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[AddComponentMenu("Flayer/Combat/Mag")]
public class Mag : NetworkBehaviour
{
    [SyncVar] public int Ammo = 30;
    [SyncVar] public int InventoryAmmo = 90;
    public int Capacity = 30;
    public int InventoryCapacity = 90;
    public int ReloadAmount = 30;
    public int ReloadTimeMS = 2500;

    [Command(ignoreAuthority = true)]
    public void CmdSetAmmo(int val)
    {
        Debug.Log(val);
        Ammo = val;
    }
    [Command(ignoreAuthority = true)]
    public void CmdSetInvAmmo(int val)
    {
        Debug.Log(val);
        InventoryAmmo = val;
    }
    public async void Reload()
    {
        await System.Threading.Tasks.Task.Delay(ReloadTimeMS);
        var ammobuffer = Ammo;
        CmdSetAmmo(Mathf.Clamp(Ammo + Mathf.Clamp(ReloadAmount, 0, InventoryAmmo), 0, Capacity));
        CmdSetInvAmmo(InventoryAmmo - (ReloadAmount - ammobuffer));
    }
}
