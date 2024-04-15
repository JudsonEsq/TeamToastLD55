using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeMixer : MonoBehaviour
{
	public void SetVolume(Single volume) {
		Debug.Log(volume);
		AudioListener.volume = volume;
	}
}
