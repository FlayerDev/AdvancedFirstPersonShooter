using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab : Mirror.NetworkBehaviour
{
    public GameObject SpawnObject;
    public override void OnStartServer()
    {
        Spawn();
    }
    [Mirror.ServerCallback]
    void Spawn()
    {
        Mirror.NetworkServer.Spawn(Instantiate(SpawnObject, transform.position, transform.rotation));
        Destroy(gameObject);
    }
}
