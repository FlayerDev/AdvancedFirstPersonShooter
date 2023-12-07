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
        Instantiate( team == 0? LobbyManager.Singleton.CT_Player_Prefab: LobbyManager.Singleton.T_Player_Prefab, GetSpawnPosition((Team)team, 1), Quaternion.identity);
        CmdSelectTeam(team);
        LocalInfo.Observer.SetActive(false);
        TeamSelectionHUD.SetActive(false);
    }

    [Command]
    public void CmdSelectTeam(int team)
    {
        LocalInfo.localIdentity.gameObject.GetComponent<PlayerInfo>().playerTeam = team;
    }

    public Vector3 GetSpawnPosition(Team team, int seed)
    {
        switch (team)
        {
            case Team.CT:
                return CT_Spawner.transform.GetChild(Math.Clamp( seed,0, CT_Spawner.transform.childCount)).position;
            case Team.T:
                return T_Spawner.transform.GetChild(Math.Clamp(seed, 0, T_Spawner.transform.childCount)).position;
        }
        return default(Vector3);
    }

}


