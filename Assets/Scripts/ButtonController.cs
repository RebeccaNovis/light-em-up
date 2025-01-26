using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour
{
    public UnityEvent onButtonUp;
    public UnityEvent onButtonDown;
    private Color buttonColor;
    [SerializeField] private GameObject button;
    [SerializeField] private Transform downPosition;
    [SerializeField] private Transform  upPosition;

    private bool isPressed = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Renderer buttonRenderer = gameObject.GetComponent<Renderer>();
        if(buttonRenderer != null){
            buttonColor = buttonRenderer.material.color;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPressed){
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, downPosition.transform.position, Time.deltaTime * 1f);
        }
        else{
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, upPosition.transform.position, Time.deltaTime * 1f);
        }
    }

    private void OnCollisionEnter(Collision collision){
        GameObject otherObject = collision.gameObject;
        Renderer otherObjRenderer = otherObject.GetComponent<Renderer>();

        if(otherObjRenderer != null){
            Color objColor = otherObjRenderer.material.color;

            if(objColor == buttonColor){
                isPressed = true;
                onButtonDown.Invoke();
            }
            else{
                Debug.Log("colors not the same. objColor: " + objColor + " buttonColor: " + buttonColor);
            }
        }
    }

    private void OnCollisionExit(Collision collision){
        isPressed = false;
        onButtonUp.Invoke();
    }
}
