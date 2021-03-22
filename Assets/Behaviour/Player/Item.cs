using UnityEngine;
using Mirror;

public class Item : NetworkBehaviour
{
    public ItemType itemType;
    public GameObject pickupPrefab;
    [Header("First Person")]
    public GameObject FP_Prefab;
    public Vector3 FP_PositionOffset;
    public Quaternion FP_RotationOffset;
    [Header("Third Person")]
    public GameObject TP_Prefab;
    public Vector3 TP_PositionOffset;
    public Quaternion TP_RotationOffset;
    [Header("Runtime")]
    public GameObject FP_Runtime;
    public GameObject TP_Runtime;
    [Space]
    public bool itemSide = true;

    private void OnEnable()
    {
        TP_Runtime = Instantiate(TP_Prefab, itemSide ? Inventory.Local.TP_RProp.transform : Inventory.Local.TP_LProp.transform);
        //TP_Runtime.transform.localPosition = Vector3.zero;
        //TP_Runtime.transform.localRotation = Quaternion.identity;
        TP_Runtime.transform.localPosition = TP_PositionOffset;
        TP_Runtime.transform.localRotation = TP_RotationOffset;
        NetworkServer.Spawn(TP_Runtime);
    }
    private void OnDisable()
    {
        NetworkServer.Destroy(TP_Runtime);
    }
}
public enum ItemType
{
    Melee,
    Main,
    Secondary,
    Utility
}
