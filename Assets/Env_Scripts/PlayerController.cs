using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Automatically adds Rigidbody if missing
public class SimplePlayer : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpForce = 20f;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Auto-configure the Rigidbody for you
        rb.gravityScale = 4f; // High gravity for snappy feel
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevents rolling
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        // 1. Movement
        float xInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);

        // 2. Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 3. Variable Jump Height (let go to fall faster)
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    // MAGIC GROUND CHECK: Works on any collider (Floor, Wall, Box)
    // No layers needed.
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Check every point we are touching. If any point is "flat" (pointing up), we are grounded.
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.7f) 
            {
                isGrounded = true;
                return; 
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}