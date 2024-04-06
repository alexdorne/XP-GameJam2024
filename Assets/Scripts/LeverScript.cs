using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{

    [SerializeField] private GameObject door; 
    public bool leverOpen = false; 

    AudioSource audioSource;

    [SerializeField] private AudioClip leverClick;
    [SerializeField] private AudioClip doorOpen;

    SpriteRenderer leverSpriteRenderer; 
    [SerializeField] private SpriteRenderer doorSpriteRenderer;
    [SerializeField] private Sprite leverSprite; 
    [SerializeField] private Sprite doorSprite; 


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();  
        leverSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (leverOpen)
        {
            Collider2D doorCollider = door.GetComponent<Collider2D>();
            doorCollider.enabled = false;
            leverSpriteRenderer.sprite = leverSprite;
            doorSpriteRenderer.sprite = doorSprite;
            audioSource.PlayOneShot(doorOpen); 
            audioSource.PlayOneShot(leverClick); 
            leverOpen = false; 
        }
    }

    
}
