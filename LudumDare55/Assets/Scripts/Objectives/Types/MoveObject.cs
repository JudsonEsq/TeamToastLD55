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

    [Tooltip("Object's name to be placed or removed")]
    [SerializeField] private string objectName = "";
    [Tooltip("Where to place or remove the objects from")]
    [SerializeField] private string platformName = "";

    [Header("Debug")]
    [SerializeField] private int currentNumberOfItems;

    protected override void Start()
    {
        base.Start();

        switch (moveType)
        {
            default:
            case MoveType.Place:
                currentNumberOfItems = 0;
                if (destroyOnFull)
                {
                    containedObjects = new GameObject[numberOfItems];
                }

                break;
            case MoveType.Remove:
                currentNumberOfItems = numberOfItems;
                break;
        }

        // update objective text
        UpdateObjectiveText();

    }

    protected override bool IsCompleted()
    {
        switch (moveType)
        {
            default:
            case MoveType.Place:
                if (destroyOnFull && currentNumberOfItems >= numberOfItems)
                {
                    foreach (GameObject obj in containedObjects)
                    {
                        Destroy(obj);
                    }
                    destroyOnFull = false;
                    // Just make this true forever once we've destroyed everything inside
                    // OT : I don't think we need the line below, since once the objective is completed, nothing is meant to run on the class anymore
                    //currentNumberOfItems = 5 * numberOfItems;
                }
                return currentNumberOfItems >= numberOfItems;
            case MoveType.Remove:
                return currentNumberOfItems <= 0;
        }
    }

    private void UpdateObjectiveText()
    {
        var text = "";
        switch (moveType)
        {
            default:
            case MoveType.Place:
                text = $"Place {objectName} on {platformName} : {currentNumberOfItems} / {numberOfItems}";
                SetObjectiveText(text);

                break;
            case MoveType.Remove:
                text = $"Remove {objectName} from {platformName} : {currentNumberOfItems}";
                SetObjectiveText(text);
                break;
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
                    if (containedObjects != null)
                    {
                        containedObjects[currentNumberOfItems] = other.gameObject;
                    }
                }
                //Debug.Log($"Added {other.name}");
                currentNumberOfItems++;

                // update objective text
                UpdateObjectiveText();
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
            if (destroyOnFull)
            {
                if (containedObjects != null)
                {
                    for (int i = 0; i < containedObjects.Length; i++)
                    {
                        if (containedObjects[i] == null) continue;

                        if (containedObjects[i].Equals(other.gameObject))
                        {
                            containedObjects[i] = null;
                            break;
                        }
                    }
                }

            }
            // When the player removes an object
            if (currentNumberOfItems > 0)
            {
                //Debug.Log($"Removed {other.name}");
                currentNumberOfItems--;

                // update objective text
                UpdateObjectiveText();
            }
        }
    }

}
