using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LocalInfo.Observer = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
