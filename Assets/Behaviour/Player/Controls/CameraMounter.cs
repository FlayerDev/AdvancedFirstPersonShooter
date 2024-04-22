using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[AddComponentMenu("Flayer/Controls/Camera Mounter")]
public class CameraMounter : MonoBehaviour
{
    public bool isPlayer = true;
    public Camera MainCamera;
    public GameObject FP_Hands;
    [Space]
    public GameObject[] BodyParts;
    [Space]
    public Vector3 HandOffset;
    public bool Focused;

    public GameObject LookAtIKObject;

    private void Awake()
    {
        MainCamera = Camera.main;
    }
    // Start is called before the first frame update
    public void Start()
    {
        unfocusHandler = () => { Unfocus(); };
        if (!transform.parent.GetComponent<Mirror.NetworkIdentity>().hasAuthority)
        {
            print("CameraMounter:QuittingInit(Is'nt local player)");
            return;
        }
        Focus();
        if(transform.parent.TryGetComponent<PlayerInfo>(out PlayerInfo plInf)) plInf.OnPlayerResurrectionEvent += () => Focus();
    }
    private void Update()
    { 
        if (transform != MainCamera.transform.parent && Focused) Unfocus();
        if (Focused)
        {
            FP_Hands.transform.localPosition = HandOffset;
            MainCamera.transform.localPosition = new Vector3(0, transform.parent.GetComponent<CustomPlayerMovement>().HeightBuffer - 0.1f, 0);
            if (transform.parent.GetComponent<Mirror.NetworkIdentity>().isLocalPlayer && isPlayer) 
                LookAtIKObject.transform.position = MainCamera.transform.position + MainCamera.transform.forward;
        }
    }

    System.Action unfocusHandler;

    [ButtonGroup]
    public void Focus()
    {
        if (isPlayer)
        {
            print("CameraMounter:isPlayer");
            Focused = true;
            //Hands
            FP_Hands.SetActive(true);
            FP_Hands.transform.parent = MainCamera.transform;
            FP_Hands.transform.localPosition = HandOffset;
            FP_Hands.transform.localRotation = Quaternion.identity;
            //Body
            setActiveBodyParts(false);
            var plInf = transform.parent.GetComponent<PlayerInfo>();
            LocalInfo.focusedPlayerInfo = plInf;
            plInf.OnPlayerDeathEvent += unfocusHandler;
        }
        //Camera
        MainCamera.transform.parent = transform;
        CameraRotation camRot = transform.parent.GetComponent<CameraRotation>();
        camRot.playerBody = transform.parent.gameObject;
        camRot.cameraObj = MainCamera.gameObject;
        MainCamera.transform.localPosition = Vector3.zero;
        print("CameraMounter:Focused");
    }
    [ButtonGroup]
    public void Unfocus()
    {
        Focused = false;
        if(MainCamera.transform.parent == transform) MainCamera.transform.parent = null;
        transform.parent.GetComponent<CameraRotation>().cameraObj = null;
        if (isPlayer)
        {
            //Hands
            FP_Hands.transform.parent = transform.parent;
            FP_Hands.transform.position = Vector3.zero;
            FP_Hands.SetActive(false);
            //Body
            setActiveBodyParts(true);
            transform.parent.GetComponent<PlayerInfo>().OnPlayerDeathEvent -= unfocusHandler;
        }
        LocalInfo.focusedPlayerInfo = null;
        
        print("CameraMounter:Unfocused");
    }
    void setActiveBodyParts(bool state)
    {
        foreach (GameObject part in BodyParts)
        {
            part.SetActive(state);
        }
    }
}
