using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

public class ObjectReplacer : Editor
{
	[MenuItem("Tools/Update Models")]
	public static void SetRigidbodyInterpolation()
	{
		// find every game object in the scene
		GameObject[] objs =  FindObjectsOfType<GameObject>();
		// get gameobjects from Assets/Models/UpdatedDestructibles/Furniture
		List<GameObject> assets = GetAssetsAtPath<GameObject>("Assets/Models/UpdatedDestructibles").ToList();
		Debug.Log(assets.Count);
		List<string> assetNames = new List<string>();
		foreach (GameObject asset in assets) {
			assetNames.Add(asset.name);
		}
		
		Debug.Log(objs.Length);
		int i = 0;
		foreach (GameObject original in objs) {
			i++; 
			
			string cleanedName = original.name;
			if (cleanedName.Contains("(")) {
				cleanedName = cleanedName.Substring(0, cleanedName.IndexOf("(") - 1);
			}
			// look for the asset with the same name as the original object in the assets array
			bool found = false;
			foreach (GameObject asset in assets) {
				if (Regex.IsMatch(asset.name, cleanedName)) {
					found = true;
					break;
				}
			}
			Debug.Log(found);
			
			if (found) {
				// get cleaned name without any parentheses and numbers like (1)
				// "I (1)" -> "I"
				// find the asset with the same name in the assets array
				GameObject actualAsset = assets.Find(x => Regex.IsMatch(x.name, cleanedName));
				GameObject clone = Instantiate(actualAsset, original.transform.position, original.transform.rotation);
				clone.transform.parent = original.transform.parent;
				clone.transform.localPosition = original.transform.localPosition;
				clone.transform.localRotation = original.transform.localRotation;
				clone.name = original.name;
				clone.layer = original.layer;
				clone.tag = original.tag;
				// copy components
				foreach (Component comp in original.GetComponents<Component>()) {
					if (comp.GetType() != typeof(Transform)) {
						Component newComp = clone.AddComponent(comp.GetType());
						EditorUtility.CopySerialized(comp, newComp);
					}
				}
				DestroyImmediate(original);
			}
		}
		Debug.Log(i);
		Debug.Log(objs.Length);
		if (i != objs.Length) {
			Debug.Log("Something's fucked.");
		}
	}
	public static T[] GetAssetsAtPath<T>(string path) where T : Object
	{
		var assets = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { path });
		List<T> foundAssets = new List<T>();
 
		foreach (var guid in assets)
		{
			foundAssets.Add(AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)));
		}
 
		// if you want to skip the convertion to array, simply change method return type
		return foundAssets.ToArray();
	}
}