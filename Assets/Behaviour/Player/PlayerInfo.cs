using Mirror;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;
using System.Linq;

public class PlayerInfo : NetworkBehaviour, IDamageable
{
    [SerializeField, Range(0, 100), SyncVar] private float health = 100;
    public float hp { get => health; }
    [SyncVar] public string Name;

    public SyncList<Offender> DamageRegistry = new SyncList<Offender>();

    public void damage(float amount, GameObject offender)
    {
        CmdDamage(amount, offender);
    }
    private void Start()
    {
        if (!isLocalPlayer) return;
        CmdSetName(LobbyManager.Singleton.LocalRoomPlayer.ClientName);
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        LocalInfo.PlayerHealth = hp;
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
        health -= amount > 0f ? amount : 0f;
        Register(amount, offender);
    }
    #endregion

    #region Utilities 
    void Register(float amount, GameObject offender) => CmdRegister(amount, offender);
    [Command(ignoreAuthority = true)]
    void CmdRegister(float amount, GameObject offender)
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

    public Offender() {}
    public Offender(float amount, GameObject offender)
    {
        NetworkIdentity identity = offender.GetComponent<NetworkIdentity>();
        netId = identity.netId;
        networkIdentity = identity;
        Name = $"Client[{netId}]";
        gameObject = offender;
        
        Damage = amount;
    }
    public static explicit operator string(Offender off) => off.ToString();
    public override string ToString()
    {
        return $"{Name} (-{Damage})";
    }
}
