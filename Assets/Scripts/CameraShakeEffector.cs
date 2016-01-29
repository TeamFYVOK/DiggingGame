using UnityEngine;
using System.Collections;

namespace eXTRIVAL {

/// <summary>
///  カメラのシェイク効果を発生させる。
/// メインカメラ側に ShakeController コンポーネントが必要
/// </summary>
public class CameraShakeEffector : MonoBehaviour 
{
	public float start = 0;
	public float time = 1;
	public Vector3 magnitude = Vector3.one;
	public Vector3 cycle = Vector3.one * 100;
	public bool ignoreTimeScale = false;
	public iTween.EaseType easeType = iTween.EaseType.linear;

	Vector3 original;

    void Start()
    {
		ShakeCamera ();
    }

	public void ShakeCamera ()
	{
		StartCoroutine (ShakeCroutine());
	}

	IEnumerator ShakeCroutine ()
    {
		original = Camera.main.transform.localPosition;

    	if (ignoreTimeScale) {
    		yield return ExtUtility.WaitForRealSeconds(start);
    	}
    	else {
			yield return new WaitForSeconds(start);
		}

        iTween.ValueTo(
			this.gameObject,
            iTween.Hash(
                "time", time,
				"from", 1f,
                "to",   0f,
				"easeType", easeType,
				"ignoretimescale", ignoreTimeScale,
				"onupdate", "OnUpdateShake",
				"oncompleted", "OnShakeCompleted"
            )
        );
    }

	void OnUpdateShake(float value)
    {
		float x = Mathf.Sin(Time.time*cycle.x) * value * magnitude.x;
		float y = Mathf.Sin(Time.time*cycle.y) * value * magnitude.y;
		float z = Mathf.Sin(Time.time*cycle.z) * value * magnitude.z;

		ShakeController sc = Camera.main.GetComponent<ShakeController>();
		sc.ApplyMagnitude(new Vector3(x, y, z));
    }

	void OnShakeCompleted(float value)
    {
		Camera.main.transform.localPosition = original;
    }
}

}
