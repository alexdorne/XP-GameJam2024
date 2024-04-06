using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;



public class OpenTextScript : MonoBehaviour
{
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private bool opensText; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (opensText)
            {
                tutorialText.gameObject.SetActive(true);
            }
            else
            {
                tutorialText.gameObject.SetActive(false);
            }
            Destroy(gameObject); 
        }
    }
}
