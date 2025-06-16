// created with https://www.youtube.com/watch?v=6bFCQqabfzo and chatGPT
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [SerializeField] Transform holdArea;
    public GameObject heldObj;
    private Rigidbody heldObjRB;

    [SerializeField] private float pickupRange = 5.0f;
    [SerializeField] private float pickupForce = 100.0f;

    [SerializeField] private float rotationSpeed = 100.0f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickupRange))
                {
                    PickupObject(hit.transform.gameObject);
                }
            }
            else
            {
                DropObject();
            }
        }
        if (heldObj != null)
        {
            MoveObject();

            // Call rotation if there's scroll input
            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0f)
            {
                Debug.Log("scroll input: " + Input.mouseScrollDelta.y);
                RotateObject();
            }
        }
    }

    void PickupObject(GameObject pickedObj)
    {
        if (pickedObj.GetComponent<Rigidbody>())
        {
            heldObjRB = pickedObj.GetComponent<Rigidbody>();
            heldObjRB.useGravity = false;
            heldObjRB.isKinematic = true;
            //heldObjRB.drag = 10;
            //heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;

            heldObjRB.transform.parent = holdArea;
            heldObj = pickedObj;
            Debug.Log("1 heldObj.name: " + heldObj.name);
        }
    }

    void DropObject()
    {
        heldObjRB.useGravity = true;
        heldObjRB.isKinematic = false;
        // heldObjRB.drag = 1;
        //heldObjRB.constraints = RigidbodyConstraints.None;

        heldObjRB.transform.parent = null;
        heldObj = null;
    }

    void MoveObject()
    {
        // Align the object's position and rotation with the hold area
        heldObjRB.position = holdArea.position;
        heldObjRB.rotation = holdArea.rotation;
    }

    void RotateObject()
    {
        float rotationAmount = Input.mouseScrollDelta.y * rotationSpeed;
        heldObj.transform.Rotate(0f, rotationAmount, 0f, Space.Self);
    }
}
