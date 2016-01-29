using UnityEngine;
using System.Collections;

namespace eXTRIVAL {

// iTween を用いた回転
public class ITweenRotation : MonoBehaviour 
{
	public enum Space {
		Local,
		World
	};

	public Space space = Space.Local;
	public Vector3 to;
	public float time = 1;
	public bool ignoreTimeScale = false;
	public iTween.EaseType easeType = iTween.EaseType.linear;

    private void Start()
    {
    	if (space == Space.World) {
			iTween.RotateTo(this.gameObject, iTween.Hash(
		        "rotation", to + transform.rotation.eulerAngles,
		        "time", time, 
		        //"oncomplete", "complete", 
		        //"oncompletetarget", this.gameObject, 
				"easeType", easeType,
				"ignoretimescale", ignoreTimeScale
	    	));
	    }
	    else {
			iTween.RotateTo(this.gameObject, iTween.Hash(
				"rotation", to + transform.localRotation.eulerAngles,
		        "time", time, 
		        //"oncomplete", "complete", 
		        //"oncompletetarget", this.gameObject, 
				"islocal", true,
				"easeType", easeType,
				"ignoretimescale", ignoreTimeScale
	    	));
	    }
    }

}

}
