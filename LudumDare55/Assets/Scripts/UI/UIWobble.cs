using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWobble : MonoBehaviour
{
    [SerializeField] private float WobbleSpeed = 1f;
    [SerializeField] private float startValue = 0f;
    [Header("Rotation")]
    [Tooltip("Whether this object should also have rotational wobble")]
    [SerializeField] private bool rotateWobble = false;
    [SerializeField] private float rotationalSpeed = 1f;
    [SerializeField] private float rotationRange = 0.2f;

    [Header("Scale")]
    [SerializeField] private bool scaleWobble = false;
    [SerializeField] private float scaleSpeed = 1f;
    [SerializeField] private float scaleRange = 0.2f;
    private Vector3 initialScale;



    private void Start()
    {
        initialScale = transform.localScale;
    }

    void FixedUpdate()
    {
        transform.position += Vector3.up * Mathf.Sin(startValue) * WobbleSpeed;
        startValue += 0.1f;

        if (rotateWobble)
        {
            transform.rotation = Quaternion.Euler(rotationRange * Mathf.Sin(startValue) * Vector3.forward);
        }

        if (scaleWobble)
        {
            transform.localScale = initialScale * (1 + scaleRange * Mathf.Sin(startValue * scaleSpeed));
        }

    }
}
