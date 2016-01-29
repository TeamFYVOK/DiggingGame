using UnityEngine;
using System.Collections;

/**
 * シングルトン MonoBehaviour
 *
 * インスタンス取得時に見つからなかった場合、自動生成せずエラーメッセージを出力する。
 *
 * 参考：http://warapuri.tumblr.com/post/28972633000/unity-50-tips
 */
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour
	where T : SingletonMonoBehaviour<T>
{
	private static T _instance = null;
          
	/**
	 * インスタンスの取得
	 * 
	 * 既にシーン内に配置しているものがあればそれを返す。
	 * 取得したインスタンスが Awake() を通過しているとは限らないため注意が必要。
	 */
	public static T instance {
		get {
			if (_instance == null) _instance = (T)FindObjectOfType (typeof(T));
			if (_instance == null) {
				Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
			}
			return _instance;
		}
	}
	
	/**
	 * インスタンスが存在するか確認
	 */
	public static bool isInstanced {
		get { 
			if (_instance == null) _instance = (T)FindObjectOfType (typeof(T));
			return _instance != null;
		}
	}
	

	/**
	 * インスタンスの生成
	 * 
	 * 空のGameObjectを作って AddComponent() でインスタンスを生成する。
	 * すでにインスタンスがあれば null を返す。
	 */
	public static T CreateInstance()
	{
		if (isInstanced == true) return null;

		GameObject go = new GameObject ();
		go.name = typeof(T).ToString ();
		return go.AddComponent<T> ();
	}
	
	protected virtual void Awake() 
	{
		// インスタンス２重生成の抑制
		if (this != instance) {
 			foreach (Transform child in transform) {
        		GameObject.Destroy (child.gameObject);
    		}			
			GameObject.DestroyImmediate (this.gameObject);
		}
	}

	protected virtual void OnApplicationQuit()
	{
		_Clear();
	}
	
	protected virtual void OnDestroy ()
	{
		_Clear();
	}

	/**
	 * インスタンスの消去
	 */
	void _Clear()
	{
		if (_instance == this) {
			_instance = null;
		}		
	}	
}
