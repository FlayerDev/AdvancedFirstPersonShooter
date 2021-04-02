using Mirror;
using NobleConnect.Mirror;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

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
        Lobby_Leave();
    }
    public override void OnRoomClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("RoomClientLeave");
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
