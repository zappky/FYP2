using UnityEngine;
using System.Collections;

public class GrappleScript : MonoBehaviour {

	public bool isGrappling = false;
	public float ascSpeed = 0.1f;					// player's ascend/descend spd (scale spd)

	public string RHandName = "RHand";				// name of gameobj.
	public string GrappleHookName = "GrappleHook";	// name of gameobj.
	public string GrappleEndName = "GrappleEnd";	// name of gameobj.
	
	public GameObject grapplePrefab;
	// how far grapple will spawn from player's rHand (forward, y, right)
	public Vector3 grappleSpawnOffset = new Vector3(1.2f, 0, 0.5f);	
	// throwing force of grapple		(-, upward force, forward force)
	public Vector3 throwForce = new Vector3(0, 400f, 100f);		

	GameObject theGrapple;
	Transform grappleHook; 
	Transform grappleEnd; 
	Transform grappleRope;
	Transform rightHand;

	float maxRopeScale;
	float ropeLength;

	void Start() {
		rightHand = Camera.main.transform.FindChild(RHandName);

		//maxRopeScale = grapplePrefab.transform.GetChild(1).localScale.y;

		ropeLength = grapplePrefab.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().bounds.size.y;
	}


	void Update() {
		if(Input.GetButtonDown("Grapple")) 	
		{
			if(theGrapple == null)				// if player havent launch grapple
			{
				FireGrapple();
			}
			else
			{
				ReleaseGrapple();
			}
		}

		if(theGrapple != null)
		{
			GrappleCollisionScript hook = grappleHook.GetComponent<GrappleCollisionScript>();

			// check if player use grapple successfully (hooks sth)
			if(hook.GetGrappleHooked())		
			{
				if(!this.GetComponent<CharacterController>().isGrounded)
					this.GetComponent<Rigidbody>().isKinematic = false;			// enable player rb
				else
					this.GetComponent<Rigidbody>().isKinematic = true;			// disable player rb

				// Ascend
				if(Input.GetButton("Fire2"))
				{				
					if(this.GetComponent<Rigidbody>().isKinematic)
						this.GetComponent<Rigidbody>().isKinematic = false;	

					if(grappleRope.localScale.x > 0.1f)
					{
						print (grappleRope.localScale.x);
						TraverseHierarchy(grappleRope, ascSpeed);	// update rope bones scale
					}
				}

				// Descend
				if(Input.GetButton("Fire3")) 
				{										
					if(grappleRope.localScale.x < maxRopeScale)
					{
						print (grappleRope.localScale.x);
						TraverseHierarchy(grappleRope, -ascSpeed);	// update rope bones scale
					}
				}
			}
		}
	}

	void TraverseHierarchy(Transform root, float _ascSpeed)
	{
		root.GetComponent<Rigidbody>().isKinematic = true;		// 'freeze' rb

		// rescale
		root.localScale -= Vector3.right*_ascSpeed;

		// Make sure setting of asc/desc wont over scale 
		root.localScale = new Vector3(Mathf.Clamp(root.localScale.x, 0.1f, maxRopeScale), 
		                              root.localScale.y, 
		                              root.localScale.z);

		root.localScale = new Vector3(Mathf.Round(root.localScale.x*100f)/100f, 
		                              root.localScale.y, 
		                              root.localScale.z);

		if(root.name == "Bone001")
		{
			print (root.GetChild(0).localPosition);
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

		root.GetComponent<Rigidbody>().isKinematic = false;		// 'unfreeze' rb

		// move to next transform(child) in hierarchy (if any)
		if(root.childCount > 0)
			TraverseHierarchy(root.GetChild(0), _ascSpeed);
	}

	public void FireGrapple()
	{
		isGrappling = true;
		Camera cam = Camera.main;
		Vector3 rightHandPos = rightHand.position;				// temp pos storage
		rightHandPos.y += grappleSpawnOffset.y;					// so we can offset spawn Y pos

		// Spawn grapple on player
		theGrapple = (GameObject)Instantiate(grapplePrefab, 
		                                     rightHandPos+
		                                     rightHand.forward*grappleSpawnOffset.x+
		                                     rightHand.right*grappleSpawnOffset.z, 
		                                     rightHand.rotation);
		
		grappleRope = theGrapple.transform.GetChild(1);

		// Get grapple's end (to let player 'hold on' to)
		grappleEnd = theGrapple.transform.FindChild(GrappleEndName);

		// Make player rHand hold grapple end
		this.gameObject.AddComponent<FixedJoint>().connectedBody = grappleEnd.GetComponent<Rigidbody>();

		grappleHook = theGrapple.GetComponentInChildren<GrappleCollisionScript>().transform;
		grappleHook.position = rightHandPos;

		// Throw grapple (adding force to hook i.e. front of grapple)
		Rigidbody theGrappleHook_rb = grappleHook.GetComponent<Rigidbody>();
		theGrappleHook_rb.AddForce(cam.transform.forward*throwForce.z + cam.transform.up*throwForce.y, 
		                           ForceMode.Impulse);

		// #NOTE1 - if got time to do 
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
		theGrapple = null;
		this.gameObject.GetComponent<Rigidbody>().isKinematic = true;	
		Destroy(this.GetComponent<FixedJoint>());
	}
}