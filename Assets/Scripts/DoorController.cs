using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private LaserTarget laserTarget;
    [SerializeField] private GameObject openPosition;
    [SerializeField] private GameObject closedPosition;
    private float doorSpeed = 1f; // Speed of the poweredObject's movement

    // Update is called once per frame
    void Update()
    {
        if (laserTarget != null && laserTarget.isPoweredOn)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, openPosition.transform.position, Time.deltaTime * doorSpeed);
    }

    void CloseDoor()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, closedPosition.transform.position, Time.deltaTime * doorSpeed);
    }
}
