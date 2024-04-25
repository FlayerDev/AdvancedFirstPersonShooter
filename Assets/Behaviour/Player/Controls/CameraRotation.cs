using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Flayer/Controls/Camera Rotation")]
public class CameraRotation : Mirror.NetworkBehaviour
{
    [Header("ReferencedObjects")]
    public GameObject cameraObj;
    public GameObject playerBody;

    public bool isCursorLocked = false;

    float xRoatation = 0f;

    public void setCursorLockState(bool state)
    {
        if (state == isCursorLocked) return;

        if (state)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnDisable() => setCursorLockState(false);
    private void OnDestroy() => OnDisable();


    void Update()
    {
        if (isLocalPlayer && cameraObj && !LocalInfo.IsPaused)
        {
            setCursorLockState(true);
            float mouseX, mouseY;
            mouseX = Input.GetAxis("Mouse X") * (LocalInfo.Sensitivity * 10) * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * (LocalInfo.Sensitivity * 10) * Time.deltaTime;

            xRoatation -= mouseY;
            xRoatation = Mathf.Clamp(xRoatation, -90f, 90f);

            cameraObj.transform.localRotation = Quaternion.Euler(xRoatation, 0, 0);
            playerBody.transform.Rotate(Vector3.up * mouseX);
        }
        else
        {
            setCursorLockState(false);
            print($"CameraRotation:UpdateSuspended loc:{isLocalPlayer} camnotnull:{(bool)cameraObj} notpaused:{!LocalInfo.IsPaused}");
        }
    }
}
