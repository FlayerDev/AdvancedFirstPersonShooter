using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : Mirror.NetworkBehaviour, IComponentInitializable
{
    [Header("ReferencedObjects")]
    public GameObject cameraObj;
    public GameObject playerBody;

    float xRoatation = 0f;
    
    public void Init() => OnEnable();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void OnDestroy() => OnDisable();


    void Update()
    {
        if (!isLocalPlayer || !cameraObj) return;
        if (!LocalInfo.IsPaused)
        {
            float mouseX, mouseY;
            mouseX = Input.GetAxis("Mouse X") * (LocalInfo.Sensitivity * 10) * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * (LocalInfo.Sensitivity * 10) * Time.deltaTime;

            xRoatation -= mouseY;
            xRoatation = Mathf.Clamp(xRoatation, -90f, 90f);

            cameraObj.transform.localRotation = Quaternion.Euler(xRoatation, 0, 0);
            playerBody.transform.Rotate(Vector3.up * mouseX);
        }
    }
}
