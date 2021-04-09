using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnComponent : Mirror.NetworkBehaviour
{
    public List<GameObject> SpawnObjects;
    public bool ClientAuthority = true;
    
    public override void OnStartServer()
    {
        foreach (var item in SpawnObjects)
        {
            if (ClientAuthority) Mirror.NetworkServer.Spawn(item, gameObject);
            else Mirror.NetworkServer.Spawn(item);
        }
        Destroy(this);
    }
    public override void OnStartClient()
    {
        Destroy(this);
    }
}
