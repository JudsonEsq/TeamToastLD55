using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class relies on a collision occuring, so the parent object needs a collider
[RequireComponent(typeof(Collider))]
public class BreakableObject : MonoBehaviour
{
    [Tooltip("The speed at which a collision will break this object.")]
    [SerializeField] private float breakThreshold = 2f;
    [Tooltip("If you want the broken object to leave scraps, put them in this array")]
    [SerializeField] private GameObject[] debrisPrefabs;
    [Tooltip("Distance between debris elements if/when they spawn")]
    [SerializeField] private float debrisSpacing = 0.1f;

    private BreakTargets parentObj;

    void Start()
    {
        // Since completing this objective destroys this item, we need to contact the objective before we destroy it
        parentObj = FindObjectOfType<BreakTargets>();

        if(parentObj.Equals(null))
        {
            Debug.LogWarning("WARNING: Breakable Object found no Break Targets Objective in scene");
            Destroy(gameObject);
        }    

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(this.GetComponentInParent<Rigidbody>().velocity.magnitude >= breakThreshold)
        {
            parentObj.TargetBroken();
            Vector3 finalPos = transform.position;
            Vector3 finalRot = transform.eulerAngles;
            // From the last position and rotation of the parent object, find the direction to stack the debris.
            Vector3 debrisVector = Vector3.RotateTowards(finalPos, finalRot, 8, debrisSpacing);

            foreach (GameObject debris in debrisPrefabs)
            {
                // Spawn each element of the Debris array, in order.
                Instantiate(debris, finalPos, Quaternion.Euler(finalRot));
                finalPos += debrisVector * debrisSpacing;
            }

            Destroy(gameObject);
        }

    }
}