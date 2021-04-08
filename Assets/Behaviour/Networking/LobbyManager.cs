using Mirror;
using NobleConnect.Mirror;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : NobleRoomManager
{
    public static LobbyManager Singleton;
    NobleNetworkManager networkManager;
    /// <summary>
    /// Returns the Local ExtendedRoomPlayer
    /// </summary>
    public ExtendedRoomPlayer LocalRoomPlayer
    {
        get
        {
            foreach (var item in roomSlots)
            {
                if (item.isLocalPlayer) return item;
            }
            return null;
        }
    }
    override public void Start()
    {
        base.Start();
        Singleton = this;
        networkManager = (NobleNetworkManager)NetworkManager.singleton;
        networkManager.InitClient();
    }
    public GameObject MainHUD;
    public GameObject LobbyHUD;

    public GameObject nameError;
    public string localName;

    public TMPro.TMP_InputField ipField;
    public TMPro.TMP_InputField portField;
    public InputField nameField;
    public Text ipText;
    public Text portText;
    public string IP = "";
    public string PORT = "";

    public LobbyState lobbyState = LobbyState.None;


    // Update is called once per frame
    override public void Update()
    {
        base.Update();
        if (networkManager.HostEndPoint != null)
        {
            ipText.text = $"IP: {networkManager.HostEndPoint.Address}";
            portText.text = $"PORT: {networkManager.HostEndPoint.Port}";
        }
    }

    public void GUI_Refresh()
    {
        var nameBuffer = nameField.text;
        if (Regex.IsMatch(nameBuffer, "[^A-Za-z1-9С-йс-љ_]") || nameField.text.Length < 4) nameError.SetActive(true);
        else
        {
            localName = nameBuffer;
            nameError.SetActive(false);
        }

        IP = ipField.text;
        PORT = portField.text;
    }
    public void GUI_State(bool isConnected)
    {

        if (SceneManager.GetActiveScene().name != "Lobby")
        {
            MainHUD.SetActive(!isConnected);
            LobbyHUD.SetActive(isConnected);
        }
    }
    public void Lobby_Join()
    {
        GUI_Refresh();
        if (nameError.activeInHierarchy) return;
        networkAddress = IP;
        networkPort = ushort.Parse(PORT);
        StartClient();
        lobbyState = LobbyState.Client;
        GUI_State(true);
    }
    public void Lobby_Host()
    {
        GUI_Refresh();
        if (nameError.activeInHierarchy) return;
        StartHost();
        lobbyState = LobbyState.Host;
        GUI_State(true);
    }
    public void Lobby_Leave()
    {
        Debug.Log("Lobby_Leave");
        if (lobbyState == LobbyState.Client)
        {
            client.Disconnect();
            StopClient();
            lobbyState = LobbyState.None;
        }
        else if (lobbyState == LobbyState.Host)
        {
            try
            {
                //client.Disconnect();
            }catch { }
            NetworkServer.Shutdown();
            NobleServer.Shutdown();
            StopHost();
            lobbyState = LobbyState.None;
        }
        GUI_State(false);
    }
    public void Lobby_Paste()
    {
        string cliptxt = GUIUtility.systemCopyBuffer;
        //if (cliptxt == null || !cliptxt.Contains("_")) return;
        ipField.text = cliptxt.Split('_')[0];
        portField.text = cliptxt.Split('_')[1];
    }
    public void Lobby_Copy()
    {
        GUIUtility.systemCopyBuffer = $"{networkManager.HostEndPoint.Address}_{networkManager.HostEndPoint.Port}";
    }
    public void Play()
    {
        ServerChangeScene(GameplayScene);
    }

    #region overrides
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("ClientLeave");
        returnToLobby();
        try
        {
            Lobby_Leave();
        }
        finally { }
    }
    #region Room Server Virtuals

    /// <summary>
    /// This is called on the host when a host is started.
    /// </summary>
    public override void OnRoomStartHost() { }

    /// <summary>
    /// This is called on the host when the host is stopped.
    /// </summary>
    public override void OnRoomStopHost() { }

    /// <summary>
    /// This is called on the server when the server is started - including when a host is started.
    /// </summary>
    public override void OnRoomStartServer() { }

    /// <summary>
    /// This is called on the server when the server is started - including when a host is stopped.
    /// </summary>
    public override void OnRoomStopServer() { }

    /// <summary>
    /// This is called on the server when a new client connects to the server.
    /// </summary>
    /// <param name="conn">The new connection.</param>
    public override void OnRoomServerConnect(NetworkConnection conn) { }

    /// <summary>
    /// This is called on the server when a client disconnects.
    /// </summary>
    /// <param name="conn">The connection that disconnected.</param>
    public override void OnRoomServerDisconnect(NetworkConnection conn) { }

    /// <summary>
    /// This is called on the server when a networked scene finishes loading.
    /// </summary>
    /// <param name="sceneName">Name of the new scene.</param>
    public override void OnRoomServerSceneChanged(string sceneName) { }

    /// <summary>
    /// This allows customization of the creation of the room-player object on the server.
    /// <para>By default the roomPlayerPrefab is used to create the room-player, but this function allows that behaviour to be customized.</para>
    /// </summary>
    /// <param name="conn">The connection the player object is for.</param>
    /// <returns>The new room-player object.</returns>
    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn)
    {
        return null;
    }

    /// <summary>
    /// This allows customization of the creation of the GamePlayer object on the server.
    /// <para>By default the gamePlayerPrefab is used to create the game-player, but this function allows that behaviour to be customized. The object returned from the function will be used to replace the room-player on the connection.</para>
    /// </summary>
    /// <param name="conn">The connection the player object is for.</param>
    /// <param name="roomPlayer">The room player object for this connection.</param>
    /// <returns>A new GamePlayer object.</returns>
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        return null;
    }

    /// <summary>
    /// This allows customization of the creation of the GamePlayer object on the server.
    /// <para>This is only called for subsequent GamePlay scenes after the first one.</para>
    /// <para>See <see cref="OnRoomServerCreateGamePlayer(NetworkConnection, GameObject)"/> to customize the player object for the initial GamePlay scene.</para>
    /// </summary>
    /// <param name="conn">The connection the player object is for.</param>
    public override void OnRoomServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
    }

    // for users to apply settings from their room player object to their in-game player object
    /// <summary>
    /// This is called on the server when it is told that a client has finished switching from the room scene to a game player scene.
    /// <para>When switching from the room, the room-player is replaced with a game-player object. This callback function gives an opportunity to apply state from the room-player to the game-player object.</para>
    /// </summary>
    /// <param name="conn">The connection of the player</param>
    /// <param name="roomPlayer">The room player object.</param>
    /// <param name="gamePlayer">The game player object.</param>
    /// <returns>False to not allow this player to replace the room player.</returns>
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        return true;
    }

    /// <summary>
    /// This is called on the server when all the players in the room are ready.
    /// <para>The default implementation of this function uses ServerChangeScene() to switch to the game player scene. By implementing this callback you can customize what happens when all the players in the room are ready, such as adding a countdown or a confirmation for a group leader.</para>
    /// </summary>
    public override void OnRoomServerPlayersReady()
    {
        // all players are readyToBegin, start the game
        ServerChangeScene(GameplayScene);
    }
    #endregion
    #region Room Clieant Virtuals
    /// <summary>
    /// This is a hook to allow custom behaviour when the game client enters the room.
    /// </summary>
    public override void OnRoomClientEnter() { }

    /// <summary>
    /// This is a hook to allow custom behaviour when the game client exits the room.
    /// </summary>
    public override void OnRoomClientExit() { }

    /// <summary>
    /// This is called on the client when it connects to server.
    /// </summary>
    /// <param name="conn">The connection that connected.</param>
    public override void OnRoomClientConnect(NetworkConnection conn) { }

    /// <summary>
    /// This is called on the client when disconnected from a server.
    /// </summary>
    /// <param name="conn">The connection that disconnected.</param>
    public override void OnRoomClientDisconnect(NetworkConnection conn) 
    {
        Debug.Log("RoomClientLeave");
        Lobby_Leave();
        returnToLobby();
    }

    /// <summary>
    /// This is called on the client when a client is started.
    /// </summary>
    /// <param name="roomClient">The connection for the room.</param>
    public override void OnRoomStartClient() { }

    /// <summary>
    /// This is called on the client when the client stops.
    /// </summary>
    public override void OnRoomStopClient() { }

    /// <summary>
    /// This is called on the client when the client is finished loading a new networked scene.
    /// </summary>
    /// <param name="conn">The connection that finished loading a new networked scene.</param>
    public override void OnRoomClientSceneChanged(NetworkConnection conn) { }

    /// <summary>
    /// Called on the client when adding a player to the room fails.
    /// <para>This could be because the room is full, or the connection is not allowed to have more players.</para>
    /// </summary>
    public override void OnRoomClientAddPlayerFailed() { }
    #endregion
    #endregion
    private void returnToLobby()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "Lobby")
        {
            SceneManager.LoadScene("Lobby");
            SceneManager.UnloadSceneAsync(scene);
            Destroy(this);
        }
    }
}
public enum LobbyState
{
    None,
    Client,
    Host,
}
