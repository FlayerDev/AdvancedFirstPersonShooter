using UnityEngine;

public static class LocalInfo
{
    public static GameObject muzzle { get => GameObject.Find("WorldCamera").transform.GetChild(0).gameObject; }

    //UI
    public static bool IsPaused = false;
    public static PlayerInfo activePlayerInfo;
    public static PlayerInfo focusedPlayerInfo;

    //Controls
    public static float Sensitivity = 10f;

    //Info
    public static Observer Observer;
    public static Team localTeam = Team.None;
    public static Mirror.NetworkIdentity localIdentity;
}