using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSlot : MonoBehaviour
{
    public bool DisableKickBttn = false;
    private void OnEnable()
    {
        if (LobbyManager.Singleton.lobbyState == LobbyState.Client || DisableKickBttn)
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else if(LobbyManager.Singleton.lobbyState == LobbyState.Host)
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Sets the name of the Slot Text
    /// </summary>
    public string Name
    {
        set => transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value;
    }
    public ExtendedRoomPlayer Player;
    /// <summary>
    /// Used to Kick the client associated with this slot
    /// </summary>
    public void KickClient() =>
        LobbyManager.Singleton.LocalRoomPlayer.CmdKickPlayer(Player.netIdentity);
}
