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

    public float horizontalInput; 
    Rigidbody2D rb;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LittleGuyScript littleGuyScript;
    [SerializeField] private float throwForce;
    [SerializeField] private LeverScript leverScript;
    [SerializeField] private GameObject throwPath;
    [SerializeField] private TMP_Text savedText;
    [SerializeField] private TMP_Text killedText;
    [SerializeField] private TMP_Text healthText; 

    AudioSource audioSource;

    [SerializeField] private AudioClip footSteps;
    [SerializeField] private AudioClip jumpSound1;
    [SerializeField] private AudioClip jumpSound2;
    [SerializeField] private AudioClip littleGuyFlying;
    [SerializeField] private AudioClip littleGuySplat;
    [SerializeField] private AudioClip oof;
    [SerializeField] private AudioClip kevinLaugh;
    [SerializeField] private AudioClip throwSound;
    
    
    Animator animator;

    public Animator littleGuyAnimator; 

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
    public bool justThrew = false; 
    public bool isPlayingFootsteps = false; 

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
        animator = GetComponent<Animator>();   
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = "Health: " + currentHealth; 
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x)); 
        
        if (GroundCheck() && (rb.velocity.x > 0 || rb.velocity.x < 0) && !isPlayingFootsteps)
        {
            isPlayingFootsteps = true; 
            StartCoroutine(PlayFootSteps()); 
            Debug.Log("Robert is stupid"); 
        }
        else if (!GroundCheck())
        {
            isPlayingFootsteps = false; 
        }

        if (!GroundCheck() && rb.velocity.y > 0)
        {
            animator.SetBool("isJumpingUp", true); 
            animator.SetBool("isFalling", false); 
        }
        else if(!GroundCheck() && rb.velocity.y < 0)
        {
            animator.SetBool("isFalling", true); 
            animator.SetBool("isJumpingUp", false); 
        }
        else
        {
            animator.SetBool("isFalling", false); 
            animator.SetBool("isJumpingUp", false); 
        }

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
        if (Input.GetKeyDown(KeyCode.P) && canThrow && readyToThrow)
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
            //Vector3 startPosition = transform.position; // Assuming the player's position
            //Vector3 endPosition = startPosition + throwDirection * throwForce; // Assuming throwDirection and throwForce are accessible here
            //Debug.DrawLine(startPosition, endPosition, Color.red, 2f); // Draw a line from start to end position

            animator.SetBool("isHolding", true); 
        }
        else
        {
            throwPath.SetActive(false);
            animator.SetBool("isHolding", false); 
        }

        if (GroundCheck())
        {
            animator.SetBool("isGrounded", true); 
        }

        if (littleGuyScript.recentDeath == true)
        {
            animator.SetBool("lilGuyDead", true); 
            
            StartCoroutine(LittleGuyDeath()); 

        }

        if (justThrew)
        {
            animator.SetBool("isThrowing", true); 
        }
        else
        {
            animator.SetBool("isThrowing", false); 
        }
        savedText.text = "Little Guys Saved: " + savedGuys;
        killedText.text = "Little Guys Brutally Murdered: " + killedGuys; 
        
        
        
        
    }

    IEnumerator PlayFootSteps()
    {
        while (GroundCheck() && (rb.velocity.x > 0 || rb.velocity.x < 0))
        {
            audioSource.PlayOneShot(footSteps); 
            
            yield return new WaitForSeconds(0.2f); 
        }
        
        isPlayingFootsteps = false; 

    }
    IEnumerator LittleGuyDeath()
    {
        littleGuyScript.recentDeath = false; 
        audioSource.PlayOneShot(kevinLaugh); 
        audioSource.PlayOneShot(littleGuySplat); 
        yield return new WaitForSeconds(1); 
        animator.SetBool("lilGuyDead", false); 
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
            int randomIndex = Random.Range(0, 2);
            AudioClip selectedSound = randomIndex == 0 ? jumpSound1 : jumpSound2;
            audioSource.PlayOneShot(selectedSound);
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
            Vector3 offset = new Vector3(0f, 0.4f, 0f); 
            
            if (guy == littleGuys[0])
            {
                if (readyToThrow)
                {
                    newPosition = transform.position + (transform.up * distanceBetweenGuys);
                    littleGuyAnimator.SetBool("isHeld", true); 
                }
                else
                {
                    if (isFacingRight == true)
                    {
                        newPosition = transform.position + (-transform.right * distanceBetweenGuys * (i + 1) - offset);
                    }
                    else
                    {
                        newPosition = transform.position + (transform.right * distanceBetweenGuys * (i + 1) - offset);          
                    }
                    littleGuyAnimator.SetBool("isHeld", false); 
                }
                
            }
            else
            {
                if (readyToThrow)
                {
                    if (isFacingRight == true)
                    {
                        newPosition = transform.position + (-transform.right * distanceBetweenGuys * (i) - offset);
                    }
                    else
                    {
                        newPosition = transform.position + (transform.right * distanceBetweenGuys * (i) - offset); 
                    }
                }
                else
                {
                    if (isFacingRight == true)
                    {
                        newPosition = transform.position + (-transform.right * distanceBetweenGuys * (i + 1) - offset);
                    }
                    else
                    {
                        newPosition = transform.position + (transform.right * distanceBetweenGuys * (i + 1) - offset);
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
            littleGuyAnimator.SetFloat("Speed", Mathf.Abs(rb.velocity.x)); 
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
        
        justThrew = true; 
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
            littleGuyAnimator.SetBool("isFlying", true); 
            audioSource.PlayOneShot(littleGuyFlying); 
            audioSource.PlayOneShot(throwSound); 
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
        justThrew = false; 
        canThrow = true; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LittleGuy"))
        {
            littleGuyScript = collision.gameObject.GetComponent<LittleGuyScript>(); 
            littleGuyAnimator = littleGuyScript.GetComponent<Animator>();
            littleGuyAnimator.SetBool("isSaved", true); 
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
            audioSource.PlayOneShot(oof); 
        }

    }
}
