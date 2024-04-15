using UnityEngine;
using UnityEditor;

public class RigidbodyInterpolationSetter : Editor
{
	[MenuItem("Tools/Set Rigidbody Interpolation")]
	public static void SetRigidbodyInterpolation()
	{
		// Get all Rigidbody components in the scene
		// find every selected object in the scene
		GameObject[] objs =  Selection.gameObjects;
		
		// Iterate over each Rigidbody
		foreach (GameObject obj in objs)
		{
			Rigidbody rb = obj.GetComponent<Rigidbody>();
			if (rb)
			{
				// Set the Rigidbody's interpolation to Interpolate
				rb.interpolation = RigidbodyInterpolation.Interpolate;
				Debug.Log("Set Rigidbody interpolation to Interpolate for " + obj.name);
			}

		}
	}
}
