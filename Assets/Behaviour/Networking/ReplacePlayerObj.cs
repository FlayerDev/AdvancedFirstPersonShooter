using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplacePlayerObj : NetworkBehaviour
{

    private void Awake()
    {
        //if (!hasAuthority) Destroy(this);
    }
    public void Replace(NetworkConnectionToClient ownerConn)
    {
        print(NetworkServer.ReplacePlayerForConnection(ownerConn, gameObject, true));
        CmdReplace(ownerConn);
    }
    [Command(ignoreAuthority = true)]
    public void CmdReplace(NetworkConnectionToClient ownerConn)
    {
        RpcReplace();
        TargetReplace(ownerConn);
        //Destroy(this);
    }
    [ClientRpc]
    public void RpcReplace()
    {
        List<IComponentInitializable> comps = new List<IComponentInitializable>(); ;
        comps.AddRange(GetComponents<IComponentInitializable>());
        comps.AddRange(GetComponentsInChildren<IComponentInitializable>());        
        foreach (IComponentInitializable comp in comps)
        {
            comp.Init();
        }
    }
    [TargetRpc]
    public void TargetReplace(NetworkConnection conn)
    {
        if (LocalInfo.Observer != null) LocalInfo.Observer.RemoveObserver();
    }
}
