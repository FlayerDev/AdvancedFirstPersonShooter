using Mirror;
using UnityEngine;

public class NetworkSpawner
{
    [Command]
    public GameObject CmdInstantiate(GameObject obj)
    {
        GameObject gameObject = Object.Instantiate(obj);
        NetworkServer.Spawn(gameObject);
        return gameObject;
    }
    [Command]
    public GameObject CmdInstantiate(GameObject obj, Vector3 position, Quaternion rotation)
    {
        GameObject gameObject = Object.Instantiate(obj, position, rotation);
        NetworkServer.Spawn(gameObject);
        return gameObject;
    }
    [Command]
    public GameObject CmdInstantiate(GameObject obj, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject gameObject = Object.Instantiate(obj, position, rotation, parent);
        NetworkServer.Spawn(gameObject);
        return gameObject;
    }
    [Command]
    public GameObject CmdInstantiate(GameObject obj, Transform parent, bool instantiateInWorldSpace = false)
    {
        GameObject gameObject = Object.Instantiate(obj, parent, instantiateInWorldSpace);
        NetworkServer.Spawn(gameObject);
        return gameObject;
    }
    [Command]
    public void CmdAssignClientAuthority(NetworkIdentity id , NetworkConnection conn)
    {
        if (conn == null) Debug.Log("conn null");
        id.AssignClientAuthority(conn);
    }
}
