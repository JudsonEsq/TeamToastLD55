using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// There should be a collider on this component, for detection
[RequireComponent(typeof(Collider))]
public class MoveObject : Objective
{
    // This is an objective to remove or place all objects in the collider,
    // The objective is marked as complete when all objects have been removed or placed
    public enum MoveType
    {
        Place,
        Remove,
    }
    [SerializeField] private MoveType moveType;
    [SerializeField] private int numberOfItems = 1;

    [SerializeField] private bool destroyOnFull = false;
    [Tooltip("The prefab to spawn when this objective is completed, if destroyOnFull is true")]
    [SerializeField] private GameObject rewardPrefab;

    private GameObject[] containedObjects;
    private int currentNumberOfItems;

    protected override void Start()
    {
        base.Start();

        switch (moveType)
        {
            default:
            case MoveType.Place:
                currentNumberOfItems = 0;
                if(destroyOnFull)
                {
                    containedObjects = new GameObject[numberOfItems];
                }
                break;
            case MoveType.Remove:
                currentNumberOfItems = numberOfItems;
                break;
        }

    }

    protected override bool IsCompleted()
    {
        switch (moveType)
        {
            default:
            case MoveType.Place:
                if(destroyOnFull && currentNumberOfItems >= numberOfItems)
                {
                    foreach(GameObject obj in containedObjects)
                    {
                        Destroy(obj);
                    }
                    destroyOnFull = false;
                    // Just make this true forever once we've destroyed everything inside
                    currentNumberOfItems = 5 * numberOfItems;
                }    
                return currentNumberOfItems >= numberOfItems;
            case MoveType.Remove:
                return currentNumberOfItems <= 0;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (Completed) return;

        if (other.gameObject.CompareTag("Interactable"))
        {
            // Incase the player drops an interactable object back in, register it once again
            if (currentNumberOfItems < numberOfItems)
            {
                if (destroyOnFull)
                {
                    containedObjects[currentNumberOfItems] = other.gameObject;
                }
                //Debug.Log($"Added {other.name}");
                currentNumberOfItems++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Completed) return;

        // The tag can later be set to whatever, but should be the same as whatever object we are testing for
        if (other.gameObject.CompareTag("Interactable"))
        {
            // Remove that item from the array of contained objects if we're using it.
            if(destroyOnFull)
            {
                for(int i = 0; i < containedObjects.Length; i++)
                {
                    if (containedObjects[i] == null)
                    {
                        break;
                    }

                    if(containedObjects[i].Equals(other.gameObject))
                    {
                        containedObjects[i] = null;
                        break;
                    }
                }
            }
            // When the player removes an object
            if (currentNumberOfItems > 0)
            {
                //Debug.Log($"Removed {other.name}");
                currentNumberOfItems--;
            }
        }
    }

}
