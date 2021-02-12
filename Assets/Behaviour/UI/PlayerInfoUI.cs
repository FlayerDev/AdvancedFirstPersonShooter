using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfoUI : Mirror.NetworkBehaviour
{
    public Gradient HpGradient;
    public Gradient TextGradient;
    public TMP_Text hpText;
    public GameObject HealthBar;
    public Transform hpBarParent;

    private void Awake()
    {
        if (!isLocalPlayer) return;
        hpBarParent = HealthBar.transform.parent;
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        hpText.text = Mathf.CeilToInt(LocalInfo.PlayerHealth).ToString();
        hpText.color = TextGradient.Evaluate(GenericUtilities.ToPercent01(0, 100, LocalInfo.PlayerHealth));
        HealthBar.GetComponent<Image>().color = HpGradient.Evaluate(GenericUtilities.ToPercent01(0, 100, LocalInfo.PlayerHealth));
        hpBarParent.localScale = new Vector3(Mathf.Clamp(LocalInfo.PlayerHealth, 0, 100) / 100, hpBarParent.localScale.y, hpBarParent.localScale.z);
    }
}
