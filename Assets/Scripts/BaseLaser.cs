//made with https://www.youtube.com/watch?v=TokDH2OSiBE and chatGPT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLaser : MonoBehaviour
{
 private LineRenderer lr;
    [SerializeField] private Transform startPoint;

    void Start()
    {
        lr = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, startPoint.position);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit)){
            if(hit.collider){
                lr.SetPosition(1, hit.point);
                Debug.Log("hit.point: " + hit.point);
            }
            if(hit.transform.tag == "destroy"){
                Destroy(hit.transform.gameObject);
            }
        }
        else {
            lr.SetPosition(1, transform.position + transform.forward * 100); //if don't hit something, make sure laser stops after the distance of 5000
        }

    }
}
