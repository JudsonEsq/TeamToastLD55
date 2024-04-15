using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class relies on a collision occuring, so the parent object needs a collider
[RequireComponent(typeof(Collider))]
public class BreakableObject : MonoBehaviour
{
    private Rigidbody rb;

    [Tooltip("The speed at which a collision will break this object.")]
    [SerializeField] private float breakThreshold = 2f;
    [Tooltip("If you want the broken object to leave scraps, put them in this array")]
    [SerializeField] private GameObject[] debrisPrefabs;
    [Tooltip("Distance between debris elements if/when they spawn")]
    [SerializeField] private float debrisSpacing = 0.1f;

    [SerializeField] private bool isObjectiveItem = false;
    public AudioClip breakSound;

    private bool breaking = false;

    private BreakTargets objectiveHandler;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (isObjectiveItem)
        {
            objectiveHandler = FindObjectOfType<BreakTargets>();
        }

        /*if(parentObj == null)
        {
            Debug.LogWarning("WARNING: Breakable Object found no Break Targets Objective in scene");
            Destroy(gameObject);
        }*/

    }

    private void BreakSelf()
    {
        if (objectiveHandler != null)
        {
            objectiveHandler.TargetBroken();
        }

        if(breakSound != null)
        {
            AudioSource source = gameObject.GetComponent<AudioSource>();
            source.clip = breakSound;
            source.Play();
        }
        
        Vector3 finalPos = transform.position;
        Vector3 finalRot = transform.eulerAngles;
        // From the last position and rotation of the parent object, find the direction to stack the debris.
        Vector3 debrisVector = Vector3.RotateTowards(finalPos, finalRot, 8f, debrisSpacing);

        foreach (GameObject debris in debrisPrefabs)
        {
            // Spawn each element of the Debris array, in order.
            Instantiate(debris, finalPos, Quaternion.Euler(finalRot));
            finalPos += debrisVector * debrisSpacing;
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rb != null && rb.velocity.magnitude >= breakThreshold && !breaking)
        {
            breaking = true;
            BreakSelf();
            return;
        }

        if (collision.rigidbody != null && collision.rigidbody.velocity.magnitude >= breakThreshold && !breaking)
        {
            breaking = true;
            BreakSelf();
            return;
        }

    }
}