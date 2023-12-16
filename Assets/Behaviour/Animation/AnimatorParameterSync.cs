using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AnimatorParameterSync : NetworkBehaviour, IComponentInitializable
{
    public NetworkAnimator networkAnimator;
    public Animator animator;

    public static AnimatorParameterSync Local { get => NetworkClient.connection.identity.gameObject.GetComponent<AnimatorParameterSync>(); }

    [Header("Upper")]
    public bool Equip = false;
    public bool Reload = false;
    public bool Fire = false;
    public AnimationIndex animIndex;

    [Header("Lower")]
    public float MoveSpeed = 1f;
    Vector2 moveDirection = new Vector2(0, 0);
    [Range(1f, 2f)] public float SmoothDamp = 1.5f;
    public float PlayerAlt = 1f;
    public bool Jump = false;
    public bool Grounded = true;

    public Vector2 MoveDirection
    {
        get => moveDirection; set
        {
            if (moveDirection.sqrMagnitude < .01f && moveDirection.sqrMagnitude > value.sqrMagnitude) moveDirection = Vector2.zero;
            else moveDirection = ((moveDirection + value) / SmoothDamp);
        }
    }

    public void Init()
    {
        Start();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!hasAuthority)
        {
            this.enabled = false;
            return;
        }
        enabled = true;
        animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Y_Velocity", moveDirection.y);
        animator.SetFloat("X_Velocity", moveDirection.x);
        animator.SetInteger("WeaponType", (int)animIndex);
        //animator.SetBool("Grounded", Grounded);
        animator.SetFloat("PlayerAlt", PlayerAlt);
        if (Jump)
        {
            networkAnimator.SetTrigger("Jump");
            Jump = false;
        }        
        if (Fire)
        {
            networkAnimator.SetTrigger("Fire");
            Fire = false;
        }        
        if (Reload)
        {
            networkAnimator.SetTrigger("Reload");
            Reload = false;
        }        
        if (Equip)
        {
            networkAnimator.SetTrigger("Equip");
            Equip = false;
        }

        if (moveDirection.sqrMagnitude < 0.1)
        {
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", MoveSpeed);
        }

        animator.SetBool("Grounded", Grounded);
    }
}
