using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace eXTRIVAL
{

public static class ExtUtility
{
	// 指定時間の間コルーチンを停止（実時間）
	public static WaitWhile WaitForRealSeconds(float time)
    {
		float start = Time.realtimeSinceStartup;
    	return new WaitWhile( () => Time.realtimeSinceStartup < start + time );
     }

    //  再帰的に指定しない名前オブジェクトを検索 
    // Transform 拡張メソッド
	public static Transform FindRecursive ( 
		this Transform self, 
        string name, 
        bool includeInactive = false )
    {
        var children = self.GetComponentsInChildren<Transform>( includeInactive );
        foreach ( var transform in children )
        {
            if ( transform.name == name )
            {
                return transform;
            }
        }
        return null;
    }

    // 再帰的に親オブジェクトを取り出す
	public static GameObject[] GetParents (GameObject go)
	{
		List<GameObject> result = new List<GameObject>();
		while (go.transform.parent != null) {
			go = go.transform.parent.gameObject;
			result.Add(go);
		}		
		return result.ToArray();
	}

	/**
	 * 保持しているMeshを取得
	 *
	 * targetに含まれるSkinnedMeshRenderer, MeshFilterからMesh部分を取り出す。
	 * @param target Meshを描画するオブジェクト
	 */
	public static Mesh GetMesh(GameObject target)
	{
		SkinnedMeshRenderer smesh = target.GetComponent(typeof (SkinnedMeshRenderer)) as SkinnedMeshRenderer;
		if (smesh != null) {
			return smesh.sharedMesh;
		}
		MeshFilter rmesh = target.GetComponent(typeof(MeshFilter)) as MeshFilter;
		if (rmesh != null) {
			return rmesh.sharedMesh;
		}
		
		return null;
	}

	/**
	 *   未使用変数の宣言
	 */
	public static void Unused<T> (T value)
	{
		T unusedValue = value;
	}
}

}