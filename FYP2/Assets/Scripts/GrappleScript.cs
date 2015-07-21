using UnityEngine;
using System.Collections;

public class GrappleScript : MonoBehaviour {

	public bool isGrappling = false;
	public float ascSpeed = 0.1f;			// player's ascend/descend spd (scale spd)
	
	public GameObject grapplePrefab;
	// throwing force of grapple		(-, upward force, forward force)
	//public Vector3 throwForce = new Vector3(0, 400f, 100f);		
	public float throwForce = 100f;

	GameObject theGrapple;
	Transform grappleHook; 
	Transform grappleEnd; 
	Transform grappleRope;
	Transform rightHand;

	float maxRopeScale;
	float ropeLength;

	void Start() {
		rightHand = Camera.main.transform.GetChild(0);

		//maxRopeScale = grapplePrefab.transform.GetChild(1).localScale.y;
		ropeLength = grapplePrefab.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().bounds.size.y;
		print (ropeLength);
	}


	void Update() {
		if(Input.GetButtonDown("Grapple")) 	
		{
			if(theGrapple == null)				
				FireGrapple();
			else
				ReleaseGrapple();
		}

		if(theGrapple == null)
			return;

		grappleEnd.position = rightHand.position;

		// Ascend
		if(Input.GetButton("Fire2"))
		{				
			if(grappleRope.localScale.x > 0.1f)
			{
				print (grappleRope.localScale.x);
				TraverseHierarchy(grappleRope, ascSpeed);	// update rope bones scale
			}
		}
		
		// Descend
		if(Input.GetButton("Fire3")) 
		{					
			if(grappleRope.localScale.x < ropeLength)
			{
				print (grappleRope.localScale.x);
				TraverseHierarchy(grappleRope, -ascSpeed);	// update rope bones scale
			}
		}

		// check if player use grapple successfully (hooks sth)
		GrappleCollisionScript hook = grappleHook.GetComponent<GrappleCollisionScript>();
		if(hook.GetGrappleHooked())		
		{
			if(!this.GetComponent<CharacterController>().isGrounded)
				this.GetComponent<Rigidbody>().isKinematic = false;			// enable player rb
			else
				this.GetComponent<Rigidbody>().isKinematic = true;			// disable player rb
		}
//			// Ascend
//			if(Input.GetButton("Fire2"))
//			{				
//				if(this.GetComponent<Rigidbody>().isKinematic)
//					this.GetComponent<Rigidbody>().isKinematic = false;	
//
//				if(grappleRope.localScale.x > 0.1f)
//				{
//					print (grappleRope.localScale.x);
//					TraverseHierarchy(grappleRope, ascSpeed);	// update rope bones scale
//
//				}
//			}
//
//			// Descend
//			if(Input.GetButton("Fire3")) 
//			{					
//				if(grappleRope.localScale.x < ropeLength)
//				{
//					print (grappleRope.localScale.x);
//					TraverseHierarchy(grappleRope, -ascSpeed);	// update rope bones scale
//				}
//			}
//		}
	}

	void TraverseHierarchy(Transform root, float _ascSpeed)
	{
		// rescale
		root.localScale -= Vector3.right*_ascSpeed;
		
		root.localScale = new Vector3(Mathf.Round(root.localScale.x*100f)/100f, 
		                              root.localScale.y, 
		                              root.localScale.z);

		// Make sure setting of asc/desc wont over scale 
		root.localScale = new Vector3(Mathf.Clamp(root.localScale.x, 0.1f, ropeLength), 
		                              root.localScale.y, 
		                              root.localScale.z);

		if(root.name == "Bone001")
		{
//			// get parent's end(btm) by its collider
//			Bounds parentColBounds = root.parent.GetComponent<Collider>().bounds;
//			//+ or -?
//			Vector3 parentEndPos = new Vector3(parentColBounds.center.x-parentColBounds.extents.x*2,
//			                                   parentColBounds.center.y,
//			                                   parentColBounds.center.z);
//			if(root.name == "Bone002")
//				print ("current pos: " + root.position + "parent end pos: " + parentEndPos);
//			// shift to endpos
//			root.localPosition = parentEndPos;
		}

		// move to next transform(child) in hierarchy (if any)
		if(root.childCount > 0)
			TraverseHierarchy(root.GetChild(0), _ascSpeed);
		else
		{
			print ("b4" + root.position + " " + grappleEnd.position);
			root.position = grappleEnd.position;
			print ("after" + root.position + " " + grappleEnd.position);
		}
	}

	public void FireGrapple()
	{
		isGrappling = true;

		// Spawn grapple on player
		theGrapple = (GameObject)Instantiate(grapplePrefab, 
		                                     rightHand.position, 
		                                     rightHand.rotation);
		
		grappleHook = theGrapple.transform.GetChild(0);
		grappleRope = theGrapple.transform.GetChild(1);
		grappleEnd = theGrapple.transform.GetChild(2);
		grappleEnd.transform.position = rightHand.position;

		this.gameObject.AddComponent<FixedJoint>().connectedBody = grappleEnd.GetComponent<Rigidbody>();

		// Throw grapple (adding force to hook i.e. front of grapple)
		Rigidbody theGrappleHook_rb = grappleHook.GetComponent<Rigidbody>();
		theGrappleHook_rb.AddForce(Camera.main.transform.forward*throwForce-Camera.main.transform.up*throwForce*0.5f,
		                           ForceMode.Impulse);

		// #TO DO
		// use raycast to get dist how far player is to grappable obj. 
		// if dist is > total length of grapple, player cant use grapple (since his grapple wont reach)

		// if raycast dist is within size,
		// use the dist to spawn grapple alr ascended/descended
		// (e.g. rayc. dist roughly is 19units length, whereas grapple size is 30u. 
		// So spawn grapple ascended - i.e. 'shortened' with 19u length)
	}
	
	public void ReleaseGrapple()
	{
		isGrappling = false;
		Destroy(this.GetComponent<FixedJoint>());
		Destroy(theGrapple);
		theGrapple = null;
		this.gameObject.GetComponent<Rigidbody>().isKinematic = true;	
	}
}