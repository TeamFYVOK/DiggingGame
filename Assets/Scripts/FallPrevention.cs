using UnityEngine;
using System.Collections;
using System.Linq;


namespace eXTRIVAL {

/*
 * 落下防止用コライダの設置制御
 */
public class FallPrevention : MonoBehaviour 
{
	static int kStageLayer = 0;
	const int kSpawnRange = 3;

	public PhysicalCharacterMover2D owner;
	public Collider2D collider2D_; // TODO: Rename
	public Collider2D ignoreColiider2D;

	Vector2 direction_;

	void Awake ()
	{
		kStageLayer = LayerMask.NameToLayer ("Stage");
	}
	
	// Use this for initialization
	void Start () 
	{
		Attend ();
	}
	
	void OnEnable ()
	{
		direction_ = Vector2.zero;		
		Attend ();
	}
	
	// 無効化する
	// GameObjectを無効化した際に判定を無効化する。
	void OnDisable ()
	{
		Ignore ();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (owner == null) return;
		
		Attend ();
	}
	
	// 有効化して設置場所を決定する
	void Attend ()
	{
		if (owner == null) return;
		
		// 設置方向を決定
		Vector2 direction = direction_;
		if (owner.movedVelocity.x != 0) {
			direction = owner.movedVelocity.x > 0 ? Vector2.right : -Vector2.right;
		}	
		
		// 方向が決まらなければ無効化する
		if (direction == Vector2.zero) {
			Ignore ();
			return;
		}
		
		// 設置するべきか？
		if (direction_ != direction || collider2D_.enabled == false) {
			
			Vector2 pos = owner.GetComponent<Rigidbody2D>().position - Vector2.up * 0.5f + direction * kSpawnRange;
			Vector2 size = new Vector2(0.5f, 2.0f);
			
			// 起点に障害物がないか調べる(トリガーと自身への衝突結果を無視)
			var overlap = Physics2D.OverlapAreaAll(pos-size/2, pos+size/2, 1 << kStageLayer)
				.Where(t => !t.isTrigger && t.gameObject != this.gameObject).LastOrDefault();
			if (overlap != null) {
				// 障害物があれば無効
				Ignore();
				return;
			}
				
			// 起点からキャストして崖際を検出(トリガーと自身への衝突結果を無視)
			var hit = Physics2D.BoxCastAll (pos, size, 0, -direction, kSpawnRange, 1 << kStageLayer)
				.Where(t => !t.collider.isTrigger && t.collider.gameObject != this.gameObject).LastOrDefault();
			if (hit.distance == 0) {
				// 接触がなければ無効
				Ignore ();
				return;
			}
			//print ("attend " + hit.collider + " count:" + hits.Count());
			
			// 有効化する				
			collider2D_.enabled = true;
			transform.position = hit.centroid;
			transform.parent = owner.transform.parent;

			direction_ = direction;
		}
	}
	
	// 無効化する
	void Ignore ()
	{
		collider2D_.enabled = false;
	}
	
	// 対オーナー以外の判定を無効化する
	void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.gameObject.layer == kStageLayer) return;
		if (collider.gameObject == owner.gameObject) return;
		IgnoreCollision (collider);
	}
	
	// Collider同士の無効化
	public void IgnoreCollision (Collider2D collider)
	{
		Physics2D.IgnoreCollision (collider2D_, collider, true);		
	}
	
		void OnDrawGizmos ()
		{
			if (Camera.current != Camera.main) return;
			
			var bc = collider2D_ as BoxCollider2D;
			if (bc != null && bc.enabled) {
				Gizmos.color = new Color(1, 0, 0.7f, 0.5F);				
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.DrawCube(bc.offset, bc.size);
			}
			
		}
		
}
}