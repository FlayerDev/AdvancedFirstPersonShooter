using UnityEngine;

public class Item : MonoBehaviour, IUsable
{
    public WeaponType weaponType;
    [SerializeField] MonoBehaviour[] DisableOnDrop;
    [SerializeField] bool RigidBodyOnDrop = true;
    [SerializeField] Collider objCollider;

    public GameObject pickupPrefab;
    
    public void drop()
    {
        var drop_item = Instantiate<GameObject>(pickupPrefab, transform.position, Quaternion.identity);
        drop_item.CopyComponent(GetComponent<Mag>());
        drop_item.GetComponent<ItemPickup>().weaponType = weaponType;
    }
    public void pickup(Inventory inventory, bool overtake_slot = true)
    {
        Transform slot = inventory.transform.GetChild((int)weaponType);
        if (slot.childCount < 1) gameObject.transform.parent = slot;
        else if (overtake_slot) { 
            slot.GetChild(0).GetComponent<Item>().drop();
            gameObject.transform.parent = slot;
        }
        objCollider.enabled = false;
        if (RigidBodyOnDrop) Destroy(gameObject.GetComponent<Rigidbody>());
        if (DisableOnDrop.Length > 0) foreach (MonoBehaviour obj in DisableOnDrop) obj.enabled = true;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
    }

    public void use(GameObject user) => pickup(user.GetComponent<Inventory>());

    private void Awake()
    {

    }
}
public enum WeaponType
{
    Main,
    Secondary,
    Utility
}
public enum ItemType
{
    None,
    Melee,
    Pistol,
    Rifle,
    Shotgun,

}
