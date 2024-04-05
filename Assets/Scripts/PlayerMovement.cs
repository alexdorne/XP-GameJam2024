using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpTime; 

    private float smoothTime = 0.25f; 
    private Vector3 velocity = Vector3.zero;

    private float horizontalInput; 
    Rigidbody2D rb;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LittleGuyScript littleGuyScript; 

    private bool canLongJump = true; 
    public bool isFacingRight = true; 
    public bool readyToThrow = false; 
    public float guyJumpDelay = 1.0f; 

    public List<GameObject> littleGuys = new List<GameObject>();
    public float distanceBetweenGuys = 1.0f; 

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
        LittleGuyMovement();
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            readyToThrow = !readyToThrow; 
        }

        if (readyToThrow && Input.GetKeyDown(KeyCode.O)){
            MoveLittleGuyToBack(); 
        }

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

    private void LittleGuyMovement()
    {
        for (int i = 0; i < littleGuys.Count ; i++)
        {
            GameObject guy = littleGuys[i];
            

            Vector3 newPosition; 

            
            if (guy == littleGuys[0])
            {
                if (readyToThrow)
                {
                    newPosition = transform.position + (transform.up * distanceBetweenGuys);
                }
                else
                {
                    if (isFacingRight == true)
                    {
                        newPosition = new Vector3(transform.position.x + (-transform.right.x * distanceBetweenGuys * (i + 1)), transform.position.z , transform.position.z);
                    }
                    else
                    {
                        newPosition = new Vector3(transform.position.x + (transform.right.x * distanceBetweenGuys * (i + 1)), transform.position.y, transform.position.z);
                    }
                }
                
            }
            else
            {
                if (readyToThrow)
                {
                    if (isFacingRight == true)
                    {
                        newPosition = new Vector3(transform.position.x + (-transform.right.x * distanceBetweenGuys * (i)), transform.position.y, transform.position.z);
                    }
                    else
                    {
                        newPosition = new Vector3(transform.position.x + (transform.right.x * distanceBetweenGuys * (i)), transform.position.y, transform.position.z);
                    }
                }
                else
                {
                    if (isFacingRight == true)
                    {
                        newPosition = new Vector3(transform.position.x + (-transform.right.x * distanceBetweenGuys * (i + 1)), transform.position.y, transform.position.z);
                    }
                    else
                    {
                        newPosition = new Vector3(transform.position.x + (transform.right.x * distanceBetweenGuys * (i + 1)), transform.position.y, transform.position.z);
                    }
                }
                
 
            }
            //if (readyToThrow && guy == littleGuys[0])
            //{
                guy.transform.position = newPosition;

                
            //}
            //else
            //{
            //    guy.transform.position = Vector3.SmoothDamp(guy.transform.position, newPosition, ref velocity, smoothTime);

            //}
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LittleGuy"))
        {
            littleGuyScript = collision.gameObject.GetComponent<LittleGuyScript>(); 
        }
        if (collision.CompareTag("LittleGuy") && littleGuyScript.canCollideWithPlayer)
        {
            littleGuys.Add(collision.gameObject);
            collision.gameObject.transform.SetParent(null); 
            collision.gameObject.transform.SetSiblingIndex(littleGuys.Count - 1);
            littleGuyScript.canCollideWithPlayer = false;    
        }
    }
    public void MoveLittleGuyToBack()
    {
        if (littleGuys.Count > 0)
        {
            GameObject firstGuy = littleGuys[0];
            littleGuys.RemoveAt(0); // Remove the first guy
            littleGuys.Add(firstGuy); // Add the first guy to the end
        }
    }


}
