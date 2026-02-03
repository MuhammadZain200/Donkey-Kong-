using UnityEngine;

public class Player : MonoBehaviour
{

    //Sprites
    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;


    private int spriteIndex;
    public float speed = 5f;
    public float jumpForce = 7f;
    private float moveInput;

    private Rigidbody2D rb;
    private Collider2D collider;
    private Vector2 direction;
    private Collider2D[] results;
    private bool isGrounded;
    private bool climbing;

    private void Start()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        results = new Collider2D[4];
        collider = GetComponent<Collider2D>(); 
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite),1f/12f, 1f/12f);
    }

    public void OnDisable()
    {
        CancelInvoke();
    }

    private void CheckCollision()
    {
        isGrounded = false;
        climbing = false;

        Vector2 size = collider.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;
        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, results);

        for (int i = 0; i < amount; i++)
        {
           GameObject hit = results[i].gameObject;

           if(hit.layer == LayerMask.NameToLayer("Ground"))
            {
                isGrounded = hit.transform.position.y < (transform.position.y - 0.5f);

                Physics2D.IgnoreCollision(collider, results[i], !isGrounded);
            }
            else if (hit.layer == LayerMask.NameToLayer("Ladder")){
                climbing = true;
            }  
        }
        // Handle gravity cleanly
        rb.gravityScale = climbing ? 0f : 1f;
    }

    private void Update()
    {
        CheckCollision();
        // Horizontal input
        moveInput = Input.GetAxisRaw("Horizontal");
        
        
        if (climbing)
        {
            direction.y = Input.GetAxisRaw("Vertical") * speed;
        }
        else
        {
            direction.y = rb.velocity.y;
        }

        // Jump input
        if (isGrounded && Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Flip player based on movement
        if (moveInput < 0)
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        else if (moveInput > 0)
            transform.eulerAngles = Vector3.zero;
    }

    private void FixedUpdate()
    {
        // Horizontal movement
        rb.velocity = new Vector2(moveInput * speed, direction.y);
    }

    // Ground check
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Objective"))
        {
           FindObjectOfType<GameManager>(). LevelComplete();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            FindObjectOfType<GameManager>(). LevelFail();
        }
    
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")){
            isGrounded = false;
        }
    }


    // Sprite Animations
    private void AnimateSprite()
    {
        if (climbing)
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if (Input.GetAxisRaw("Horizontal") != 0f)
        {
            spriteIndex++;

            if (spriteIndex >= runSprites.Length) {
                spriteIndex = 0;
            }
            spriteRenderer.sprite = runSprites[spriteIndex];
        }
    }
}

