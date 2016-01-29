using UnityEngine;
using System.Collections;

namespace eXTRIVAL {

public class ShakeController : MonoBehaviour 
{
	public Vector3 magnitude { get; set; }
	Vector3 inpulseMagnitude = Vector3.zero;

	void Update ()
    {
    	Vector3 m = magnitude + inpulseMagnitude;
		if (m  == Vector3.zero) return;

		transform.localPosition = m;
		inpulseMagnitude = Vector3.zero;
 	 }

	public void ApplyMagnitude (Vector3 value)
	{
		inpulseMagnitude += value;
	}

}

}
