using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTracer : MonoBehaviour
{
    public float minimumDamageSpeed = 15f;
    public float fallDamageMultiplier = 1f;
    public bool isGrounded = false;

    private async void OnTriggerExit(Collider other) {
        var mov = transform.parent.GetComponent<CustomPlayerMovement>();
        if (mov.velocity.y <= 0) mov.velocity.y = Mathf.Clamp(mov.velocity.y - 5 * Time.fixedDeltaTime, -10, 10);

        await System.Threading.Tasks.Task.Delay(100);
        isGrounded = false;
    }

    private void OnTriggerStay(Collider other) => isGrounded = true;

    private void OnTriggerEnter(Collider other) {
        if (transform.parent.GetComponent<CustomPlayerMovement>().velocity.y < -minimumDamageSpeed)
            damagePlayer((Mathf.Abs(transform.parent.GetComponent<CustomPlayerMovement>().velocity.y) - minimumDamageSpeed) * fallDamageMultiplier);
    }
    void damagePlayer(float amount)
    {
        if (!transform.parent.GetComponent<Mirror.NetworkIdentity>().isLocalPlayer) return;
        transform.parent.GetComponent<IDamageable>().damage(amount, transform.parent.gameObject);
    }
}
