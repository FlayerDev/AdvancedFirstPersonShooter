using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnComponent : MonoBehaviour
{
    public List<GameObject> SpawnObjects;
    private void Start()
    {
        Spawn();
    }
    [Mirror.ServerCallback]
    void Spawn()
    {
        foreach (var item in SpawnObjects)
        {
            Mirror.NetworkServer.Spawn(item);
        }
        Destroy(this);
    }
}
