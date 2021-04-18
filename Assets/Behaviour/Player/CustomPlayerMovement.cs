using Unity.Flayer.InputSystem;
using UnityEngine;
using Mirror;
using static UnityEngine.Mathf;

public class CustomPlayerMovement : NetworkBehaviour
{
    public CharacterController controller;
    public AnimatorParameterSync anim;

    [Header("Movement")]
    public float speed = 10f;
    public float gravity = -25f; 
    public float jumpForce = 1.2f; //NOTE jumpForce: Subject to change

    [Header("Crouch")]
    public float UprightHeight = 1.8f;
    public float CrouchedHeight = .9f;
    public float HeightTransitionSpeed = .1f;
    public float StaminaLossPerCrouch = .5f;
    public float StaminaGainPerSecond = .33f;
    [Range(0f,1f)]public float CrouchedPlayerSpeedFactor = .4f;

    [Header("Others")]
    public GameObject groundCheckSphere;
    public float AnimationSpeedMultiplier = 3f;
    public float CrouchSpeedMultiplier = .5f;

    [Header("Runtime")]
    public bool isGrounded = false;
    public Vector3 velocity;
    [SyncVar]public float HeightBuffer = 1.8f;
    public float stamina = 1f;
    internal Vector3 PlanarMovement = Vector3.zero;

    Vector3 LastLocation = Vector3.zero;
    private void Start()
    {
        if (!isLocalPlayer) return;
        anim = GetComponent<AnimatorParameterSync>();
        groundTracer = groundCheckSphere.GetComponent<GroundTracer>();
        LocalInfo.localIdentity = base.netIdentity;
    }
    void Update()
    {
        if (!isLocalPlayer) return;
        float x = Input.GetAxis("Horizontal");      // X- = A ,X+ = D
        float z = Input.GetAxis("Vertical");        // Z- = S ,Z+ = W

        Vector3 move = transform.right * x + transform.forward * z;
        if (move.magnitude > 1) move.Normalize();
        controller.Move(move.normalized * move.magnitude *
            Lerp(speed * CrouchedPlayerSpeedFactor, speed, GenericUtilities.ToPercent01(CrouchedHeight, UprightHeight, HeightBuffer)) * Time.deltaTime);

        if (InputManager.GetBindDown("Jump") && isGrounded && !LocalInfo.IsPaused)
        {
            velocity.y = Sqrt(jumpForce * -2 * gravity);
            if (anim != null) anim.Jump = true;
        }
        controller.Move(velocity * Time.deltaTime);

        if (InputManager.GetBindDown("Crouch")) stamina -= StaminaLossPerCrouch;

        if (InputManager.GetBind("Crouch"))
        {
            SetHeightBuffer(Clamp(HeightBuffer - ((CrouchSpeedMultiplier * 5) * Time.deltaTime * stamina), CrouchedHeight, UprightHeight));
            stamina = Clamp01(stamina + (StaminaGainPerSecond / 3) * Time.deltaTime);
        }
        else
        {
            SetHeightBuffer(Clamp(HeightBuffer + ((CrouchSpeedMultiplier * 5) * Time.deltaTime), CrouchedHeight, UprightHeight));
            stamina = Clamp01(stamina + StaminaGainPerSecond * Time.deltaTime);
        }
    }
    [Command]
    void SetHeightBuffer(float val)
    {
        HeightBuffer = val;
    }
    private GroundTracer groundTracer;
    private void FixedUpdate()
    {
        controller.height = HeightBuffer;
        controller.center = new Vector3(0, HeightBuffer / 2, 0);
        if (!isLocalPlayer) return;
        if (groundTracer.isGrounded)
        {
            if (velocity.y <= 0) velocity.y = Clamp(velocity.y - 5 * Time.fixedDeltaTime, -10, 10);
            isGrounded = true;
        }
        else
        {
            velocity.y += gravity * Time.fixedDeltaTime;
            isGrounded = false;
        }
        if (anim != null)
        {
            //Planar Movement
            PlanarMovement = ((transform.position - LastLocation) / Time.deltaTime);
            var vecmag = PlanarMovement.magnitude / 10;
            var angle = Atan2(PlanarMovement.x, PlanarMovement.z) - (transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
            var movDir = new Vector2(Cos(angle), Sin(angle)) * (vecmag > .01 ? vecmag : 0);
            anim.MoveDirection = movDir;
            anim.MoveSpeed = PlanarMovement.magnitude * (AnimationSpeedMultiplier / 10);
            anim.PlayerAlt = GenericUtilities.ToPercent01(CrouchedHeight, UprightHeight, HeightBuffer);
            anim.Grounded = isGrounded;

            LastLocation = transform.position;
        }
    }
}
