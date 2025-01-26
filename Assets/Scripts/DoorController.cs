using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject openPosition;
    [SerializeField] private GameObject closedPosition;
    private bool isPoweredOn = false; 
    private float doorSpeed = 1f; // Speed of the poweredObject's movement

    // Update is called once per frame
    void Update()
    {
        if (isPoweredOn)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, openPosition.transform.position, Time.deltaTime * doorSpeed);
        }
        else
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, closedPosition.transform.position, Time.deltaTime * doorSpeed);
        }
    }

    public void OpenDoor()
    {
       isPoweredOn = true;
    }

    public void CloseDoor()
    {
        isPoweredOn = false;
    }
}
