using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ExtendedRoomPlayer : NobleRoomPlayer
{
    [Header("PlayerData"), SyncVar]
    public string ClientName;

    LobbyManager lobbyManager = LobbyManager.Singleton;

    public override void OnClientEnterRoom()
    {

    }

    public override void OnClientExitRoom()
    {
        lobbyManager.Lobby_Leave();
    }
    #region Commands and Rpcs
#pragma warning disable IDE0060
    [Command]
    public void CmdKickPlayer(NetworkIdentity id)
    {
        TargetKickPlayer(id.connectionToClient);
    }
    [TargetRpc]
    void TargetKickPlayer(NetworkConnection target)
    {
        LobbyManager.Singleton.Lobby_Leave();
    }
    [Command] public void CmdSetName(string name)
    {
        ClientName = name;
    }
#pragma warning restore IDE0060
    #endregion
    public override void OnStartLocalPlayer()
    {
        if (lobbyManager.lobbyState.Equals(LobbyState.Client))readyToBegin = true;
        CmdSetName(lobbyManager.localName);
    }
    public void Start()
    {
        if (NetworkManager.singleton is NobleRoomManager room)
        {
            // NetworkRoomPlayer object must be set to DontDestroyOnLoad along with NetworkRoomManager
            // in server and all clients, otherwise it will be respawned in the game scene which would
            // have undesireable effects.
            if (room.dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            room.roomSlots.Add(this);
            room.RecalculateRoomPlayerIndices();

            OnClientEnterRoom();
        }
        else
            Debug.LogError("RoomPlayer could not find a NetworkRoomManager. The RoomPlayer requires a NetworkRoomManager object to function. Make sure that there is one in the scene.");
    }
}
