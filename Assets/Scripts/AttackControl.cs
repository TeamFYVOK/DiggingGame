using UnityEngine;
using System.Collections;

public class AttackControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        //ブロックに当たったらブロックを削除
        if (coll.gameObject.tag == "Block")
        {
            Destroy(coll.gameObject);
            Destroy(gameObject);
        }
    }
}
