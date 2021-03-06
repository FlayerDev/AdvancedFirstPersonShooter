﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : Mirror.NetworkBehaviour
{
    [Header("ReferencedObjects")]
    public GameObject cameraObj;
    public GameObject playerBody;

    public float mouseSensitivity = 1f;

    float xRoatation = 0f;
    
    void Start()
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
            mouseX = Input.GetAxis("Mouse X") * (mouseSensitivity * 10) * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * (mouseSensitivity * 10) * Time.deltaTime;

            xRoatation -= mouseY;
            xRoatation = Mathf.Clamp(xRoatation, -90f, 90f);

            cameraObj.transform.localRotation = Quaternion.Euler(xRoatation, 0, 0);
            playerBody.transform.Rotate(Vector3.up * mouseX);
        }
    }
}
