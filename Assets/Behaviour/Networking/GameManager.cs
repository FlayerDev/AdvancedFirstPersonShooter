using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    private void Start()
    {
        if (isServer)
        {
            print("GM:isServer");
        }
        else
        {
            print("GM:!isServer");
        }
    }
}
