using Unity.Flayer.InputSystem;
using UnityEngine;
using static UnityEngine.Mathf;

public class CustomPlayerMovement : Mirror.NetworkBehaviour
{
    public CharacterController controller;

    [Header("Movement")]
    public float speed = 10f;
    public float gravity = -25f;
    public float jumpForce = 2f;

    [Header("Crouch")]
    public float UprightHeight = 1.8f;
    public float CrouchedHeight = .9f;
    public float HeightTransitionSpeed = .1f;
    public float StaminaLossPerCrouch = .5f;
    public float StaminaGainPerSecond = .33f;

    [Header("Others")]
    public GameObject groundCheckSphere;
    public float AnimationSpeedMultiplier = 3f;
    public float CrouchSpeedMultiplier = 5f;

    [Header("Runtime")]
    public bool isGrounded = false;
    public Vector3 velocity;
    public float HeightBuffer = 1.8f;
    public float stamina = 1f;

    Vector3 LastLocation = Vector3.zero;

    void Update()
    {
        if (!isLocalPlayer) return;
        float x = Input.GetAxis("Horizontal");      // X- = A ,X+ = D
        float z = Input.GetAxis("Vertical");        // Z- = S ,Z+ = W

        Vector3 move = transform.right * x + transform.forward * z;
        if (move.magnitude > 1) move.Normalize();
        controller.Move(move.normalized * move.magnitude * speed * Time.deltaTime);

        if (InputManager.GetBindDown("Jump") && isGrounded && !LocalInfo.IsPaused)
        {
            velocity.y = Sqrt(jumpForce * -2 * gravity);
            if (CtrlAnimationMovementParameters.Singleton != null) CtrlAnimationMovementParameters.Singleton.Jump = true;
        }
        controller.Move(velocity * Time.deltaTime);

        if (InputManager.GetBindDown("Crouch")) stamina -= StaminaLossPerCrouch;

        if (InputManager.GetBind("Crouch"))
        {
            HeightBuffer = Clamp(HeightBuffer - ((CrouchSpeedMultiplier * 5) * Time.deltaTime * stamina), CrouchedHeight, UprightHeight);
            stamina = Clamp01(stamina + (StaminaGainPerSecond / 3) * Time.deltaTime);
        }
        else
        {
            HeightBuffer = Clamp(HeightBuffer + ((CrouchSpeedMultiplier * 5) * Time.deltaTime), CrouchedHeight, UprightHeight);
            stamina = Clamp01(stamina + StaminaGainPerSecond * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer) return;
        if (CtrlAnimationMovementParameters.Singleton != null)
        {
            //Planar Movement
            var vec = ((transform.position - LastLocation) / Time.deltaTime);
            var vecmag = vec.magnitude / 10;
            var angle = Atan2(vec.x, vec.z) - (transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
            var movDir = new Vector2(Cos(angle), Sin(angle)) * (vecmag > .01 ? vecmag : 0);
            //var moveDir = new Vector2((transform.right * vec.x).x, (transform.forward * vec.z).z);
            /*
            var angle = Vector3.SignedAngle(transform.position, vec, Vector3.up);
            var movDir = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.zero;
            */
            CtrlAnimationMovementParameters.Singleton.MoveDirection = movDir;
            CtrlAnimationMovementParameters.Singleton.MoveSpeed = vec.magnitude * (AnimationSpeedMultiplier / 10);
            CtrlAnimationMovementParameters.Singleton.PlayerAlt = GenericUtilities.ToPercent01(CrouchedHeight, UprightHeight, HeightBuffer);
            //CtrlAnimationMovementParameters.Singleton.Grounded = isGrounded;

            LastLocation = transform.position;


        }
        else if (JumpParameterMachine.Singleton != null) JumpParameterMachine.Singleton.Grounded = isGrounded;
    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        if (groundCheckSphere.GetComponent<GroundTracer>().isGrounded)
        {
            velocity.y /= 1.1f;
            isGrounded = true;
        }
        else
        {
            velocity.y += gravity * Time.fixedDeltaTime;
            isGrounded = false;
        }
    }
}
