using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkInventory : NetworkBehaviour
{
    public bool isLocalInventory => GetComponentInParent<NetworkIdentity>().isLocalPlayer;

    public Inventory inventory;
    private void Start()
    {
        inventory.netInventory = this;
    }

    [Command(ignoreAuthority = true)]
    public void CmdDrop(GameObject item, int ammo)
    {
        Item itm = item.GetComponent<Item>();
        GameObject drop_item = Instantiate<GameObject>(itm.pickupPrefab, transform.position + new Vector3(0, 0.5f, 0), new Quaternion(20,10,30,0));
        NetworkServer.Spawn(drop_item);
        drop_item.GetComponent<Mag>().Ammo = ammo;
        itm.ToggleActive(false);
    }

    [Command(ignoreAuthority = true)]
    public void CmdPickup(GameObject item, int slot, int ammo)
    {
        ItemPickup item_pickup = item.GetComponent<ItemPickup>();
        GameObject wepbuff = Instantiate(item_pickup.weaponPrefab, inventory[slot].transform);
        if (wepbuff.TryGetComponent(out Mag mag)) mag.Ammo = ammo;
        NetworkServer.Spawn(wepbuff, gameObject);
        RpcPickup(wepbuff,slot);
    }
    [ClientRpc]
    void RpcPickup(GameObject wepbuff, int slot)
    {
        wepbuff.transform.parent = inventory[slot].transform;
    }

    #region Animator commands
    [Command]
    public void CmdSetTrigger(string trigger)
    {
        RpcSetTrigger(trigger);
    }
    [ClientRpc]
    void RpcSetTrigger(string trigger)
    {
        inventory.FP_HandsAnimator.SetTrigger(trigger);
    }
    #endregion
}
