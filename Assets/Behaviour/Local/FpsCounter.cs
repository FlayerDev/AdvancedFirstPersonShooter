using System.Collections;
using UnityEngine;
using Mirror;
public class FpsCounter : MonoBehaviour
{
    string label = "";
    float count;
    public int updateDelay = 1;
    public UnityEngine.UI.Text text;

    void Update()
    {
        if (Time.timeScale == 1)
        {
            count = (1 / Time.deltaTime);
            label = $"FPS: {Mathf.Round(count)}";
        }
        else
        {
            label = "Pause";
        }
        if (LobbyManager.Singleton.lobbyState != LobbyState.None)
        {
            int Ping = Mathf.FloorToInt((float)NetworkTime.rtt * 1000);
            label += Ping == 0 ? " Host" : $" Ping: {Ping}ms";
        }
        text.text = label;
    }
}
