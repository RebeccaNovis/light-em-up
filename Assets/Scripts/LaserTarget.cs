using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserTarget : MonoBehaviour
{
    public UnityEvent<Vector3, Vector3> onLaserHit;
    public UnityEvent onLaserStop;

    private float hitCooldown = 0.2f;  // Time to wait before considering the object no longer hit
    private float timeSinceLastHit = 0f; // Timer to track time since last hit

    private Color[] acceptableColors = new Color[]{
        Color.red,
            Color.green,
            Color.blue,
            Color.red,
            new Color(1, 1, 0), //yellow 
            new Color(1, 0, 1), //magenta
            new Color(0, 1, 1) //cyan
    };
    private Color newColor;
    private Color newGlowColor;
    private Color oldColor;

    [SerializeField] private GameObject poweredObject;
    [SerializeField] private GameObject openPosition;
    [SerializeField] private GameObject closedPosition;

    private bool isHit = false; // Tracks if the raycast is hitting this object
    public bool isPoweredOn = false; // Tracks the powered state of the object

    public void RegisterHit(Vector3 hitPoint, Vector3 hitNormal, Color laserColor, Color glowColor)
    {
        onLaserHit.Invoke(hitPoint, hitNormal);
        newColor = laserColor;
        newGlowColor = glowColor;

        isHit = true;
        timeSinceLastHit = 0f;
    }

    private void Update()
    {
        // If the object was hit, increment the time since last hit
        if (isHit)
        {
            timeSinceLastHit += Time.deltaTime;

            // If the timer exceeds the cooldown, the object is no longer being hit
            if (timeSinceLastHit >= hitCooldown)
            {
                isHit = false;
                OnLaserStop();
            }
        }
    }

    private void OnLaserStop()
    {
        Debug.Log("Laser is no longer hitting the target.");
        onLaserStop?.Invoke();
    }

    public void ChangeColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (isHit)
        {
            if (renderer != null)
            {
                oldColor = renderer.material.color;
                //check that the color is actually a laser color
                for (int i = 0; i < acceptableColors.Length; i++)
                {
                    if (newColor == acceptableColors[i])
                    {
                        renderer.material.color = newColor;
                    } else{
                        Debug.Log(gameObject.name + " object was hit by a laser but the color was not acceptable");
                    }
                }
            }
        }

    }

    public void ChangeLaserColor()
    {
        LineRenderer hitLaserLR = gameObject.GetComponent<LineRenderer>();
        if (hitLaserLR != null)
        {
            //check that the color is actually a laser color
            for (int i = 0; i < acceptableColors.Length; i++)
            {
                if (newColor == acceptableColors[i])
                {
                    // Ensure the material is unique to this instance
                    hitLaserLR.material = new Material(hitLaserLR.material);

                    hitLaserLR.material.SetColor("_Color", newColor);
                    // Update the emission color
                    if (hitLaserLR.material.HasProperty("_EmissionColor"))
                    {
                        hitLaserLR.material.EnableKeyword("_EMISSION");
                        hitLaserLR.material.SetColor("_EmissionColor", newGlowColor);
                    }

                    Debug.Log(gameObject.name +  " was hit by another laser and emission color was updated! new color: " + newColor);
                }
                else
                {
                    //Debug.Log(gameObject.name +  " was hit by another laser but the color was not acceptable! bad color: " + newColor);
                }

            }

        }

    }

        

    public void PowerOn()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer.material.color == newColor)
        {
            isPoweredOn = true;
            //poweredObject.transform.position = Vector3.Lerp(poweredObject.transform.position, openPosition.transform.position, Time.deltaTime * poweredObjectSpeed);
        }

    }

    public void PowerOff()
    {
        Debug.Log("closing...");
        isPoweredOn = false;
        //poweredObject.transform.position = Vector3.Lerp(poweredObject.transform.position, closedPosition.transform.position, Time.deltaTime * poweredObjectSpeed);
    }
}
