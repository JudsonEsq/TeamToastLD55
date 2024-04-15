using UnityEngine;
using UnityEditor;

public class RigidbodyInterpolationSetter : Editor
{
	[MenuItem("Tools/Set Rigidbody Interpolation")]
	public static void SetRigidbodyInterpolation()
	{
		// Get all Rigidbody components in the scene
		Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();
		Debug.Log("Found " + rigidbodies.Length + " Rigidbody components in the scene.");
		
		// Iterate over each Rigidbody
		foreach (Rigidbody rb in rigidbodies)
		{

			Debug.Log("Setting interpolation mode for Rigidbody on " + rb.gameObject.name);
			// If it does, set its interpolation mode to Interpolate
			rb.interpolation = RigidbodyInterpolation.Interpolate;
			
		}
	}
}
