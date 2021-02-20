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

[System.Serializable] public class SurfaceDictionary : SerializableDictionary<string, SurfaceData> { }

[System.Serializable]
public class SurfaceData
{
    public GameObject Decal;
    public GameObject CollisionParticle;
    public float DamageModifier;
    //Sound
}