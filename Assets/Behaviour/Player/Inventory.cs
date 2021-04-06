using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Flayer.InputSystem;
using Mirror;

public class Inventory : Mirror.NetworkBehaviour
{
    public static Inventory Local;

    public InventorySlot[] inventorySlots = new InventorySlot[0];
    public GameObject TP_RProp;
    public GameObject TP_LProp;
    public int enabledIndex = 0;
    [Range(1f, 10f)] public float usableDistance = 5f;
    public bool allowBombPickup = false;
    public CameraMounter camount;

    public NetworkIdentity ParentNetID;


    public InventorySlot this[int index]
    {
        get => inventorySlots[index];
    }

    void Start()
    {
        if (hasAuthority) print("has auth");
        ParentNetID = gameObject.transform.parent.GetComponent<NetworkIdentity>();
        if (!ParentNetID.isLocalPlayer) return;
        Local = this;
        //CmdGetAuthority(ParentNetID.connectionToClient);
        //if (!hasAuthority) return;
        SetIndex(0);
    }
    [Command]
    void CmdGetAuthority(NetworkConnectionToClient conn)
    {
        netIdentity.AssignClientAuthority(conn);
    }

    void Update()
    {
        if (!ParentNetID.isLocalPlayer) return;
        //if (!hasAuthority) return;
        if (InputManager.GetBindDown("Use") && !LocalInfo.IsPaused) use();
        if (InputManager.GetBindDown("Drop") && !LocalInfo.IsPaused) drop();
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        if (scrollValue != 0) IncrementIndex(scrollValue < 0);
    }
    void use()
    {
        //RaycastHit hit = (RaycastHit)LocalInfo.useRaycastHit;
        Physics.Raycast(camount.MainCamera.transform.position, camount.MainCamera.transform.forward, out RaycastHit hit);
        if (hit.collider.gameObject.TryGetComponent(out ItemPickup itm)) {
            Pickup(itm.gameObject, false);
        }

    }

    void drop() => Drop();

    public async void Drop()
    {
        GameObject item = inventorySlots[enabledIndex][inventorySlots[enabledIndex].activeSubSlot];
        if (!inventorySlots[enabledIndex].AllowDrop) return;
        Item itm = item.GetComponent<Item>();
        GameObject drop_item = Instantiate<GameObject>(itm.pickupPrefab, transform.position, Quaternion.identity);
        drop_item.GetComponent<ItemPickup>().itemType = itm.itemType;
        item.SetActive(false);
        NetworkServer.Spawn(drop_item);
        await Task.Delay(10);
        drop_item.GetComponent<Mag>().CmdSetAmmo(item.GetComponent<Mag>().Ammo);
        NetworkServer.Destroy(item);
        Debug.Log("Inventory:Drop");
    }

    //[Command]
    public async void Pickup(GameObject item, bool overtake_slot)
    {
        InventorySlot slot = null;
        GameObject weaponPrefab = item.GetComponent<ItemPickup>().weaponPrefab;
        for (int i = 0; i < inventorySlots.Length; i++) 
        {
            if (this[i].itemType == item.GetComponent<ItemPickup>().itemType) { 
                slot = this[i];
                SetIndex(i);
            }
        }
        if (slot.subslots > slot.transform.childCount)
        {
            GameObject wepbuff = Instantiate(weaponPrefab, slot.transform);
            NetworkServer.Spawn(wepbuff, transform.parent.gameObject);
            await Task.Delay(10);
            wepbuff.GetComponent<Mag>().CmdSetAmmo(item.GetComponent<Mag>().Ammo);
            //wepbuff.GetComponent<Mag>().CmdSetAmmo(item.GetComponent<Mag>().Ammo);
            NetworkServer.Destroy(item);
            Debug.Log("Inventory:Slot_PickedUp");
        }
        else if (overtake_slot)
        {
            Drop();
            GameObject wepbuff = Instantiate(weaponPrefab, slot.transform);
            NetworkServer.Spawn(wepbuff, transform.parent.gameObject);
            await Task.Delay(10);
            wepbuff.GetComponent<Mag>().CmdSetAmmo(item.GetComponent<Mag>().Ammo);
            NetworkServer.Destroy(item);
            Debug.Log("Inventory:Slot_Overtaken");
        }
        Debug.Log("Inventory:Slot_Full");
    }

    #region ChangeWeapon

    void IncrementIndex(bool dir)
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
    void SetIndex(int index)
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
