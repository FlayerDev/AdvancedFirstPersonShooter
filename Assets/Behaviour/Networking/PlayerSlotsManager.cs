using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlotsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        var plList = LobbyManager.Singleton.roomSlots;
        for (int i = 0; i < plList.Count; i++)
        {
            var slot = gameObject.transform.GetChild(i);
            slot.gameObject.SetActive(true);
            slot.GetComponent<PlayerSlot>().Name = plList[i].ClientName;
        }
        for (int i = plList.Count; i < transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
