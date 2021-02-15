using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AnimatorParameterSync : NetworkBehaviour
{
    public NetworkAnimator networkAnimator;
    public Animator animator;

    public float MoveSpeed = 1f;
    Vector2 moveDirection = new Vector2(0, 0);
    [Range(1f, 2f)] public float SmoothDamp = 1.5f;
    public float PlayerAlt = 1f;
    //public bool Grounded = true;
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

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        animator.SetFloat("Y_Velocity", moveDirection.y);
        animator.SetFloat("X_Velocity", moveDirection.x);
        //animator.SetBool("Grounded", Grounded);
        animator.SetFloat("PlayerAlt", PlayerAlt);
        if (Jump)
        {
            networkAnimator.SetTrigger("Jump");
            Jump = false;
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
