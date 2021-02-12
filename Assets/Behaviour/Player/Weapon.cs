using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Flayer.InputSystem;
[RequireComponent(typeof(Item))]
public class Weapon : WeaponBehaviour
{
    #region Properties
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
    public Mag mag;
    #endregion
    #endregion
    #region others
    private Action update;
    //private Thread fireThread;
    #endregion
    private void Awake()
    {
        if (!isLocalPlayer) return;
        mag = GetComponent<Mag>();
        mag.ammo = mag.Capacity;
        muzzle = LocalInfo.muzzle;
        update += isWeaponAutomatic
            ? update += () => { if (InputManager.GetBind("Primary")) fire(); }
        : () => { if (InputManager.GetBindDown("Primary")) fire(); };
        //if (allowADS) update += () => { // };
    }
    public void Update() 
    {
        if (!isLocalPlayer) return;
        update();
    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
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
        if (mag.ammo > 0) mag.ammo--; else return;
        rearm();
        float dmg = baseDamage;
        calculateRecoil();
        RaycastHit[] hitarr = Physics.RaycastAll(muzzle.transform.position,
            muzzle.transform.forward + new Vector3(0f, currentRecoil.x, currentRecoil.y), effectiveRange);

        Array.Sort(hitarr, (x, y) => x.distance.CompareTo(y.distance)); // Sorts hit objects by distance
        foreach (RaycastHit item in hitarr)
        {
            if (item.collider.gameObject.TryGetComponent(out IDamageable dmgable)) applyDamage(dmgable, dmg);
            //if (item.collider.gameObject.TryGetComponent(out Rigidbody rb)) rb.AddForce((rb.position - muzzle.transform.position).normalized * bulletWeight);
            dmg = calculateDamage(dmg, item, muzzle, effectiveRange, PenetrationPower);
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
    void applyDamage(IDamageable dmgable, float amount)
    {
        dmgable.damage(amount, gameObject);
    }
    //static RaycastHit findOppositeSide(Ray ray, GameObject gO)
    //{
    //    var res = Physics.RaycastAll(ray, 9999f);
    //    foreach (RaycastHit item in res) if (item.collider.gameObject == gO) return item;
    //    return new RaycastHit();
    //}
    #endregion

}

