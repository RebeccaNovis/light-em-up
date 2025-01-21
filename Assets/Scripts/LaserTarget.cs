using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserTarget : MonoBehaviour
{
    public UnityEvent<Vector3, Vector3> onLaserHit;
    private Color newColor;
    private Color newGlowColor;
    private Color oldColor;

    public void RegisterHit(Vector3 hitPoint, Vector3 hitNormal, Color laserColor, Color glowColor)
    {
        onLaserHit.Invoke(hitPoint, hitNormal);
        newColor = laserColor;
        newGlowColor = glowColor;
    }

    public void ChangeColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            oldColor = renderer.material.color;
            renderer.material.color = newColor;
        }
    }

    public void ChangeLaserColor()
    {
        LineRenderer hitLaserLR = gameObject.GetComponent<LineRenderer>();
        if (hitLaserLR != null)
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

            Debug.Log("Laser was hit by another laser and emission color was updated!");
        }

    }
}
