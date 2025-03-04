using UnityEngine;

public class Player : MonoBehaviour

{
    
    public float speed = 5f;
    public float jumpForce = 7f;
   

    public Rigidbody2D rb;
    public Animator animator;

    void Update()
    {
        
        animator.SetBool("walk", Input.GetAxisRaw("Horizontal") != 0);
    
        rb.linearVelocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, rb.linearVelocity.y);
                
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("jump", true);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }else if (Input.GetKeyUp(KeyCode.Space)){

animator.SetBool("jump", false);
        }
    }
}
