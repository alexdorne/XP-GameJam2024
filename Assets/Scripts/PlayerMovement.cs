using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpTime; 

    private float smoothTime = 0.02f; 
    private Vector3 velocity = Vector3.zero;
    Vector3 throwDirection; 

    private float horizontalInput; 
    Rigidbody2D rb;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LittleGuyScript littleGuyScript;
    [SerializeField] private float throwForce;
    [SerializeField] private LeverScript leverScript;
    [SerializeField] private GameObject throwPath;
    [SerializeField] private TMP_Text savedText;
    [SerializeField] private TMP_Text killedText;

    [SerializeField] private int maxHealth; 
    public int currentHealth; 

    public int savedGuys;
    public int killedGuys; 

    private bool canLongJump = true; 
    public bool isFacingRight = true; 
    public bool readyToThrow = false; 
    public float guyJumpDelay = 1.0f; 
    public bool canThrow = true; 
    public bool inputAllowed = true; 
    public bool hasThrown = false; 
    public bool hasSaved = false; 

    public List<GameObject> littleGuys = new List<GameObject>();
    public float distanceBetweenGuys = 1.0f; 
    public Vector3 spikePoint; 
    public Vector3 checkPoint; 
    public TextScript textScript; 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth; 
    }

    // Update is called once per frame
    void Update()
    {
        LittleGuyMovement();
        if (!inputAllowed)
        {
            return; 
        }
        horizontalInput = Input.GetAxisRaw("Horizontal"); 
        Jump(); 
        Flip(); 
        
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            readyToThrow = !readyToThrow; 
        }

        if (readyToThrow && Input.GetKeyDown(KeyCode.O)){
            MoveLittleGuyToBack(); 
        }
        if (Input.GetKeyDown(KeyCode.P) && canThrow)
        {
            Throw(); 
        }

        if (readyToThrow && littleGuys.Count > 0)
        {
            throwPath.SetActive(true);
            if (isFacingRight)
            {
                throwDirection = (transform.right + Vector3.up).normalized; 
            }
            else
            {
                throwDirection = (-transform.right + Vector3.up).normalized; 

            }
            Vector3 startPosition = transform.position; // Assuming the player's position
            Vector3 endPosition = startPosition + throwDirection * throwForce; // Assuming throwDirection and throwForce are accessible here
            Debug.DrawLine(startPosition, endPosition, Color.red, 2f); // Draw a line from start to end position
        }
        else
        {
            throwPath.SetActive(false);
        }

        savedText.text = "Little Guys Saved: " + savedGuys;
        killedText.text = "Little Guys Brutally Murdered: " + killedGuys; 
        
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * movementSpeed, rb.velocity.y); //Horizontal Movement
        

        if (currentHealth == 0)
        {
            transform.position = checkPoint; 
            currentHealth = maxHealth; 
        }
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
                        newPosition = transform.position + (-transform.right * distanceBetweenGuys * (i + 1));
                    }
                    else
                    {
                        newPosition = transform.position + (transform.right * distanceBetweenGuys * (i + 1));          }
                }
                
            }
            else
            {
                if (readyToThrow)
                {
                    if (isFacingRight == true)
                    {
                        newPosition = transform.position + (-transform.right * distanceBetweenGuys * (i));
                    }
                    else
                    {
                        newPosition = transform.position + (transform.right * distanceBetweenGuys * (i)); 
                    }
                }
                else
                {
                    if (isFacingRight == true)
                    {
                        newPosition = transform.position + (-transform.right * distanceBetweenGuys * (i + 1));
                    }
                    else
                    {
                        newPosition = transform.position + (transform.right * distanceBetweenGuys * (i + 1));
                    }
                }
                
 
            }
            if (readyToThrow && guy == littleGuys[0])
            {
                guy.transform.position = newPosition;

                
            }
            else
            {
                guy.transform.position = Vector3.SmoothDamp(guy.transform.position, newPosition, ref velocity, smoothTime);

            }
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

    public void Throw()
    {
        canThrow = false; 
        if (!readyToThrow)
        {
            return; 
        }

        if (littleGuys.Count > 0)
        {
            
            GameObject thrownLittleGuy = littleGuys[0];
            littleGuys.RemoveAt(0); 
            Rigidbody2D guyRb = thrownLittleGuy.GetComponent<Rigidbody2D>();    
            Collider2D guyCollider = thrownLittleGuy.GetComponent<Collider2D>();
            guyCollider.isTrigger = false;
            guyRb.gravityScale = 4f; 
            guyRb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse); 
            readyToThrow = false; 
            StartCoroutine(throwDelay()); 

            
        }
        if (hasThrown == false)
        {
            killedText.gameObject.SetActive(true);
            hasThrown = true;
        }

        savedGuys--; 
        killedGuys++; 
    }

    IEnumerator throwDelay()
    {
        yield return new WaitForSeconds(1); 
        canThrow = true; 
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
            if (hasSaved == false)
            {
                savedText.gameObject.SetActive(true);
                hasSaved = true; 
            }
            savedGuys++; 
        }
        if (collision.CompareTag("SpikePoint"))
        {
            spikePoint = transform.position; 
        }
        if (collision.CompareTag("CheckPoint"))
        {
            checkPoint = transform.position;
        }
        if (collision.CompareTag("Lever"))
        {
            leverScript = collision.gameObject.GetComponentInParent<LeverScript>();
            leverScript.leverOpen = true;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spikes"))
        {
            currentHealth--; 
            transform.position = spikePoint; 
        }

    }
}
