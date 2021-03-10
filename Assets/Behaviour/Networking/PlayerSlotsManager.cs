using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlotsManager : MonoBehaviour
{
    private void OnEnable()
    {
        //disables all slots for visual purposes 
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);

        InvokeRepeating(nameof(Refresh), 1, .2f);
    }
    private void OnDisable()
    {
        CancelInvoke();
    }
    /// <summary>
    /// Recalculates and binds all slots to the corresponding clients 
    /// </summary>
    public void Refresh()
    {
        Debug.Log("lobby refreshed");
        List<ExtendedRoomPlayer> plList = LobbyManager.Singleton.roomSlots;
        for (int i = 0; i < plList.Count; i++)
        {
            Transform slot = gameObject.transform.GetChild(i);
            slot.gameObject.SetActive(true);
            PlayerSlot playerSlot = slot.GetComponent<PlayerSlot>();
            playerSlot.Name = plList[i].ClientName;
            playerSlot.Player = plList[i];
        }
        for (int i = plList.Count; i < transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
