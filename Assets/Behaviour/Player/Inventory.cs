using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Flayer.InputSystem;
using Mirror;

public class Inventory : Mirror.NetworkBehaviour
{
    public static Inventory Local { get => NetworkClient.connection.identity.gameObject.GetComponentInChildren<Inventory>(); }

    public InventorySlot[] inventorySlots = new InventorySlot[0];
    public GameObject TP_Prop;
    public GameObject FP_RProp;
    public GameObject FP_LProp;
    public int enabledIndex = 0;
    [Range(1f, 10f)] public float usableDistance = 5f;
    public bool allowBombPickup = false;
    public CameraMounter camount;

    public NetworkIdentity ParentNetID { get => NetworkClient.connection.identity; }


    public InventorySlot this[int index]
    {
        get => inventorySlots[index];
    }

    void Start()
    {
        if (!hasAuthority) return;
        SetIndex(0);
    }
    [Command(ignoreAuthority = true)]
    void CmdGetAuthority(NetworkConnectionToClient conn)
    {
        netIdentity.AssignClientAuthority(conn);
    }

    void Update()
    {
        if (!hasAuthority) return;
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

    public void Drop()
    {
        GameObject item = inventorySlots[enabledIndex][inventorySlots[enabledIndex].activeSubSlot];
        if (!inventorySlots[enabledIndex].AllowDrop) return;
        CmdDrop(item, item.TryGetComponent<Mag>(out Mag mag) ? mag.Ammo : 0);
        NetworkServer.Destroy(item);
        Debug.Log("Inventory:Drop");
    }
    [Command(ignoreAuthority = true)]
    void CmdDrop(GameObject item,int ammo)
    {
        Item itm = item.GetComponent<Item>();
        GameObject drop_item = Instantiate<GameObject>(itm.pickupPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(drop_item);
        drop_item.GetComponent<Mag>().Ammo = ammo;
    }

    public void Pickup(GameObject item, bool overtake_slot)
    {
        InventorySlot slot = null;
        int slotIndex = 0;
        for (int i = 0; i < inventorySlots.Length; i++) 
        {
            if (this[i].itemType == item.GetComponent<ItemPickup>().itemType) { 
                slot = this[i];
                slotIndex = i;
                SetIndex(i);
            }
        }
        if (slot.subslots > slot.transform.childCount)
        {
            CmdPickup(item, slotIndex, item.TryGetComponent<Mag>(out Mag mag) ? mag.Ammo : 0);
            NetworkServer.Destroy(item);
            Debug.Log("Inventory:Slot_PickedUp");
        }
        else if (overtake_slot)
        {
            Drop();
            CmdPickup(item, slotIndex, item.TryGetComponent<Mag>(out Mag mag) ? mag.Ammo : 0);
            NetworkServer.Destroy(item);
            Debug.Log("Inventory:Slot_Overtaken");
        }
        Debug.Log("Inventory:Slot_Full");
    }
    [Command(ignoreAuthority = true)]
    void CmdPickup(GameObject item, int slot, int ammo)
    {
        var prefab = item.GetComponent<ItemPickup>().weaponPrefab;
        GameObject wepbuff = Instantiate(prefab, this[slot].transform);
        if (wepbuff.TryGetComponent(out Mag mag)) mag.Ammo = ammo;
        NetworkServer.Spawn(wepbuff, transform.parent.gameObject);
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
