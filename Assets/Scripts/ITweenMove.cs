using UnityEngine;
using System.Collections;

namespace eXTRIVAL {

public class ITweenMove : MonoBehaviour 
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
			iTween.MoveTo(this.gameObject, iTween.Hash(
		        "position", to + transform.position,
		        "time", time, 
		        //"oncomplete", "complete", 
		        //"oncompletetarget", this.gameObject, 
				"easeType", easeType,
				"ignoretimescale", ignoreTimeScale
	    	));
	    }
	    else {
			iTween.MoveTo(this.gameObject, iTween.Hash(
				"position", to + transform.localPosition,
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
