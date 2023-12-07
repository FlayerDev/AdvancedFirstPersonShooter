using System;
using System.Collections;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public static class LocalInfo
{
    public static bool IsPaused = false;
    public static GameObject muzzle { get => GameObject.Find("WorldCamera").transform.GetChild(0).gameObject; }
    public static float PlayerHealth = 100f;
    public static PlayerInfo activePlayerInfo;
    public static GameObject Observer;
    public static Mirror.NetworkIdentity localIdentity;
}