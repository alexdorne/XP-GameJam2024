using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyScript : MonoBehaviour
{
    public bool canCollideWithPlayer = true; 
    Rigidbody2D rb; 
    public bool isDead = false; 

    [SerializeField] private LeverScript leverScript; 


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            rb.gravityScale = 0f; 

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
            isDead = true; 
            rb.constraints = RigidbodyConstraints2D.FreezeAll; 
        


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lever"))
        {
            leverScript = collision.gameObject.GetComponentInParent<LeverScript>();
            leverScript.leverOpen = true;
        }
    }
}
