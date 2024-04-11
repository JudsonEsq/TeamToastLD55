using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float mouseSens = 3;
    Vector2 mouseDelta = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        mouseDelta.y += Input.GetAxis("Mouse X");
        mouseDelta.x += -Input.GetAxis("Mouse Y");
        transform.eulerAngles = (Vector2)mouseDelta * mouseSens;
    }
}
