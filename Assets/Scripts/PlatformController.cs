using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private GameObject startPosition;
    [SerializeField] private GameObject secondPosition;
    [SerializeField] private bool isPoweredOn = false; 
    private float moveSpeed = 1f; 

    private Vector3 targetPosition;
    private Vector3 initialPosition;
    private bool movingToSecondPosition = true;

    private void Start()
    {
        // Set the initial target to secondPosition
        targetPosition = secondPosition.transform.position;
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (isPoweredOn)
        {
            MovePlatform();
        }
    }

    private void MovePlatform()
    {
        // Move the platform towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if the platform has reached the target
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            // Toggle the direction of movement
            movingToSecondPosition = !movingToSecondPosition;
            targetPosition = movingToSecondPosition ? secondPosition.transform.position : startPosition.transform.position;
        }
    }

        public void PowerOn()
    {
       isPoweredOn = true;
    }

    public void PowerOff()
    {
        isPoweredOn = false;
    }

    private void OnCollisionEnter(Collision collision){
                // Parent the object to the platform so it moves along with it
        if (collision.gameObject.GetComponent<Rigidbody>())
        {
            collision.transform.SetParent(transform);
        }
    }

     private void OnCollisionExit(Collision collision){
        if (collision.gameObject.GetComponent<Rigidbody>())
        {
            collision.transform.SetParent(null);
        }
    }
}
