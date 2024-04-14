	using System;
	using UnityEngine;
	public class RenderAboveEverything : MonoBehaviour{
		public void Awake () {
			// this is rough
			Material mat = GetComponent<Renderer>().material;
			mat.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always);
		}
	}

