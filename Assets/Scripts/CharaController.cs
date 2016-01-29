using UnityEngine;
using System.Collections;

public class CharaController : MonoBehaviour
{

    // 変数の定義と初期化
	public float flap;
	public float scroll;

    //ショット
    public GameObject prefabShot;

    private bool flagJamp = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector2.right * scroll);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector2.left * scroll);
        }

        // スペースキーが押されたら
        if (Input.GetKeyDown("space") && flagJamp == false)
        {
            // 落下速度をリセット
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            // (0,1)方向に瞬間的に力を加えて跳ねさせる
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * flap, ForceMode2D.Impulse);

            flagJamp = true;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Vector3 pos = transform.position;
            pos.x += 1f;
            pos.y += 2f;
            GameObject shot = Instantiate(prefabShot, pos, transform.rotation) as GameObject;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        flagJamp = false;
    }
}
