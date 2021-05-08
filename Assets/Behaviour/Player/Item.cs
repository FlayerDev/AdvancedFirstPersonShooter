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
        AnimatorParameterSync.Local.animIndex = animationIndex;
        AnimatorParameterSync.Local.Equip = true;
        CmdSpawnItem(Inventory.Local.transform.parent.gameObject, itemSide);
    }
    [Command(ignoreAuthority = true)]
    public void CmdSpawnItem(GameObject player, bool handSide)
    {
        // TP
        TP_Runtime = Instantiate(TP_Prefab);
        NetworkServer.Spawn(TP_Runtime, ownerConnection: player.GetComponent<NetworkIdentity>().connectionToClient);
        // FP
        FP_Runtime = Instantiate(FP_Prefab);
        NetworkServer.Spawn(FP_Runtime, ownerConnection: player.GetComponent<NetworkIdentity>().connectionToClient);

        // ClientRpc
        RpcSpawnItem(player, TP_Runtime, FP_Runtime, handSide);
    }
    [ClientRpc]
    void RpcSpawnItem(GameObject player, GameObject TPr, GameObject FPr, bool handSide)
    {
        Inventory inv = player.GetComponent<NetworkInventory>().inventory;
        //TP
        TP_Runtime = TPr;
        TPr.transform.SetParent(inv.TP_Prop.transform);
        TPr.transform.localPosition = TP_PositionOffset;
        TPr.transform.localRotation = TP_RotationOffset;
        //FP
        FP_Runtime = FPr;
        FPr.transform.SetParent(handSide? inv.FP_RProp.transform : inv.FP_LProp.transform);
        FPr.transform.localPosition = FP_PositionOffset;
        FPr.transform.localRotation = FP_RotationOffset;
        Animator anim = player.GetComponentInChildren<Inventory>().FP_HandsAnimator;
        anim.runtimeAnimatorController = FP_HandAnimations as RuntimeAnimatorController;
        anim.SetTrigger("Equip");
    }
    [Command]
    void CmdDespawn() //TODO: Doesnt despawn the first object picked up
    {
        NetworkServer.Destroy(TP_Runtime);
        NetworkServer.Destroy(FP_Runtime);
    }
    private void OnDisable()
    {
        if (!hasAuthority) return;
        CmdDespawn();
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