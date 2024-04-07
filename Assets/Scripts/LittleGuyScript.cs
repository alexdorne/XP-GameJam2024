using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyScript : MonoBehaviour
{
    public bool canCollideWithPlayer = true; 
    Rigidbody2D rb;

    [SerializeField] private ParticleSystem bloodParticles; 
    public bool isDead = false; 
    public bool recentDeath = false; 
    public bool isFacingRight = true; 

    [SerializeField] private LeverScript leverScript;
    [SerializeField] private PlayerMovement playerMovement; 

    Animator animator; 


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            rb.gravityScale = 4f; 
            rb.constraints = RigidbodyConstraints2D.FreezePositionX; 
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
        }
        if (!isDead)
        {
            Flip(); 
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.SetBool("isFlying", false); 
        if (collision.gameObject.CompareTag("Player"))
        {
            return; 
        }
        isDead = true; 
        
        recentDeath = true;     

        if (collision.gameObject.CompareTag("Spikes"))
        {
            animator.SetBool("isDead", true); 
            bloodParticles.Play();
  


        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            animator.SetBool("isFlattened", true); 
            bloodParticles.Play();


        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lever"))
        {
            leverScript = collision.gameObject.GetComponentInParent<LeverScript>();
            leverScript.leverOpen = true;
        }
    }

    private void Flip()
    {
        if (isFacingRight && playerMovement.horizontalInput < 0f || !isFacingRight && playerMovement.horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale; 
            localScale.x *= -1f; 
            transform.localScale = localScale;
        }
    }
}
