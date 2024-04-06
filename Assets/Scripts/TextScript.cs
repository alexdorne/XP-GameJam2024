using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextScript : MonoBehaviour
{
    public TMP_Text[] text; 
    private int currentIndex = 0;
    [SerializeField] private PlayerMovement playerMovement;
    public Collider2D openText;
    public Collider2D closeText; 

    private void Update()
    {
        
    }

    public void ClickNext()
    {
        if (currentIndex < text.Length - 1) 
        {
            text[currentIndex].gameObject.SetActive(false);

            currentIndex++;

            text[currentIndex].gameObject.SetActive(true);
        }     
        
    }

    public void ClickClose()
    {
        gameObject.SetActive(false);
        playerMovement.inputAllowed = true; 
        Cursor.visible = false;
    }
}
