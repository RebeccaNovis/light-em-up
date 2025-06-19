using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMirror : MonoBehaviour
{
    private LineRenderer lr;
    [SerializeField] private Transform startPoint;
    [SerializeField] private int maxBounces = 5; // Maximum number of reflections
    [SerializeField] private float maxDistance = 100f; // Maximum laser distance

    private GameObject heldObj;

    public PickupController pickupController;

    void Start()
    {
        lr = GetComponent<LineRenderer>();

        pickupController = GameObject.Find("FirstPersonCamera").GetComponent<PickupController>();

    }

    void Update()
    {
        lr.positionCount = 1; // Reset line renderer points
        lr.SetPosition(0, startPoint.position);

        Vector3 currentPosition = startPoint.position;
        Vector3 currentDirection = transform.forward;
        int bounces = 0;

        if (pickupController != null)
        {
            if (pickupController.heldObj != null)
            {
                heldObj = pickupController.heldObj;
                Debug.Log("2 heldObj.name: " + heldObj.name);
            }
            else
            {
                heldObj = null;
            }

        }
        else
        {
            heldObj = null;
        }


        while (bounces < maxBounces)
        {
            RaycastHit hit;
            if (Physics.Raycast(currentPosition, currentDirection, out hit, maxDistance))
            {
                lr.positionCount++;
                lr.SetPosition(lr.positionCount - 1, hit.point);

                Color laserColor = lr.material.GetColor("_Color");
                Color glowColor = lr.material.GetColor("_EmissionColor");

                // Notify the object that it was hit
                LaserTarget target = hit.transform.GetComponent<LaserTarget>();
                if (target != null)
                {
                    target.RegisterHit(hit.point, hit.normal, laserColor, glowColor, this);
                }

                // Check if we hit a "mirror"
                if (hit.collider.CompareTag("Mirror"))
                {
                    Debug.Log("hit object: " + hit.collider.gameObject.name);
                    if (heldObj != null && (heldObj == hit.transform.gameObject || hit.collider.transform.parent == heldObj.transform))
                    {
                        Debug.Log("hit.transform.gameObject.name: " + hit.transform.gameObject.name);
                        Debug.Log("heldObj: " + heldObj);
                        lr.positionCount++;
                        lr.SetPosition(lr.positionCount - 1, currentPosition + currentDirection * maxDistance);
                        break;
                    }
                    else
                    {
                        // Reflect the direction and continue the loop
                        currentPosition = hit.point;
                        currentDirection = Vector3.Reflect(currentDirection, hit.normal);
                        bounces++;
                    }

                }
                else
                {
                    // Stop the laser at the hit point if not a mirror
                    break;
                }
            }
            else
            {
                // If no hit, extend the laser to the maximum distance
                lr.positionCount++;
                lr.SetPosition(lr.positionCount - 1, currentPosition + currentDirection * maxDistance);
                break;
            }
        }
    }
}
