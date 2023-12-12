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
        GameObject pref = LobbyManager.Singleton.playerPrefab;
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
        var gO = Instantiate(pref, getSpawnPosition(team, 1), Quaternion.identity);
        NetworkServer.Spawn(gO, owner);
        NetworkServer.ReplacePlayerForConnection(owner, gameObject, true);

        List<IComponentInitializable> comps = new List<IComponentInitializable>(); ;
        comps.AddRange(GetComponents<IComponentInitializable>());
        comps.AddRange(GetComponentsInChildren<IComponentInitializable>());

        foreach (IComponentInitializable comp in comps) comp.Init();

        CmdSetTeam(team);

        if (LocalInfo.Observer != null && team != Team.None) LocalInfo.Observer.RemoveObserver();
    }

    Vector3 getSpawnPosition(Team team, int seed)
    {
        switch (team)
        {
            case Team.CT:
                return CT_Spawner.transform.GetChild(Math.Clamp(seed, 0, CT_Spawner.transform.childCount)).position;
            case Team.T:
                return T_Spawner.transform.GetChild(Math.Clamp(seed, 0, T_Spawner.transform.childCount)).position;
        }
        return default(Vector3);
    }

}


