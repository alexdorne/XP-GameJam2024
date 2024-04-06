using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -15f); 
    private float smoothTime = 0.25f; 
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform player;

    private void Start()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);

    }

    void LateUpdate()
    {
        Vector3 targetPosition = player.position + offset; 
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime); 
    }
}
