using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed;
    public Rigidbody2D rb;
    public Animator animator;
    public float jumpForce;

    public bool isJumping; 
    public bool isGrounded;

    public Transform checkGround;
    public float groundCheckRadius;
    public LayerMask collisionLayers;
    
    public SpriteRenderer spriteRenderer;
    public PlayerHealth ph;

    private Vector3 velocity = Vector3.zero;
    private float MaxVelY;

    void Update(){

        if(!isGrounded){
            this.MaxVelY += 1;
        }
        else if(isGrounded){
            if(MaxVelY > 600 && Time.time > 10.0f){
                int damage = (int)MaxVelY/30;
                Debug.Log("Chute " + MaxVelY);
                ph.TakeDamage(damage); 
            }
            this.MaxVelY = 0;
        }

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
        }

    }

    void FixedUpdate()
    {

        isGrounded = Physics2D.OverlapCircle(checkGround.position, groundCheckRadius, collisionLayers);

        float horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;


        MovePlayer(horizontalMovement);

        Flip(rb.velocity.x);

        float characterVelocity = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", characterVelocity);

    }

    void MovePlayer(float _horizontalMovement)
    {
        Vector3 targetVelocity = new Vector2(_horizontalMovement,rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);

        if(isJumping == true)
        {
            rb.AddForce(new Vector2(0f, jumpForce));
            isJumping = false;
        }
    }

    void Flip(float _velocity)
    {
        if(_velocity > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if(_velocity < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkGround.position, groundCheckRadius);
    }
}
