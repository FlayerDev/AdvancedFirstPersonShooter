using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!hasAuthority) return;
        LocalInfo.Observer = this;
        LocalInfo.localIdentity = netIdentity;
    }

    public void RemoveObserver()
    {
        GetComponentInChildren<CameraMounter>().Unfocus();
        NetworkServer.Destroy(gameObject);
    }
}
