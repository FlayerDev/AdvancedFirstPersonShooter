using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkSpawner
{
    public void ServerSpawn(GameObject obj , Vector3 position , Quaternion rotation)
    {
        GameObject gameObject = Object.Instantiate(obj);
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        NetworkServer.Spawn(gameObject);
    }
}
