using UnityEngine;
using System.Collections.Generic;
using Unity.Flayer.InputSystem;
using Mirror;

public class Inventory : Mirror.NetworkBehaviour
{
    public InventorySlot[] inventorySlots = new InventorySlot[0];
    public int enabledIndex = 0;
    [Range(1f, 10f)] public float usableDistance = 5f;
    public bool allowBombPickup = false;
    public CameraMounter camount;

    NetworkIdentity ParentNetID;


    public InventorySlot this[int index]
    {
        get => inventorySlots[index];
    }

    void Start()
    {
        ParentNetID = gameObject.transform.parent.GetComponent<NetworkIdentity>();
        if (!ParentNetID.isLocalPlayer) return;
        CmdSetIndex(0);
    }


    void Update()
    {
        if (!ParentNetID.isLocalPlayer) return;
        if (InputManager.GetBindDown("Use") && !LocalInfo.IsPaused) use();
        if (InputManager.GetBindDown("Drop") && !LocalInfo.IsPaused) drop();
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        if (scrollValue != 0) CmdIncrementIndex(scrollValue < 0);
    }
    void use()
    {
        /*
        RaycastHit hit = (RaycastHit)LocalInfo.useRaycastHit;
        */
        Physics.Raycast(camount.MainCamera.transform.position, camount.MainCamera.transform.forward, out RaycastHit hit);
        if (hit.collider.gameObject.TryGetComponent(out ItemPickup itm)) {
            new NetworkSpawner().CmdAssignClientAuthority(itm.netIdentity, transform.parent.GetComponent<NetworkIdentity>().connectionToClient);
            itm.CmdPickup(this, true); 
        }

    }

    void drop() => inventorySlots[enabledIndex].drop();

    #region ChangeWeapon
    [Command]
    void CmdIncrementIndex(bool dir)
    {
        RpcIncrementIndex(dir);
    }
    [ClientRpc]
    void RpcIncrementIndex(bool dir)
    {
        if (inventorySlots[enabledIndex].IncrementIndex(dir)) return;
        int step = (dir ? 1 : -1);
        for (int i = 0,x = enabledIndex; i < inventorySlots.Length; i++)
        {
            x += step;
            if (x >= inventorySlots.Length) x = 0;
            if (x < 0) x = inventorySlots.Length - 1;
            if (inventorySlots[x].transform.childCount != 0)
            {
                enableIndex(x);
                enabledIndex = x;
                return;
            }
        }
    }
    [Command]
    void CmdSetIndex(int index)
    {
        RpcSetIndex(index);
    }
    [ClientRpc]
    void RpcSetIndex(int index)
    {
        if(index == enabledIndex)
        {
            if (inventorySlots[enabledIndex].IncrementIndex(false)) return;
        }
        for (int i = 0, x = index; i < inventorySlots.Length; i++)
        {
            x += 1;
            if (x >= inventorySlots.Length) x = 0;
            if (x < 0) x = inventorySlots.Length - 1;
            if (inventorySlots[x].transform.childCount != 0)
            {
                enableIndex(x);
                enabledIndex = x;
                return;
            }
        }
    }
    void enableIndex(int index)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i == index) inventorySlots[i].SetActive(true);
            else inventorySlots[i].SetActive(false);
        }
    }
    #endregion
}
