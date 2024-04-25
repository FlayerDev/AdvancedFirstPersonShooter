using Mirror;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;
using System.Linq;
using System;

public class PlayerInfo : NetworkBehaviour, IDamageable
{
    [SerializeField, Range(0, 100), SyncVar] private float health = 100;
    public float hp { get => health; }

    [SyncVar] public int expMagAmmo = 0;
    [SyncVar] public int expInvAmmo = 0;
    [Command]
    public void CmdSetExpAmmo(int mag, int inv)
    {
        expInvAmmo = inv;
        expMagAmmo = mag;
    }

    [SyncVar] public bool isPlayerAlive = true;
    public void SetPlayerAliveState(bool state)
    {
        if (state == isPlayerAlive) return;

        if (state) OnPlayerResurrectionEvent();
        else OnPlayerDeathEvent();

        CmdSetPlayerAliveState(state);
    }
    [Command] public void CmdSetPlayerAliveState(bool state)
    {
        isPlayerAlive = state;
    }


    public event Action OnPlayerDeathEvent;
    public event Action OnPlayerResurrectionEvent;

    [SyncVar] public string Name;
    [SyncVar] public int playerTeam;

    public List<Offender> DamageRegistry = new List<Offender>();

    public void damage(float amount, GameObject offender)
    {
        CmdDamage(amount, offender);
    }

    private void Start()
    {
        if (!hasAuthority) return;
        CmdSetName(LobbyManager.Singleton.LocalRoomPlayer.ClientName);
        LocalInfo.activePlayerInfo = this;
        print("LocalInformation:SetPlayer");
    } 
    private void Update()
    {
        if (!hasAuthority) return;
        if (health <= 0 && isPlayerAlive) SetPlayerAliveState(false);
    }

    #region Networking
    [Command]
    void CmdSetName(string name)
    {
        Name = name;
    }
    [Command(ignoreAuthority = true)]
    public void CmdDamage(float amount, GameObject offender)
    {
        var health_buffer = health;
        health -= amount > 0f ? amount : 0f;
        Register(health_buffer - health, offender);
    }
    #endregion

    #region Utilities 
    void Register(float amount, GameObject offender) => CmdRegister(amount, offender);

    [Command(ignoreAuthority = true)]
    void CmdRegister(float amount, GameObject offender)
    {
        localRegister(amount, offender);
    }

    void localRegister(float amount, GameObject offender)
    {
        IEnumerable<Offender> ResultList =
            from item in DamageRegistry
            where item.gameObject == offender
            select item;
        if (ResultList.Count() == 0)
        {
            DamageRegistry.Add(new Offender(amount, offender));
        }
        else
        {
            ResultList.First().Damage += amount;
            if (ResultList.Count() > 1) Debug.LogError("Multiple Offender Instances");
        }
    }

    public float getDamageByPlayer(GameObject player) // TODO: Implement Later
    {
        float damage = 0;
        //foreach (var item in damageHistory) if (item.Item1 == player) damage += item.Item2;
        return damage;
    }
    #endregion
}
[System.Serializable]
public class Offender
{
    public uint netId;
    public NetworkIdentity networkIdentity;
    public string Name;
    public GameObject gameObject;

    public float Damage;

    public Offender() {} // DO NOT USE. It exists only becase unity cant display it in the editor without a def. const.
    public Offender(float amount, GameObject offender)
    {
        NetworkIdentity identity = offender.GetComponent<NetworkIdentity>();
        PlayerInfo playerInfo = offender.GetComponent<PlayerInfo>();
        netId = identity.netId;
        networkIdentity = identity;
        Name = playerInfo.Name;
        gameObject = offender;
        
        Damage = amount;
    }
    public static explicit operator string(Offender off) => off.ToString();
    public override string ToString()
    {
        return $"{Name} (-{Damage})";
    }
}


public enum Team
{
    None, CT, T
}
