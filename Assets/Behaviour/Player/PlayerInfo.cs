using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour, IDamageable
{
    [SerializeField, Range(0, 100), SyncVar] private float health = 100;
    public float hp { get => health; }
    public List<(GameObject, float)> damageHistory = new List<(GameObject, float)>();

    public void damage(float amount, GameObject offender)
    {
        CmdDamage(amount, offender);
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        LocalInfo.PlayerHealth = hp;
    }

    #region Networking
    [Command]
    public void CmdDamage(float amount, GameObject offender)
    {
        health -= amount > 0f ? amount : 0f;
        //TargetDamage(gameObject.GetComponent<NetworkIdentity>().connectionToClient, amount, offender);
    }
    //[TargetRpc]
    //public void TargetDamage(NetworkConnection target, float amount, GameObject offender)
    //{
    //    damage(amount, offender);
    //}
    #endregion

    #region Utilities 
    public float getDamageByPlayer(GameObject player)
    {
        float damage = 0;
        foreach (var item in damageHistory) if (item.Item1 == player) damage += item.Item2;
        return damage;
    }
    #endregion
}
