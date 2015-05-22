using UnityEngine;
using System.Collections;

public class GrappleCollisionScript : MonoBehaviour {

	public float throwForce = 100f;
	
	private bool isHooked = false;					//to check if grapple hooked to sth successfully
	//private GameObject theGrapple;

	private Rigidbody theGrappleHook_rb;
	private FixedJoint grabJoint;
	
	// Use this for initialization
	void Start () {
		theGrappleHook_rb = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!isHooked)
		{ 
		}
	}

	// Hook's collision with sth
	void OnCollisionEnter(Collision col)
	{
		if(col.rigidbody == null || !col.rigidbody.isKinematic)	//if that sth dont have rb or isKinematic, return
			return;
		Debug.Log(col.gameObject.name);

		// check if wall/obj can be 'hookable'
		//if(col.tag == "Hookable")

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