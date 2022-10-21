using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerPlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool Grounded;
    private bool isJumping;
    private bool m_FacingRight = true; // For determining which way the player is currently facing.

    private Animator anim;

    [SerializeField] private float MovementSpeed;
    [SerializeField] private float JumpSpeed;
    private float RayLength; // made hidden for inspector
    private float RayPositionOffset; // made hidden for inspector

    Vector2 RayPositionCenter;
    Vector2 RayPositionLeft;
    Vector2 RayPositionRight;

    RaycastHit2D[] GroundHitsCenter;
    RaycastHit2D[] GroundHitsLeft;
    RaycastHit2D[] GroundHitsRight;
    RaycastHit2D[][] AllRaycastHits = new RaycastHit2D[3][];

    CapsuleCollider2D capsuleComponent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleComponent = this.GetComponent<CapsuleCollider2D>();

    }

    private void Update()
    {
        Movement();
        Jump();
    }

    private void Movement()
    {
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            rb.velocity =
                new Vector2(MovementSpeed * Time.fixedDeltaTime, rb.velocity.y);
            anim.SetBool("isWalking", true);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            rb.velocity =
                new Vector2(-MovementSpeed * Time.fixedDeltaTime,
                    rb.velocity.y);
            anim.SetBool("isWalking", true);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            anim.SetBool("isWalking", false);
        }

        if (Input.GetKey(KeyCode.Space) && Grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
        }

        if (Grounded)
        {
            anim.SetBool("isJumping", false);
        }
        else
        {
            anim.SetBool("isJumping", true);
        }

        // If the input is moving the player right and the player is facing left...
        if (rb.velocity.x > 0 && !m_FacingRight)
        {
            // ... flip the player.
            Flip();
        } // Otherwise if the input is moving the player left and the player is facing right...
        else if (rb.velocity.x < 0 && m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }
    }

    public void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector2 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    private void Start() //Declare RayPositionOffset & Raylength and write them to console
    {
        RayPositionOffset = capsuleComponent.size.x * (float)0.5; // removed redundunt "+ (float)0.05"
        RayLength = (float)capsuleComponent.size.y * (float)0.5 + (float)0.05;
        Debug.Log("RayLength = " + RayLength);
        Debug.Log("RayPositionOffset = " + RayPositionOffset);
    }

    private void Jump()
    {
        var position = transform.position;


        // Moved to Start() so it is only declared once
        //RayPositionOffset = capsuleComponent.size.x * (float)0.5; // removed redundunt "+ (float)0.05"
        //RayLength = (float)capsuleComponent.size.y * (float)0.5 + (float)0.05;


        // Old RayPositions
        //RayPositionLeft = new Vector2(position.x - RayPositionOffset, position.y);
        //RayPositionRight = new Vector2(position.x + RayPositionOffset, position.y);


        // New RayPositions >> takes offset for dynamic heigth
        // Also added RayPositionCenter
        RayPositionCenter = new Vector2(position.x, position.y + (float)capsuleComponent.offset.y); // added
        RayPositionLeft = new Vector2(position.x - RayPositionOffset, position.y + (float)capsuleComponent.offset.y); //Dynamic position.y
        RayPositionRight = new Vector2(position.x + RayPositionOffset, position.y + (float)capsuleComponent.offset.y); //Dynamic position.y

        GroundHitsCenter = Physics2D.RaycastAll(position, -Vector2.up, RayLength);
        GroundHitsLeft = Physics2D.RaycastAll(RayPositionLeft, -Vector2.up, RayLength);
        GroundHitsRight = Physics2D.RaycastAll(RayPositionRight, -Vector2.up, RayLength);

        AllRaycastHits[0] = GroundHitsCenter;
        AllRaycastHits[1] = GroundHitsLeft;
        AllRaycastHits[2] = GroundHitsRight;

        Grounded = GroundCheck(AllRaycastHits);

        Debug.DrawRay(RayPositionCenter, -Vector2.up * RayLength, Color.red);
        Debug.DrawRay(RayPositionLeft, -Vector2.up * RayLength, Color.red);
        Debug.DrawRay(RayPositionRight, -Vector2.up * RayLength, Color.red);
    }

    private bool GroundCheck(RaycastHit2D[][] GroundHits)
    {
        foreach (RaycastHit2D[] HitList in GroundHits)
        {
            foreach (RaycastHit2D hit in HitList)
            {
                if (hit.collider != null)
                {
                    if (hit.transform != transform)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
