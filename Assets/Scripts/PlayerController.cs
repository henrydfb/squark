using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float speed;

    public float jumpImpulse;

    private bool isJumping;

    private bool isInAir;

    private bool isColliding;

	// Use this for initialization
	void Start () 
    {
        isJumping = false;
        isInAir = false;
        isColliding = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        HandleInput();
	}

    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x,transform.position.y - collider2D.bounds.size.y/2), -Vector3.up/2);
        Debug.DrawLine(new Vector3(transform.position.x, transform.position.y - collider2D.bounds.size.y / 2), new Vector3(transform.position.x, transform.position.y - collider2D.bounds.size.y / 2) - Vector3.up/2, Color.white);
        if (hit.collider != null && hit.collider != collider2D)
        {
            if (hit.collider.gameObject.tag == Names.Platform && isColliding)
            {
                isInAir = false;
            }
        }
    }
    
    private void HandleInput()
    {
        rigidbody2D.velocity = new Vector2(Input.GetAxis(Names.HorizontalInput) * speed,rigidbody2D.velocity.y);
        if (Input.GetButtonDown(Names.JumpInput) && !isJumping && !isInAir)
        {
            rigidbody2D.AddForce(new Vector2(0, jumpImpulse));
            isJumping = true;
        }

        if (Input.GetButtonUp(Names.JumpInput) && isJumping)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y/2);
            isJumping = false;
            isInAir = true;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        isColliding = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == Names.Platform)
        {
            isInAir = true;
            isColliding = false;
        }
    }
}
