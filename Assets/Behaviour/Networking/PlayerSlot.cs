using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour
{
    public string Name
    {
        set
        {
            transform.GetChild(0).GetComponent<Text>().text = value;
        }
    }
    public ExtendedRoomPlayer Player;
    public void KickClient()
    {
        try
        {
            Player.connectionToServer.Disconnect();
        }
        catch
        {
            try
            {
                Player.connectionToClient.Disconnect();
            }
            catch { }
        }
    }
}
