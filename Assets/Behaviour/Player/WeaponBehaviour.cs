using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    public DecalPrefabs decalPrefabs;
    protected float calculateDamage(float dmg, RaycastHit hit, GameObject muzzle, float effectiveRange, float PenetrationPower)
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
    void printBulletDecal(RaycastHit hit, RaycastHit outhit, Vector3 inpoint, Vector3 outpoint)
    {
        if (hit.collider.gameObject.CompareTag("Player")) return;
        var decal = decalPrefabs.decalDictionary(hit.collider.gameObject.tag) ?? decalPrefabs.defaultDecal;
        if (TryGetComponent(out ObjectValueOverride valueOverride)) decal = valueOverride.bulletDecal ?? decal;
        Instantiate(decal, inpoint, Quaternion.LookRotation(hit.normal));
        Instantiate(decal, outpoint, Quaternion.LookRotation(outhit.normal));
    }

}
