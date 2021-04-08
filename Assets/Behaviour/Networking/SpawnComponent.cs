using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnComponent : MonoBehaviour
{
    public List<GameObject> SpawnObjects;
    public bool ClientAuthority = true;
    private void Start()
    {
        Spawn();
    }
    [Mirror.ServerCallback]
    void Spawn()
    {
        foreach (var item in SpawnObjects)
        {
            if (ClientAuthority) Mirror.NetworkServer.Spawn(item, gameObject);
            else Mirror.NetworkServer.Spawn(item);
        }
        Destroy(this);
    }
}
