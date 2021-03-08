using Mirror;
using NobleConnect.Mirror;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : NobleRoomManager
{
    public static LobbyManager Singleton;
    NobleNetworkManager networkManager;
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
        if (Regex.IsMatch(nameBuffer, "[^A-Za-z1-9_]") || nameField.text.Length < 4) nameError.SetActive(true);
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
        MainHUD.SetActive(!isConnected);
        LobbyHUD.SetActive(isConnected);
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
        if (lobbyState == LobbyState.Client)
        {
            client.Disconnect();
            //client.connection.Disconnect();
            lobbyState = LobbyState.None;
        }
        else if (lobbyState == LobbyState.Host)
        {
            client.Disconnect();
            NetworkServer.Shutdown();
            NobleServer.Shutdown();
            lobbyState = LobbyState.None;
        }
        GUI_State(false);
    }
    #region overrides
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Lobby_Leave();
    }
    public override void OnRoomClientDisconnect(NetworkConnection conn)
    {
        Lobby_Leave();
    }
    #endregion
}
public enum LobbyState
{
    None,
    Client,
    Host,
}
