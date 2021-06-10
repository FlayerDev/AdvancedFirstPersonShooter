using Mirror;
using UnityEngine;

public class Item : NetworkBehaviour
{
    public ItemType itemType;
    public AnimationIndex animationIndex;
    public GameObject pickupPrefab;
    public bool allowIdleAnimation; //TODO: Implement

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
        ToggleActive(true);
    }
    private void Update() //Note: that may cause a bug
    {
        if (!(FP_Runtime && TP_Runtime))
        {
            FP_Runtime = null;
            TP_Runtime = null;
        }
    }
    private void OnEnable() 
    {
        ToggleActive(true);
        //ToggleActive(true);
    }
    [TargetRpc]
    public void TargetToggleActive(NetworkConnection netConn, bool state) => ToggleActive(state);
    public void ToggleActive(bool state)
    {
        //Note: re-enable this
        //if (state == gameObject.activeInHierarchy) return; //Note: re-enable this
        if (state)
        {
            if (!hasAuthority) return;
            gameObject.SetActive(true);
            AnimatorParameterSync.Local.animIndex = animationIndex;
            AnimatorParameterSync.Local.Equip = true;
            CmdSpawnItem(Inventory.Local.transform.parent.gameObject, itemSide);
        }
        else
        {
            if (!hasAuthority) return;
            CmdDespawn();
            gameObject.SetActive(false);
        }
    }
    #region Spawn Item Rpcs  
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
        clearHand(inv.TP_Prop);
        TP_Runtime = TPr;
        TPr.transform.SetParent(inv.TP_Prop.transform);
        TPr.transform.localPosition = TP_PositionOffset;
        TPr.transform.localRotation = TP_RotationOffset;
        //FP
        clearHand(inv.FP_RProp);
        clearHand(inv.FP_LProp);
        FP_Runtime = FPr;
        FPr.transform.SetParent(handSide ? inv.FP_RProp.transform : inv.FP_LProp.transform);
        FPr.transform.localPosition = FP_PositionOffset;
        FPr.transform.localRotation = FP_RotationOffset;
        Animator anim = player.GetComponentInChildren<Inventory>().FP_HandsAnimator;
        anim.runtimeAnimatorController = FP_HandAnimations as RuntimeAnimatorController;
        anim.SetBool("allowIdle", allowIdleAnimation);
        anim.SetTrigger("Equip");
        //Others

        static void clearHand(GameObject hand)
        {
            if (hand.transform.childCount < 1) return;
            for (int i = 0; i < hand.transform.childCount; i++)
            {
                if (hand.transform.GetChild(i).gameObject.CompareTag("Model"))
                    Destroy(hand.transform.GetChild(i).gameObject);
            }

        }
    }
    #endregion
    [Command]
    void CmdDespawn() //NOTE: Doesnt despawn the first object picked up.  WorkAround: clearHand inline methond above
    {
        NetworkServer.Destroy(TP_Runtime);
        NetworkServer.Destroy(FP_Runtime);
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