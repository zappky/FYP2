using UnityEngine;
using System.Collections;

public class GrappleCollisionScript : MonoBehaviour {

	public float throwForce = 100f;
	
	private bool isHooked = false;					//to check if grapple hooked to sth successfully
	//private GameObject theGrapple;

	private Rigidbody theGrappleHook_rb;
	private HingeJoint grabHinge;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("I AM HOOK: " + this.name);
		if(isHooked)
		{ 
		}
	}

	// Hook's collision with sth
	void OnCollisionEnter(Collision col)
	{
		if(col.rigidbody == null || !col.rigidbody.isKinematic || col.gameObject.name == "PlayerController")	//if that sth dont have rb or isKinematic, return
			return;
		Debug.Log(col.gameObject.name);

		// check if wall/obj can be 'hookable'
		//if(col.gameObject.name == "Hookable")

		if(!isHooked)	 
		{
			theGrappleHook_rb = this.GetComponent<Rigidbody>();
			theGrappleHook_rb.velocity *= 0;	//stop hook's movement
			isHooked = true;
			
			// create hingejoint to obj it collided with 
			grabHinge = this.gameObject.AddComponent<HingeJoint>();
			grabHinge.connectedBody = col.rigidbody;
		}
	}
}