using Mirror;
using UnityEngine;

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
        CmdSpawnItem(Inventory.Local.transform.parent.gameObject,itemSide);
    }
    private void OnEnable()
    {
        if (!hasAuthority) return;
        CmdSpawnItem(Inventory.Local.transform.parent.gameObject, itemSide);
    }
    [Command(ignoreAuthority = true)]
    public void CmdSpawnItem(GameObject player, bool handSide)
    {
        // Third Person
        TP_Runtime = Instantiate(TP_Prefab);
        NetworkServer.Spawn(TP_Runtime, ownerConnection: player.GetComponent<NetworkIdentity>().connectionToClient);

        AnimatorParameterSync.Local.animIndex = animationIndex;
        AnimatorParameterSync.Local.Equip = true;

        RpcSpawnItem(player, TP_Runtime);
    }
    [ClientRpc]
    void RpcSpawnItem(GameObject player, GameObject TPr)
    {
        TP_Runtime = TPr;
        TPr.transform.SetParent(player.GetComponent<NetworkInventory>().inventory.TP_Prop.transform);
        TPr.transform.localPosition = TP_PositionOffset;
        TPr.transform.localRotation = TP_RotationOffset;
    }
    private void OnDisable()
    {
        if (!hasAuthority) return;
        NetworkServer.Destroy(TP_Runtime);
    }
    private void OnDestroy() => OnDisable();

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