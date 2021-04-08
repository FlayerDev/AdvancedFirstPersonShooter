using UnityEngine;
using Mirror;
using System.Threading.Tasks;

public class Item : NetworkBehaviour
{
    public ItemType itemType;
    public AnimationIndex animationIndex;
    public GameObject pickupPrefab;

    [Header("First Person")]
    public GameObject FP_Prefab;
    public Vector3 FP_PositionOffset;
    public Quaternion FP_RotationOffset;
    public AnimatorOverrideController FP_HandAnimations;

    [Header("Third Person")]
    public GameObject TP_Prefab;
    public Vector3 TP_PositionOffset;
    public Quaternion TP_RotationOffset;

    [Header("Runtime")]
    public GameObject FP_Runtime;
    public GameObject TP_Runtime;
    [Space]
    public bool itemSide = true;

    private void Start()
    {
        if (!hasAuthority || !gameObject.activeInHierarchy) return;
        CmdSpawnItem();
    }
    private void OnEnable()
    {
        if (!hasAuthority) return;
        CmdSpawnItem();
    }
    [Command(ignoreAuthority = true)]
    public void CmdSpawnItem()
    {
        TP_Runtime = Instantiate(TP_Prefab, Inventory.Local.TP_Prop.transform);
        TP_Runtime.transform.localPosition = TP_PositionOffset;
        TP_Runtime.transform.localRotation = TP_RotationOffset;
        NetworkServer.Spawn(TP_Runtime, Inventory.Local.transform.parent.GetComponent<NetworkIdentity>().connectionToClient);
        Inventory.Local.transform.parent.GetComponent<AnimatorParameterSync>().animIndex = animationIndex;
    }
    private void OnDisable()
    {
        if (!hasAuthority) return;
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
public enum AnimationIndex : byte
{
    Melee,
    Rifle,
    SMG,
    Pistol,
    Throwable
}