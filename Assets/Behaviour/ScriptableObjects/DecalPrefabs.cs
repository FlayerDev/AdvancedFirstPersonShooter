using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flayer/DecalPrefabs", fileName = "DecalPrefabs")]
public class DecalPrefabs : ScriptableObject
{
    public GameObject defaultDecal;
    [Space]
    public GameObject syntheticDecal;
    public GameObject woodDecal;
    public GameObject metalDecal;
    public GameObject cobbleDecal;
    public GameObject concreteDecal;

    public GameObject decalDictionary(string decal)
    {
        Dictionary<string, GameObject> decalDictionary = new Dictionary<string, GameObject>
        {
        {"SYNTHETIC", syntheticDecal},
        {"WOOD" , woodDecal},
        {"METAL", metalDecal},
        {"COBBLE", cobbleDecal},
        {"CONCRETE", concreteDecal}
        };
        if (decalDictionary.TryGetValue(decal, out GameObject returnObj)) return returnObj;
        else return null;
    }
}
