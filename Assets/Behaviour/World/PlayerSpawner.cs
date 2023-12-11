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
        CmdSelectTeam(team, connectionToClient as NetworkConnectionToClient);

        if (LocalInfo.Observer != null) LocalInfo.Observer.RemoveObserver();
        TeamSelectionHUD.SetActive(false);
        // TODO: Set player info team
    }

    [Command(ignoreAuthority =true)]
    public void CmdSelectTeam(int team, NetworkConnectionToClient owner)
    {
        GameObject pref = LobbyManager.Singleton.playerPrefab;
        switch (team)
        {
            case 0:
                pref = LobbyManager.Singleton.CT_Player_Prefab;
                break;
            case 1:
                pref = LobbyManager.Singleton.T_Player_Prefab;
                break;
            default:
                pref = LobbyManager.Singleton.playerPrefab;
                break;
        }
        Debug.Log(pref);
        var gO = Instantiate(pref, GetSpawnPosition((Team)team, 1), Quaternion.identity);
        Debug.Log(gO);
        NetworkServer.Spawn(gO, owner);
        //gO.GetComponent<> 
        //NetworkServer.ReplacePlayerForConnection(this.connectionToClient, gO);
    }

    public Vector3 GetSpawnPosition(Team team, int seed)
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


