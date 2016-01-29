using UnityEngine;
using System.Collections;

namespace eXTRIVAL {

/**
 * カプセル型
 */
[System.Serializable]
public class CapsuleValue : System.ICloneable
{
	public float radius = 1;
	public float height = 3;
	public Vector3 center = new Vector3(0, 1.5f, 0);
	
    public System.Object Clone() 
	{
		return MemberwiseClone();
    }
    
    public void DrawGizmo(Vector3 position, Color color) 
	{
		Vector3 p1 = position + center + (height*0.5f-radius)*Vector3.down;
		Vector3 p2 = p1 + (height-radius)*Vector3.up;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(p1, radius);
        Gizmos.DrawWireSphere(p2, radius);
	}
}

/**
 * コリジョン関係のユーティリティ
 */
static
public class ColliderUtility 
{
	
	public static CapsuleCollider AddCapsuleCollider(GameObject self, float radius, float height, Vector3 center) 
	{
		RemoveCollider(self);
		CapsuleCollider co = self.AddComponent<CapsuleCollider>();
		co.radius = radius;
		co.height = height;
		co.center = center;
		return co;
	}

	public static BoxCollider AddBoxCollider(GameObject self, Vector3 size, Vector3 center) 
	{
		RemoveCollider(self);
		BoxCollider co = self.AddComponent<BoxCollider>();
		co.center = center;
		co.size = size;
		return co;
	}
	
	
    public static void RemoveCollider(GameObject self) 
    {
		Collider co = self.GetComponent<Collider>();
		if (co) GameObject.Destroy(co);
    }
	
}

}
