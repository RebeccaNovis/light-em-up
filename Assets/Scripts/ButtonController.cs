using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour
{
    public UnityEvent onButtonDown;
    public UnityEvent onButtonUp;
    private Color buttonColor;
    [SerializeField] private GameObject button;
    [SerializeField] private Transform downPosition;
    [SerializeField] private Transform upPosition;

    private bool isPressed = false;
    private bool isCheckingCollision = false; // Track collision status

    // Start is called before the first frame update
    void Start()
    {
        Renderer buttonRenderer = gameObject.GetComponent<Renderer>();
        if (buttonRenderer != null)
        {
            buttonColor = buttonRenderer.material.color;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, downPosition.transform.position, Time.deltaTime * 1f);
        }
        else
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, upPosition.transform.position, Time.deltaTime * 1f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollisionColor(collision);
        if (!isCheckingCollision)
        {
            StartCoroutine(CheckCollisionContinuously(collision));
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        isCheckingCollision = false;
        isPressed = false;
        onButtonUp.Invoke();
        StopAllCoroutines(); // Stop collision checks
    }

    private IEnumerator CheckCollisionContinuously(Collision collision)
    {
        isCheckingCollision = true;
        while (isCheckingCollision)
        {
            CheckCollisionColor(collision);
            yield return null; // Wait for the next frame
        }
    }

    private void CheckCollisionColor(Collision collision)
    {
        GameObject otherObject = collision.gameObject;
        Renderer otherObjRenderer = otherObject.GetComponent<Renderer>();

        if (otherObjRenderer != null)
        {
            Color objColor = otherObjRenderer.sharedMaterial.color;

            if (objColor == buttonColor)
            {
                isPressed = true;
                onButtonDown.Invoke();
            }
            else if (objColor != buttonColor)
            {
                if (isPressed)
                {
                    isPressed = false;
                    onButtonUp.Invoke();
                }
            }
        }
    }
}
