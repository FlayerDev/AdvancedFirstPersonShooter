using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSpawner : NetworkBehaviour
{
    public GameObject CT_Spawner;
    public GameObject T_Spawner;

    public GameObject TeamSelectionHUD;

    private void Start()
    {
        TeamSelectionHUD.SetActive(true);
    }

    public void SelectTeam(int team)
    {
        //CmdSelectTeam(team, LocalInfo.localIdentity.connectionToClient);

        SpawnPlayer((Team)team, LocalInfo.localIdentity.connectionToClient);

        TeamSelectionHUD.SetActive(false);

        // TODO: Set player info team
    }

    [Command(ignoreAuthority = true)]
    void CmdSetTeam(Team team) 
    { 
        if (LocalInfo.localIdentity.gameObject.TryGetComponent(out PlayerInfo inf)) inf.playerTeam = (int)team; 
    }

    public void SpawnPlayer(Team team, NetworkConnectionToClient owner)
    {
        GameObject pref; ;
        switch (team)
        {
            case Team.CT:
                pref = LobbyManager.Singleton.CT_Player_Prefab;
                break;
            case Team.T:
                pref = LobbyManager.Singleton.T_Player_Prefab;
                break;
            default:
                pref = LobbyManager.Singleton.playerPrefab;
                break;
        }
        var playerObject = Instantiate(pref, getSpawnPosition(team, 1), Quaternion.identity);
        NetworkServer.Spawn(playerObject, owner);
        NetworkServer.ReplacePlayerForConnection(owner, playerObject, true);

        //CmdReInitPlayer(gO);
        CmdSetTeam(team);

        if (LocalInfo.Observer != null && team != Team.None) LocalInfo.Observer.RemoveObserver();
    }
    /*
    [Command]
    public void CmdReInitPlayer(GameObject playerObject)
    {
        ReInitPlayerRpc(playerObject);
    }

    [ClientRpc]
    public void ReInitPlayerRpc(GameObject gO)
    {
        List<IComponentInitializable> comps = new List<IComponentInitializable>(); ;
        comps.AddRange(gO.GetComponentsInChildren<IComponentInitializable>());
        comps.AddRange(gO.GetComponents<IComponentInitializable>());

        foreach (IComponentInitializable comp in comps) comp.Init();
    }
    */
    Vector3 getSpawnPosition(Team team, int seed)
    {
        switch (team)
        {
            case Team.CT:
                return CT_Spawner.transform.GetChild(Math.Clamp(seed, 0, CT_Spawner.transform.childCount)).position;
            case Team.T:
                return T_Spawner.transform.GetChild(Math.Clamp(seed, 0, T_Spawner.transform.childCount)).position;
            default:
                return new Vector3(0, 10, 0);
        }
    }

}


