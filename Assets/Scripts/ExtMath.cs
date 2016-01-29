using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace eXTRIVAL
{

public static class ExtMath
{

	public static Vector3 AngleToVector3X (float angle)
	{
		angle *= Mathf.Deg2Rad;
		float z = Mathf.Sin (angle);
		float y = Mathf.Cos (angle);
		return new Vector3 (0, y, z);
	}


	public static Vector3 AngleToVector3Z (float angle)
	{
		angle *= Mathf.Deg2Rad;
		float x = Mathf.Sin (angle);
		float y = Mathf.Cos (angle);
		return new Vector3 (x, y, 0);
	}


	public static Vector2 AngleToVector2 (float angle)
	{
		angle *= Mathf.Deg2Rad;
		float x = Mathf.Sin (angle);
		float y = Mathf.Cos (angle);
		return new Vector2 (x, y);
	}
		
	
	public static int Ratio (params float[] rates)
	{
		float value = UnityEngine.Random.value * 100;
		int result = 0;
		foreach (float rate in rates) {
			value -= rate;
			if (value <= 0)
				return result;
			result ++;
		}
		return -1;
	}

	public static int Ratio2 (float[] rates)
	{
		float value = UnityEngine.Random.value * 100;
		int result = 0;
		foreach (float rate in rates) {
			value -= rate;
			if (value <= 0)
				return result;
			result ++;
		}
		return -1;
	}
		
		
	public static bool Range (float distance, float min, float max)
	{
		return (distance >= min && distance <= max) ? true : false;
	}
	
	public static float Wariai (float dstMin, float dstMax, float src, float srcMin, float srcMax)
	{
		float per = (src - srcMin) / (srcMax - srcMin);
		return (dstMax - dstMin) * Mathf.Clamp01 (per) + dstMin;
	}
		
	public static void NormalizeQuaternion (ref Quaternion q)
	{
		float sum = 0;
		for (int i = 0; i < 4; ++i)
			sum += q[i] * q[i];

		float magnitudeInverse = 1 / Mathf.Sqrt (sum);

		for (int i = 0; i < 4; ++i)
			q[i] *= magnitudeInverse;   

	}		
}

}