using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class LaserTarget : MonoBehaviour
{
    public UnityEvent<Vector3, Vector3> onLaserHit;
    public UnityEvent onLaserStop;
    public UnityEvent onPoweredOn;
    public UnityEvent onPoweredOff;

    private float hitCooldown = .2f;  // Time to wait before considering the object no longer hit
    private Dictionary<LaserMirror, float> activeLasers = new Dictionary<LaserMirror, float>(); // Track lasers hitting this object
    private HashSet<Color> activeLaserColors = new HashSet<Color>(); // Set to track all laser colors hitting the target
    private Color[] acceptableColors = new Color[]{
        Color.red,
            Color.green,
            Color.blue,
            Color.red,
            Color.white,
            new Color(1, 1, 0), //yellow 
            new Color(1, 0, 1), //magenta
            new Color(0, 1, 1) //cyan
    };
    private Color oldColor;
    Color oldLaserColor;
    private Color combinedColor = Color.black;

    private float timeTillColorRevert = 100f;

    private bool isHit = false; // Tracks if the raycast is hitting this object

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        oldColor = renderer.material.color;
        if (gameObject.GetComponent<LineRenderer>())
        {
            oldLaserColor = gameObject.GetComponent<LineRenderer>().material.color;
        }
    }

    private void Update()
    {
        List<LaserMirror> lasersToRemove = new List<LaserMirror>();
        List<Color> colorsToRemove = new List<Color>(); // List to track colors to remove

        // Update timers for active lasers
        foreach (var laser in new List<LaserMirror>(activeLasers.Keys)) // Creating a snapshot to avoid modifying the collection directly
        {
            activeLasers[laser] += Time.deltaTime;

            // Add lasers and colors to be removed if they have timed out
            if (activeLasers[laser] >= hitCooldown)
            {
                lasersToRemove.Add(laser);

                // Assuming each laser has a color associated with it, add the corresponding color to be removed
                LineRenderer lr = laser.GetComponent<LineRenderer>();
                if (lr != null)
                {
                    Color laserColor = lr.material.GetColor("_Color");
                    colorsToRemove.Add(laserColor);
                }
            }
        }

        // Remove lasers and colors after the iteration is complete
        foreach (var laser in lasersToRemove)
        {
            activeLasers.Remove(laser);
        }

        foreach (var color in colorsToRemove)
        {
            activeLaserColors.Remove(color);
        }

        // Combine colors or perform logic based on active colors
        CombineLaserColors();

        if (activeLasers.Count == 0)
        {
            OnLaserStop();
        }

    }
    
    public void RegisterHit(Vector3 hitPoint, Vector3 hitNormal, Color laserColor, Color glowColor, LaserMirror laser)
    {
        if (!activeLasers.ContainsKey(laser))
        {
            activeLasers.Add(laser, 0f); // Add the laser to the list and reset its timer
        }
        else
        {
            activeLasers[laser] = 0f; // Reset the timer if it's already active
        }

        // Add the color of the laser to the activeLaserColors set
        activeLaserColors.Add(laserColor);
        isHit = true;

        onLaserHit.Invoke(hitPoint, hitNormal);
    }

    private void OnLaserStop()
    {
        if (!isHit) return; // Avoid unnecessary calls
        isHit = false;
        onLaserStop?.Invoke();
    }

    public void CombineLaserColors()
    {
        // If there are no lasers, return early
        if (activeLaserColors.Count == 0) return;

        // Start with object's color and combine all laser colors
        if(gameObject.GetComponent<LineRenderer>()){
            combinedColor = oldLaserColor;
        } else{
            combinedColor = oldColor;
        }
        

        // Combine all laser colors
        foreach (Color laserColor in activeLaserColors)
        {
            combinedColor += laserColor; // Add each laser's color
        }

        combinedColor = new Color(
            Mathf.Clamp01(combinedColor.r),
            Mathf.Clamp01(combinedColor.g),
            Mathf.Clamp01(combinedColor.b)
        );

    }

    public void ChangeColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (isHit)
        {
            if (renderer != null)
            {
                //check that the color is actually a laser color
                for (int i = 0; i < acceptableColors.Length; i++)
                {
                    if (combinedColor == acceptableColors[i])
                    {
                        renderer.material.color = combinedColor;
                    }
                    else
                    {
                        //Debug.Log(gameObject.name + " object was hit by a laser but the color was not acceptable");
                    }
                }
            }
        }


    }

    public void ReturnColor(){
        StartCoroutine(DelayedUpdateColor(timeTillColorRevert,  oldColor));
    }
    private IEnumerator DelayedUpdateColor(float delay, Color newColor){
        Renderer renderer = GetComponent<Renderer>();
        yield return new WaitForSeconds(delay);
        renderer.material.color = newColor;
    }

    public void ChangeLaserColor()
    {
        LineRenderer hitLaserLR = gameObject.GetComponent<LineRenderer>();
        if (hitLaserLR != null)
        {
            //check that the color is actually a laser color
            for (int i = 0; i < acceptableColors.Length; i++)
            {
                if (combinedColor == acceptableColors[i])
                {
                    // Ensure the material is unique to this instance
                    hitLaserLR.material = new Material(hitLaserLR.material);

                    hitLaserLR.material.SetColor("_Color", combinedColor);
                    // Update the emission color
                    if (hitLaserLR.material.HasProperty("_EmissionColor"))
                    {
                        hitLaserLR.material.EnableKeyword("_EMISSION");
                        hitLaserLR.material.SetColor("_EmissionColor", combinedColor * 2);
                        
                    }

                    //Debug.Log(gameObject.name + " was hit by another laser and emission color was updated! new color: " + combinedColor);
                }
                else
                {
                    //Debug.Log(gameObject.name +  " was hit by another laser but the color was not acceptable! bad color: " + newColor);
                }

            }

        }

    }

    public void ResetLaserColor()
    {
        LineRenderer hitLaserLR = gameObject.GetComponent<LineRenderer>();
        if (hitLaserLR != null)
        {
            //check that the color is actually a laser color
            for (int i = 0; i < acceptableColors.Length; i++)
            {
                if (oldLaserColor == acceptableColors[i])
                {
                    // Ensure the material is unique to this instance
                    hitLaserLR.material = new Material(hitLaserLR.material);

                    hitLaserLR.material.SetColor("_Color", oldLaserColor);
                    // Update the emission color
                    if (hitLaserLR.material.HasProperty("_EmissionColor"))
                    {
                        hitLaserLR.material.EnableKeyword("_EMISSION");
                        hitLaserLR.material.SetColor("_EmissionColor", oldLaserColor * 2);
                        
                    }

                    //Debug.Log(gameObject.name + " was hit by another laser and emission color was updated! new color: " + combinedColor);
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

        if (renderer.material.color == combinedColor)
        {
            onPoweredOn.Invoke();
        }

    }

    public void PowerOff()
    {
        onPoweredOff.Invoke();
    }
}
