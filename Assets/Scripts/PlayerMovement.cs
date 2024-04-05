using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpTime; 

    private float horizontalInput; 
    Rigidbody2D rb;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask ground;

    private bool canLongJump = true; 
    public bool isFacingRight = true; 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); 
        Jump(); 
        Flip(); 

        Debug.Log(GroundCheck()); 
        
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * movementSpeed, rb.velocity.y); //Horizontal Movement
        
    }

    private bool GroundCheck()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground); 
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && GroundCheck()) //checks if on the ground to jump
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            StartCoroutine(IsJumping()); 
            Debug.Log("Jumping"); 
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f && canLongJump == true) //enables player to hold down space to jump longer
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f); 
        }

        

    }

    private IEnumerator IsJumping()
    {
        yield return new WaitForSeconds(jumpTime); 

    }

    private void Flip()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale; 
            localScale.x *= -1f; 
            transform.localScale = localScale;
        }
    }


}
