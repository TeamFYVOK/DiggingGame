using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace eXTRIVAL {

/**
 * FixedUpdateの後処理呼び出し
 *   
 * すべてのFixedUpdateの処理終了後にLateFixedUpdateを呼び出す。
 * ただし,このコンポーネントが埋め込まれたオブジェクトの子オブジェクトのみが対象。
 */
public class LateFixedUpdateManager : SingletonMonoBehaviour<LateFixedUpdateManager>
{
	List<GameObject> objects_ = new List<GameObject>();
	bool processedFixedUpdate_ = false;
		
	protected override void Awake ()
	{
		base.Awake ();
		if (this != instance) return;

		DontDestroyOnLoad (this.gameObject);			
	}
	
	void FixedUpdate() 
	{
		if (processedFixedUpdate_) {
			BroadcastLateFixedUpdate ();
			processedFixedUpdate_ = false;
		}
		
		processedFixedUpdate_ = true;
	}
	
	void Update() 
	{
		if (processedFixedUpdate_) {
			BroadcastLateFixedUpdate ();
			processedFixedUpdate_ = false;
		}
	}

	void BroadcastLateFixedUpdate ()
	{	
		foreach (GameObject obj in objects_) {
			obj.BroadcastMessage("LateFixedUpdate", SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void Add (GameObject obj)
	{
		if (!isInstanced) return;		
		if (obj == null) return;
		instance.objects_.Add(obj);
	}
	
	public static void Remove (GameObject obj)
	{
		if (!isInstanced) return;		
		if (obj == null) return;
		instance.objects_.Remove(obj);
	}
}

}

