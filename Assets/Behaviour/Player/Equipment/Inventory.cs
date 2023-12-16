using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Flayer.InputSystem;
using Mirror;

public class Inventory : MonoBehaviour, IComponentInitializable
{
    public static Inventory Local => NetworkClient.connection.identity.gameObject.GetComponentInChildren<Inventory>();
    public NetworkInventory netInventory;


    public InventorySlot[] inventorySlots = new InventorySlot[0];
    public GameObject TP_Prop;
    public Animator FP_HandsAnimator;
    public GameObject FP_RProp;
    public GameObject FP_LProp;
    public int enabledIndex = 0;
    [Range(1f, 10f)] public float usableDistance = 5f;
    public bool allowBombPickup = false;
    public CameraMounter camount;

    public LayerMask UseLayer;

    public InventorySlot this[int index]
    {
        get => inventorySlots[index];
    }

    public void Init() => Start();

    void Start()
    {
        if (!netInventory.hasAuthority) return;
        SetIndex(0);
        print("Inventory:Set");
    }

    void Update()
    {
        if (!netInventory.hasAuthority) return;
        if (InputManager.GetBindDown("Use") && !LocalInfo.IsPaused) use();
        if (InputManager.GetBindDown("Drop") && !LocalInfo.IsPaused) drop();
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        if (scrollValue != 0) IncrementIndex(scrollValue < 0);
    }
    void use()
    {
        //RaycastHit hit = (RaycastHit)LocalInfo.useRaycastHit;
        Physics.Raycast(camount.MainCamera.transform.position, camount.MainCamera.transform.forward, out RaycastHit hit, usableDistance, UseLayer);
        if (!default(RaycastHit).Equals(hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out IUsable usable)) usable.use(this);
            //else if (hit.collider.gameObject.TryGetComponent(out ItemPickup itm)) Pickup(itm.gameObject, itm.overtaking);
            else Debug.Log("Player: Nothing to use");
        }
    }

    void drop() => Drop();

    #region Basic Functions
    public bool Drop()
    {
        GameObject item = inventorySlots[enabledIndex][inventorySlots[enabledIndex].activeSubSlot];
        if (!inventorySlots[enabledIndex].AllowDrop) return false;
        netInventory.CmdDrop(item, item.TryGetComponent<Mag>(out Mag mag) ? mag.Ammo : 0);
        NetworkServer.Destroy(item);
        Debug.Log("Inventory:Drop");
        return true;
    }
    public bool Drop(int indx)
    {
        if (!SetIndex(indx)) return false;
        return Drop();
    }

    public void Pickup(GameObject item, bool overtake_slot)
    {
        InventorySlot slot = null;
        int slotIndex = 0;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (this[i].itemType == item.GetComponent<ItemPickup>().itemType)
            {
                slot = this[i];
                slotIndex = i;
                SetIndex(i);
            }
        }
        if (slot.subslots > slot.transform.childCount)
        {
            netInventory.CmdPickup(item, slotIndex, item.TryGetComponent<Mag>(out Mag mag) ? mag.Ammo : 0);
            NetworkServer.Destroy(item);
            Debug.Log("Inventory:Slot_PickedUp");
        }
        else if (overtake_slot)
        {
            if (!Drop(slotIndex)) { Debug.Log("Inventory:CantDrop-CantPickup " + slotIndex); return; }
            netInventory.CmdPickup(item, slotIndex, item.TryGetComponent<Mag>(out Mag mag) ? mag.Ammo : 0);
            NetworkServer.Destroy(item);
            Debug.Log("Inventory:Slot_Overtaken");
        }
        else
        {
            Debug.Log("Inventory:DidNotPickUp");
        }
        Debug.Log("Inventory:Slot_Full");
    }
    #endregion
    #region Index Methods
    /// <summary>
    /// Equips the next nearest item towards the given direction
    /// </summary>
    /// <param name="dir">The given direction</param>
    void IncrementIndex(bool dir)
    {
        if (inventorySlots[enabledIndex].TryIncrementIndex(dir)) return;
        int step = (dir ? 1 : -1);
        for (int i = 0, x = enabledIndex; i < inventorySlots.Length; i++)
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
    /// <summary>
    /// Equips the nearest item to the given index
    /// </summary>
    /// <param name="index">The given index</param>
    bool SetIndex(int index)
    {
        if (index == enabledIndex) return true;
        int x = index;
        if (index == enabledIndex)
        {
            if (inventorySlots[enabledIndex].TryIncrementIndex(false)) return false;
        }
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            x += 1;
            if (x >= inventorySlots.Length) x = 0;
            if (x < 0) x = inventorySlots.Length - 1;
            if (inventorySlots[x].transform.childCount != 0)
            {
                enableIndex(x);
                enabledIndex = x;
                return x == index;
            }

        }
        return false;
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
