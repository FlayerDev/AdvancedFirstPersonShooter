using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplacePlayerObj : NetworkBehaviour
{

    private void Awake()
    {
        if (!hasAuthority) Destroy(this);
    }

    //[ServerCallback]
    [Command]
    public void Replace(NetworkConnectionToClient ownerConn)
    {
        NetworkServer.ReplacePlayerForConnection(this.connectionToClient, gameObject);
        IComponentInitializable[] comps = GetComponents<IComponentInitializable>();
        foreach(IComponentInitializable comp in comps)
        {
            comp.Init();
        }
        GetComponent<CameraMounter>().Focus();
        Destroy(this);
    }
}
