using UnityEngine;
using System.Collections;

public class GrappleCollisionScript : MonoBehaviour {

	bool isHooked = false;					//to check if grapple hooked to sth successfully

	Rigidbody theGrappleHook_rb;
	FixedJoint grabJoint;

	void Start () {
		theGrappleHook_rb = this.GetComponent<Rigidbody>();
	}

	void Update () {
		if(!isHooked)
		{ 
		}
	}

	// Hook's collision with sth
	void OnCollisionEnter(Collision col)
	{
		//if gameobj dont have rb or !isKinematic, or players hand, return
		if(col.rigidbody == null || !col.rigidbody.isKinematic 
		|| col.transform == Camera.main.transform.GetChild(0))	
			return;

		if(!isHooked)	 
		{
			theGrappleHook_rb.velocity *= 0;	//stop hook's movement
			isHooked = true;
			
			// create joint to obj it collided with 
			grabJoint = this.gameObject.AddComponent<FixedJoint>();
			grabJoint.connectedBody = col.rigidbody;
		}
	}

	public void SetGrappleHooked(bool isHooked)
	{
		this.isHooked = isHooked;
	}
	
	public bool GetGrappleHooked()
	{
		return isHooked;
	}
}