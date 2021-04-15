using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTracer : MonoBehaviour
{
    public float minimumDamageSpeed = 15f;
    public float fallDamageMultiplier = 1f;
    public bool isGrounded = false;
    float verticalVelocity = 0;
    CustomPlayerMovement mov;

    private void Awake() => mov = transform.parent.GetComponent<CustomPlayerMovement>();
    private void Update() => verticalVelocity = mov.PlanarMovement.y != 0 ? mov.PlanarMovement.y : verticalVelocity;
    private void OnTriggerStay(Collider other) => isGrounded = true;

    private async void OnTriggerExit(Collider other) {
        var mov = transform.parent.GetComponent<CustomPlayerMovement>();
        if (mov.velocity.y <= 0) mov.velocity.y = 0f;//Mathf.Clamp(mov.velocity.y - 5 * Time.fixedDeltaTime, -10, 10);

        await System.Threading.Tasks.Task.Delay(100);
        isGrounded = false;
    }

    private void OnTriggerEnter(Collider other) {
        var mov = transform.parent.GetComponent<CustomPlayerMovement>();

        if (verticalVelocity < -minimumDamageSpeed)
            damagePlayer((Mathf.Abs(verticalVelocity) - minimumDamageSpeed) * fallDamageMultiplier);
    }

    void damagePlayer(float amount)
    {
        verticalVelocity = 0;
        if (!transform.parent.GetComponent<Mirror.NetworkIdentity>().isLocalPlayer) return;
        transform.parent.GetComponent<IDamageable>().damage(amount, transform.parent.gameObject);
    }
}
