using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine;
using Mirror;

[AddComponentMenu("Flayer/Combat/Mag")]
public class Mag : NetworkBehaviour
{
    /// <summary>
    /// This is the instance of the held magazine
    /// </summary>
    public static Mag MagSingleton; 
    public bool IsEquipped { get => transform.parent != null; }

    [SyncVar] public int Ammo = 30;
    [SyncVar] public int InventoryAmmo = 90;
    public int Capacity = 30;
    public int InventoryCapacity = 90;
    public int ReloadAmount = 30;
    public int ReloadTimeMS = 2500;

    CancellationTokenSource reloadCancellationToken;
    bool reloaded = false;

    private void OnEnable()
    {
        if (IsEquipped && transform.parent.parent.TryGetComponent(out Inventory inv))
            if (inv.isLocalInventory) MagSingleton = this;
    }
    private void OnDisable()
    {
        if (reloadCancellationToken != null)
            try { reloadCancellationToken.Cancel(); } catch { print("Mag:CouldNotCancelReload"); }

        if (IsEquipped && transform.parent.parent.TryGetComponent(out Inventory inv))
            if (inv.isLocalInventory) MagSingleton = null;
    }

    private void Update()
    {
        if (reloaded)
        {
            reloaded = false;
            var ammobuffer = Ammo;
            CmdSetAmmo(Mathf.Clamp(Ammo + Mathf.Clamp(ReloadAmount, 0, InventoryAmmo), 0, Capacity));
            CmdSetInvAmmo(InventoryAmmo - (ReloadAmount - ammobuffer));
        }
    }

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
    public void StartReload()
    {
        reloadCancellationToken = new CancellationTokenSource();
        Task.Run(async () =>
            {
                await Task.Delay(ReloadTimeMS);
                if (!reloadCancellationToken.Token.IsCancellationRequested) reloaded = true;
            }
        );
    }
    public void CancelReload()
    {
        try
        {
            reloadCancellationToken.Cancel();
            print("Mag:ReloadCancelled");
        }
        catch (NullReferenceException)
        { 
            print("Mag:CouldNotCancelReload(no_active_reload)"); 
        }
        catch (Exception ex)
        {
            print($"Mag:CouldNotCancelReload({ex})");
        }
    }
}
