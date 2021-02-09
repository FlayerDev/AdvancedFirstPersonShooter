using UnityEngine;

[CreateAssetMenu(menuName = "Flayer/SurfacePropertyLibrary", fileName = "SurfacePropertyLibrary")]
public class SurfacePropertyLibrary : ScriptableObject
{
    [SerializeField]
    SurfaceDictionary SurfaceDictionary;
    public SurfaceData Default { get
        {
            SurfaceDictionary.TryGetValue("Default", out var def);
            return def;
        } 
    }

    public SurfaceData getSurfaceData(string decal)
    {
        if (SurfaceDictionary.TryGetValue(decal, out SurfaceData returnObj)) return returnObj;
        else return Default;
    }
}
#if UNITY_EDITOR
[System.Serializable] public class SurfaceDictionary : SerializableDictionary<string, SurfaceData> { }
#else
[System.Serializable] public class SurfaceDictionary : System.Collections.Generic.Dictionary<string, SurfaceData> { }
#endif
[System.Serializable]
public class SurfaceData
{
    public GameObject Decal;
    public GameObject CollisionParticle;
    public float DamageModifier;
    //Sound
}