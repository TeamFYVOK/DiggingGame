using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace eXTRIVAL {

// uGUI のカラーをTween
public class ITweenUIColor : MonoBehaviour 
{
	public float start = 0;
	public float time = 1;
	public MaskableGraphic target;
	public Color to;
	public bool ignoreTimeScale = false;
	public iTween.EaseType easeType = iTween.EaseType.linear;

	Color from;

    void Start()
    {
		StartCoroutine (StartTween());
    }

    public IEnumerator StartTween ()
    {
    	if (ignoreTimeScale) {
    		yield return ExtUtility.WaitForRealSeconds(start);
    	}
    	else {
			yield return new WaitForSeconds(start);
		}

		from = target.color;
        iTween.ValueTo(
			this.gameObject,
            iTween.Hash(
                "time", time,
				"from", 0f,
                "to",   1f,
				"easeType", easeType,
				"ignoretimescale", ignoreTimeScale,
				"onupdate", "OnUpdateColor"
            )
        );
    }

    void OnUpdateColor(float value)
    {
		this.target.color = Color.Lerp(from, to, value);
    }

}

}
