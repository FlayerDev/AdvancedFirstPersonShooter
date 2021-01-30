﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Flayer.InputSystem;
[RequireComponent(typeof(Item))]
public class Weapon : MonoBehaviour
{
    #region Properties
    #region Decals
    [Header("Decals")]
    public GameObject woodDecal;
    public GameObject metalDecal;
    public GameObject cobbleDecal;
    public GameObject syntheticDecal;
    public GameObject concreteDecal;
    public GameObject defaultDecal;
    #endregion
    #region Generic Options
    [Header("Basic Settings")]
    public bool isWeaponAutomatic = true; //While enabled the weapon will automatically fire when the Shoot button is held
    public float baseDamage = 32; // Initial damage of the weapon
    [Range(10f, 2000f)] public float effectiveRange = 500; // Max distance the bullet/Raycast will travel
    public float DistanceDropoff = .1f;// ![TO BE IMPLEMENTED]! <-----------------------------------------------------------------------------------------
    public float PenetrationPower = 1f;// Νeutralizes the wallbang's DamageDropoffPerMaterial
    public float bulletWeight = 1f;
    public bool allowADS = false;
    [SerializeField] int RPM = 200; // Rounds Per Minute: MS between shots = 1000 / (RPM / 60)
    #endregion
    #region Runtime
    [Header("Runtime")]
    [SerializeField] bool isArmed = true; // If enabled weapon will fire upon Fire button click
    GameObject muzzle;
    #endregion
    #region Recoil
    [Header("Recoil")]
    public bool doRecoil = true;
    [Range(.01f, 1f)] public float recoilReturnSpeed = 1f;
    public float VerticalRecoil = .4f;
    public float MaximumVerticalRecoil = 1f;
    public float HorizontalRecoil = .1f;
    public float HorizontalMultiplierOnMaxVertical = 3f;
    public float MaxNegativeHorizontalRecoil = 3f;

    Vector2 currentRecoil = Vector2.zero; // X = Vertical Y = Horizontal
    #endregion
    #region Mag
    [Header("Ammunition")]
    public int ammo;
    public int inventoryCapacity = 90;
    public int magSize = 30;
    public int reloadAmount = 30;
    public float reloadTime = 2500f;
    #endregion
    #endregion
    #region others
    private Action update;
    //private Thread fireThread;
    #endregion
    private void Awake()
    {
        ammo = magSize;
        muzzle = LocalInfo.muzzle;
        update += isWeaponAutomatic
            ? update += () => { if (InputManager.GetBind("Primary")) fire(); }
        : () => { if (InputManager.GetBindDown("Primary")) fire(); };
        //if (allowADS) update += () => { // };
    }
    public void Update() => update();
    private void FixedUpdate()
    {
        currentRecoil.y /= 1f + recoilReturnSpeed * Time.fixedDeltaTime;
        currentRecoil.x /= 1f + recoilReturnSpeed * Time.fixedDeltaTime;
    }
    async void rearm()
    {
        isArmed = false;
        var ms = RPM / 60;
        ms = 1000 / ms;
        await System.Threading.Tasks.Task.Delay(ms);
        isArmed = true;
    }

    public void fire()
    {
        if (!isArmed) return;
        if (ammo > 0) ammo--; else return;
        rearm();
        float dmg = baseDamage;
        calculateRecoil();
        RaycastHit[] hitarr = Physics.RaycastAll(muzzle.transform.position,
            muzzle.transform.forward + new Vector3(0f, currentRecoil.x, currentRecoil.y), effectiveRange);

        Array.Sort(hitarr, (x, y) => x.distance.CompareTo(y.distance)); // Sorts hit objects by distance
        foreach (RaycastHit item in hitarr)
        {
            if (item.collider.gameObject.TryGetComponent(out IDamageable dmgable)) applyDamage(dmgable, dmg);
            if (item.collider.gameObject.TryGetComponent(out Rigidbody rb)) rb.AddForce((rb.position - muzzle.transform.position).normalized * bulletWeight);
            dmg = calculateDamage(dmg, item);
        }
    }
    #region Fire Functions
    void calculateRecoil()
    {
        if (currentRecoil.x < MaximumVerticalRecoil)
        {
            currentRecoil.x += VerticalRecoil / 10;
            currentRecoil.y += HorizontalRecoil / 10;
        }
        else
        {
            currentRecoil.y -= HorizontalRecoil * HorizontalMultiplierOnMaxVertical;
        }
    }
    float calculateDamage(float dmg, RaycastHit hit)
    {
        if (dmg == 0) return 0;
        Vector3 returnPoint = muzzle.transform.position + (hit.point - muzzle.transform.position).normalized * effectiveRange;
        hit.collider.Raycast(new Ray(returnPoint, (muzzle.transform.position - returnPoint).normalized)
            , out RaycastHit outhit, effectiveRange * 2);
        Vector3 inpoint = hit.point; Vector3 outpoint = outhit.point; // Gets coordinates of hit positions
        printBulletDecal(hit, outhit, inpoint, outpoint);
        float dropvalue = 0f;
        if (TryGetComponent(out ObjectValueOverride valueOverride)) dropvalue = valueOverride.wallbangDamageDropoff;
        else if (!DamageDropoffPerMaterial.MaterialValue.TryGetValue(hit.collider.gameObject.tag, out dropvalue)) return 0f;
        dmg -= Vector3.Distance(inpoint, outpoint) * (dropvalue / PenetrationPower);
        dmg = dmg < 999f && dmg > 0f ? dmg : (dmg > 999f ? 999f : 0f);
        return dmg;
    }
    //static RaycastHit findOppositeSide(Ray ray, GameObject gO)
    //{
    //    var res = Physics.RaycastAll(ray, 9999f);
    //    foreach (RaycastHit item in res) if (item.collider.gameObject == gO) return item;
    //    return new RaycastHit();
    //}
    void printBulletDecal(RaycastHit hit, RaycastHit outhit, Vector3 inpoint, Vector3 outpoint)
    {
        if (hit.collider.gameObject.CompareTag("Player")) return;
        var decal = decalDictionary(hit.collider.gameObject.tag) ?? defaultDecal;
        if (TryGetComponent(out ObjectValueOverride valueOverride)) decal = valueOverride.bulletDecal ?? decal;
        Instantiate(decal, inpoint, Quaternion.LookRotation(hit.normal));
        Instantiate(decal, outpoint, Quaternion.LookRotation(outhit.normal));
    }
    void applyDamage(IDamageable dmgable, float amount)
    {
        dmgable.damage(amount, gameObject);
    }
    #endregion
    #region Dictionaries And Enums
    GameObject decalDictionary(string decal)
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
static class DamageDropoffPerMaterial
{
    public static Dictionary<string, float> MaterialValue = new Dictionary<string, float>
    {
        {"SYNTHETIC", 8f},
        {"WOOD" , 16f},
        {"METAL", 32f},
        {"COBBLE", 40f},
        {"CONCRETE", 60f},
        {"Player", 20f}
    };
}

#endregion