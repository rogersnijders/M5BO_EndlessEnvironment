using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerPlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private bool Grounded;
    private bool isJumping;
    

    private bool m_FacingRight = true;  // For determining which way the player is currently facing.

    private Animator anim;

    [SerializeField] private float MovementSpeed;
    [SerializeField] private float JumpSpeed;

    [SerializeField] private float RayLength;
    [SerializeField] private float RayPositionOffset;

    Vector3 RayPositionCenter;
    Vector3 RayPositionLeft;
    Vector3 RayPositionRight;


    RaycastHit2D[] GroundHitsCenter;
    RaycastHit2D[] GroundHitsLeft;
    RaycastHit2D[] GroundHitsRight;

    RaycastHit2D[][] AllRaycastHits = new RaycastHit2D[3][];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CapsuleCollider2D>();

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
            rb.velocity = new Vector2(MovementSpeed * Time.fixedDeltaTime, rb.velocity.y);
            anim.SetBool("isWalking", true);
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            rb.velocity = new Vector2(-MovementSpeed * Time.fixedDeltaTime, rb.velocity.y);
            anim.SetBool("isWalking", true);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            anim.SetBool("isWalking", false);

        }

        if (Input.GetKey(KeyCode.Space) && Grounded)
        {
          //anim.SetBool("isJumping", false);
          rb.velocity = new Vector2(rb.velocity.x, 0);
          rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);

        }
        if (Grounded) {
          anim.SetBool ("isJumping", false);

          }
          else {
            anim.SetBool ("isJumping", true);
          }

        // If the input is moving the player right and the player is facing left...
        if (rb.velocity.x > 0 && !m_FacingRight)
        {
          // ... flip the player.
          Flip();
        }
          // Otherwise if the input is moving the player left and the player is facing right...
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

    private void Jump()
    {
        RayPositionCenter = transform.position + new Vector3(0, RayLength * .5f, 0);
        RayPositionLeft = transform.position + new Vector3(-RayPositionOffset, RayLength * .5f, 0);
        RayPositionRight = transform.position + new Vector3(RayPositionOffset, RayLength * .5f, 0);

        GroundHitsCenter = Physics2D.RaycastAll(RayPositionCenter, -Vector2.up, RayLength);
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
              if (hit.collider.tag != "PlayerCollider")
              {
                return true;
              }
            }
          }
        }
        return false;
      }

    }
