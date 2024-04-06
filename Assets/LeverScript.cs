using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{

    [SerializeField] private GameObject door; 
    public bool leverOpen = false; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (leverOpen)
        {
            Collider2D doorCollider = door.GetComponent<Collider2D>();
            doorCollider.enabled = false;
        }
    }

    
}
