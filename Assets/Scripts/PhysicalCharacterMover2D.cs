using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace eXTRIVAL
{

//TODO: 削除予定
public interface ICharacterMover
{
	float gravity { get; set; }

	float maxFallSpeed { get; set; }

	Vector3 velocity { get; set; }

	Vector3 controlledVelocity { get; }

	float groundFriction { get; set; }
	// 対壁摩擦
	bool enableStayOnCliff { get; set; }

	bool isDescendPlate { get; set; }

	bool isGrounded { get; }

	bool isFreezing { get; }

	float bounciness { get; set; }

	Vector3 platformNormal { get; }

	void MovePosition (Vector3 deltaPosition);

	void Fly ();
}

// 移動コントローラ
[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Collider2D))]
public class PhysicalCharacterMover2D : MonoBehaviour, ICharacterMover
{
	static int StageLayer;
	static int IgnoreCollisionLayer;
	// 離陸判定になるまでの遅延フレーム数
	static readonly int FlyingDelayCount = 2;

	// デバグ用フラグ
	public bool debug = false;
	// 地面として認識する角度
	public float slopeLimit = 30.0f;
	// 掴み用コリジョン
	public Collider2D hangonCollision;
	// 自動着地が有効か？
	public bool enableAutoLanding = true;

	// 落下防止機能が有効か？
	public bool enableStayOnCliff { get; set; }
	// 重力
	public float gravity { get; set; }
	// 最大落下速度
	public float maxFallSpeed { get; set; }

	// 対地摩擦
	public float groundFriction { get; set; }
	// 板属性コリジョンの通りぬけ制御の有無
	public bool isDescendPlate { get; set; }

	Collider2D fallPreventionCollider_;
	bool isGrounded_ = false;
	bool isFreezing_ = false;
	Collider2D platform_;
	Vector3 platformNormal_;
	Vector3 platformLocalPosition_;
	Vector3 platformWorldPosition_;
	Vector3 moveVector_ = Vector3.zero;
	PhysicsMaterial2D physicalMaterial_;
	Collider2D[] colliders_;
	Rigidbody2D rigidbody2D_;
	Vector2 velocity_ = Vector2.zero;
	Vector2 movedVelocity_ = Vector2.zero;

	// 地面として接触中のコライダリスト
	HashSet<Collider2D> contactColliders_ = new HashSet<Collider2D> ();

	// 空中にいるとの判断を遅延させるためのカウンタ
	int flyingDelayCount_ = 0;

	// 移動ベクトル
	public Vector3 velocity {
		get { return rigidbody2D_ != null ? rigidbody2D_.velocity : Vector2.zero; }
		set { 
			velocity_ = value;
			if (isFreezing)
				return;
			rigidbody2D_.velocity = value;
		}
	}

	public Vector3 controlledVelocity {
		get { return velocity_; }
	}

	public Vector2 movedVelocity {
		get { return movedVelocity_; }
	}

	public bool isGrounded {
		get { return isGrounded_; }
	}

	public bool isFreezing { 
		get { return isFreezing_; }
	}

	public float friciton {
		get { return physicalMaterial_.friction; }
		set {
			if (physicalMaterial_.friction == value)
				return;
			physicalMaterial_.friction = value; 
			_FixPhysicMaterial (); 
		}
	}

	public float bounciness {
		get { return physicalMaterial_.bounciness; }
		set {
			if (physicalMaterial_.bounciness == value)
				return;
			physicalMaterial_.bounciness = value;
			_FixPhysicMaterial ();
		}
	}

	public Vector3 platformNormal {
		get { return platformNormal_; }
	}

	void _FixPhysicMaterial ()
	{
		foreach (Collider2D col in colliders_) {
			col.enabled = false;
			col.enabled = true;
		}
	}

	void Awake ()
	{
		colliders_ = GetComponents<Collider2D> ();
		rigidbody2D_ = GetComponent<Rigidbody2D> ();
			
		StageLayer = LayerMask.NameToLayer ("Stage");

		rigidbody2D_.gravityScale = 1.0f;
		rigidbody2D_.constraints = RigidbodyConstraints2D.FreezeRotation;
		rigidbody2D_.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		          
		physicalMaterial_ = new PhysicsMaterial2D ();
		physicalMaterial_.friction = 0;
		physicalMaterial_.bounciness = 0;
		
		foreach (Collider2D col in colliders_) {
			col.sharedMaterial = physicalMaterial_;
		}
	}

	void Start ()
	{
		gravity = 90;
		maxFallSpeed = 40;
		_CreateFallPrevention ();
	}

	// 落下防止のためのオブジェクトを生成
	void _CreateFallPrevention ()
	{
		var go = new GameObject ("FallPrevention");
		go.transform.parent = this.transform;
		go.layer = StageLayer;
			
		var pm = new PhysicsMaterial2D ();
		pm.bounciness = 0;
		pm.friction = 0;

		var co = go.AddComponent<BoxCollider2D> ();
		co.sharedMaterial = pm;
		co.size = new Vector2 (0.5f, 2);
		co.offset = new Vector2 (0, 1);
			
		var ignoreco = go.AddComponent<BoxCollider2D> ();
		ignoreco.isTrigger = true;
		ignoreco.size = new Vector2 (10, 10);
							
		var fp = go.AddComponent<FallPrevention> ();
		fp.collider2D_ = co;
		fp.ignoreColiider2D = ignoreco;
		fp.owner = this;

		fallPreventionCollider_ = co;
	}

	// 強制離陸
	public void Fly ()
	{
		isGrounded_ = false;
		if (debug)
			print ("to fly");	
	}

	// 移動
	public void MovePosition (Vector3 deltaPosition)
	{
		moveVector_ += deltaPosition;
	}

	void FixedUpdate ()
	{
		_UpdatePlatform ();

		if (isFreezing)
			return;

		fallPreventionCollider_.gameObject.SetActive (enableStayOnCliff && isGrounded);

		var mvel = new Vector2 (moveVector_.x, moveVector_.y) / Time.deltaTime;
																		
		if (bounciness == 0) {
			velocity_ = velocity_ - Vector2.up * (gravity * Time.deltaTime);
			velocity_ = new Vector2 (velocity_.x, Mathf.Max (velocity_.y, -maxFallSpeed));
			if (isGrounded) {
				velocity_ *= (1.0f - groundFriction);
			}
			rigidbody2D_.velocity = velocity_;
				
			movedVelocity_ = velocity_ + mvel;
			if (isGrounded_) {
				//rigidbody2D_.MovePosition (rigidbody2D_.position + movedVelocity_*Time.deltaTime);
				rigidbody2D_.MovePosition (rigidbody2D_.position + movedVelocity_ * Time.deltaTime + Vector2.down * 0.1f);
			} else {
				rigidbody2D_.MovePosition (rigidbody2D_.position + movedVelocity_ * Time.deltaTime);
			}
		} else {
			velocity_ = velocity - Vector3.up * (gravity * Time.deltaTime);
			velocity_ = new Vector2 (velocity_.x, Mathf.Max (velocity_.y, -maxFallSpeed));				
			rigidbody2D_.velocity = velocity_;
		}
			
		moveVector_ = Vector2.zero;
			
		if (platform_ != null) {
			platform_.transform.localToWorldMatrix.MultiplyPoint (platformLocalPosition_);
		}
				
		if (debug)
			Debug.DrawLine (rigidbody2D_.position, rigidbody2D_.position + velocity_ * 0.2f, Color.white);
	}

	void OnCollisionEnter2D (Collision2D collision)
	{
		foreach (ContactPoint2D contact in collision.contacts) {	
			bool isGroundContact = Vector3.Angle (Vector3.up, contact.normal) < slopeLimit ? true : false;
			_ChangeContactColliders (isGroundContact, contact.collider);
		}
		bool isGround = contactColliders_.Count != 0 ? true : false;
		SendMessage ("OnContactedMover", isGround, SendMessageOptions.DontRequireReceiver);
		_ChangePlatform ();		
	}

	void OnCollisionExit2D (Collision2D collision)
	{
		foreach (ContactPoint2D contact in collision.contacts) {
			_ChangeContactColliders (false, contact.collider);
		}
		_ChangePlatform ();		
	}

	// 接地コリジョンの変更
	void _ChangeContactColliders (bool isGround, Collider2D collider)
	{
		if (isGround) {
			if (!contactColliders_.Contains (collider)) {
				contactColliders_.Add (collider);
			}
		} else {
			if (contactColliders_.Contains (collider)) {
				contactColliders_.Remove (collider);
			}
		}
	}

	// 接地状態の変更
	// 接地した瞬間を着地とする。
	void _ChangePlatform ()
	{
		bool isGround = contactColliders_.Count != 0 ? true : false;
		if (isGround) {
			flyingDelayCount_ = 0;
		}

		// 着地判定
		if (isGrounded == false && isGround == true) {
			if (enableAutoLanding && bounciness == 0) {
				if (debug)
					print ("landing mover");
				SendMessage ("OnLandingMover", isGround, SendMessageOptions.DontRequireReceiver);
				isGrounded_ = true;

			}
		// 離陸判定（２フレーム遅延させる）
		} else if (isGrounded == true && isGround == false) {
			if (flyingDelayCount_ == 0) {
				flyingDelayCount_ = FlyingDelayCount;
			}
		}
	}

	// 接地状態の監視
	// ２フレーム以上無接地状態が続くと離陸とする。
	void _UpdatePlatform ()
	{
		bool isGround = contactColliders_.Count != 0 ? true : false;

		// 離陸判定
		if (!isGrounded_)
			return;

		if (flyingDelayCount_ > 0) {
			flyingDelayCount_--;
			if (flyingDelayCount_ == 0) {
				if (debug)
					print ("flying mover");
				SendMessage ("OnFlyingMover", SendMessageOptions.DontRequireReceiver);
				isGrounded_ = false;
				platform_ = null;
			}
		}
	}

	public void OnFreeze ()
	{
		isFreezing_ = true;
		velocity_ = rigidbody2D_.velocity;
		rigidbody2D_.velocity = Vector2.zero;
	}

	public void OnUnfreeze ()
	{
		isFreezing_ = false;
		rigidbody2D_.velocity = velocity_;
	}

	public void OnDrawGizmos ()
	{
		if (Camera.current != Camera.main)
			return;
		if (isGrounded) {
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube (transform.position, new Vector3 (1 * 2, 0, 2));
		} else {
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube (transform.position, new Vector3 (1 * 2, 0, 2));
			//Gizmos.DrawIcon(hangPosition_, "dot.png");    
		}
		if (GetComponent<Rigidbody2D> ()) {
			Gizmos.DrawLine (transform.position, transform.position + velocity.normalized * 1.5f);
		}
	}

	public static void IgnoreCollision (Collider2D target)
	{
		var hits = Physics2D.OverlapCircleAll (target.transform.position, 4, 1 << StageLayer);
		foreach (var hit in hits) {
			var fp = hit.GetComponent<FallPrevention> ();
			if (fp) {
				fp.IgnoreCollision (target);
			}
		}
	}
}
}

