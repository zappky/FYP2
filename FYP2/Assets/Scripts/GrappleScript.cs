using UnityEngine;
using System.Collections;

public class GrappleScript : MonoBehaviour {

	public float ascSpeed = 0.1f;					// player's ascend/descend spd (scale spd)
	public float ascDelay = 0.01f;					// player's ascend/descend input delay (more is slower)
	
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
	Transform rightHand;

	int currentRope;				// current rope (child) in grapple player is at 
	int totalRopeCt;				// total rope (child) in grapple 
	float ascInputDelay;			// asc/desc input delay
	
	//FpsMovement FPSController;
	
	void Start() {
		//FPSController = this.GetComponent<FpsMovement>();
		rightHand = Camera.main.transform.FindChild(RHandName);

		currentRope = 0;
		// Get current rope player is on (to be done when fired if doing raycasting for grapple - see #NOTE1)
		foreach(Transform child in grapplePrefab.transform)
		{
			if(child.name != GrappleHookName 
			&& child.name != GrappleEndName
			&& child.name != "GrappleEnd(Connector)")	// if child is a rope
				++currentRope;
		}

		totalRopeCt = currentRope;

		ascInputDelay = 0;
	}


	void Update() {
		if(Input.GetButtonDown("Grapple")) 	
		{
			if(theGrapple == null)				// if player havent launch grapple
			{
				FireGrapple();
				//FPSController.fireGrapple();
			}
			else
			{
				ReleaseGrapple();
				//FPSController.releaseGrapple();
			}
		}

		if(theGrapple != null)
		{
			GrappleCollisionScript hook = grappleHook.GetComponent<GrappleCollisionScript>();

			// check if player use grapple successfully (hooks sth)
			if(hook.GetGrappleHooked())		
			{
				this.gameObject.GetComponent<Rigidbody>().isKinematic = false;			// enable rigidbody

				// current rope
				Transform grappleRope = theGrapple.transform.GetChild(currentRope);

				grappleRope.GetComponentInChildren<Renderer>().enabled = true;
		
				ascInputDelay -= Time.deltaTime;

				// Ascend
				if(Input.GetButton("Fire1") && ascInputDelay <= 0) 
				{
					ascInputDelay = ascDelay;

					grappleRope.localScale -= Vector3.up*ascSpeed;
					
					// Make sure setting of asc/desc wont over scale 
					grappleRope.localScale = new Vector3(grappleRope.localScale.x,
					                                     Mathf.Clamp(grappleRope.localScale.y, 0, 1),
					                                     grappleRope.localScale.z);

					// btm of grappleEnd to cur. rope
					//grappleEnd.transform.position = grappleRope.position;
					Vector3 ropeBtmPos = new Vector3(grappleRope.GetComponentInChildren<Renderer>().bounds.center.x, 
					                                 grappleRope.GetComponentInChildren<Renderer>().bounds.center.y-
					                                 grappleRope.GetComponentInChildren<Renderer>().bounds.extents.y,
					                                 grappleRope.GetComponentInChildren<Renderer>().bounds.center.z);

					grappleEnd.transform.position = ropeBtmPos;
					
					// re-hinge to current rope
					grappleEnd.GetComponent<HingeJoint>().connectedBody = grappleRope.GetComponent<Rigidbody>();

					// if reached limit for current rope, go to next one
					if(grappleRope.localScale.y <= 0)
					{
						// if still got other rope to asc
						if(currentRope > 1)
						{
							grappleRope.GetComponentInChildren<Renderer>().enabled = false;
							--currentRope;
						}
						else
						{
							grappleRope.localScale = new Vector3(grappleRope.localScale.x,
							                                     Mathf.Clamp(grappleRope.localScale.y, 0.1f, 1),
							                                     grappleRope.localScale.z);
						}
					}
				}

				// Descend
				if(Input.GetButton("Fire2") && ascInputDelay <= 0) 
				{
					ascInputDelay = ascDelay;
					
					grappleRope.localScale += Vector3.up*ascSpeed;
					
					// Make sure setting of asc/desc wont over scale 
					grappleRope.localScale = new Vector3(grappleRope.localScale.x,
					                                     Mathf.Clamp(grappleRope.localScale.y, 0, 1),
					                                     grappleRope.localScale.z);	
					// btm of grappleEnd to cur. rope
					//grappleEnd.transform.position = grappleRope.position;
					Vector3 ropeBtmPos = new Vector3(grappleRope.GetComponentInChildren<Renderer>().bounds.center.x, 
					                                 grappleRope.GetComponentInChildren<Renderer>().bounds.center.y-
					                                 grappleRope.GetComponentInChildren<Renderer>().bounds.extents.y,
					                                 grappleRope.GetComponentInChildren<Renderer>().bounds.center.z);
					
					grappleEnd.transform.position = ropeBtmPos;
					
					// re-hinge to current rope
					grappleEnd.GetComponent<HingeJoint>().connectedBody = grappleRope.GetComponent<Rigidbody>();

					// if reached limit for current rope, go to next one
					if(grappleRope.localScale.y >= 1)
					{
						// if still got other rope to asc
						if(currentRope < totalRopeCt)
							++currentRope;
					}
				}
			}
		}
	}
	
	void FireGrapple()
	{
		Camera cam = Camera.main;
		Vector3 rightHandPos = rightHand.position;		// temp pos storage
		rightHandPos.y += grappleSpawnOffset.y;						// so we can offset spawn Y pos

		// Spawn grapple on player
		theGrapple = (GameObject)Instantiate(grapplePrefab, 
		                                     rightHandPos+
		                                     rightHand.forward*grappleSpawnOffset.x+
		                                     rightHand.right*grappleSpawnOffset.z, 
		                                     rightHand.rotation);

		// Get grapple's end (to let player 'hold on' to)
		grappleEnd = theGrapple.transform.FindChild(GrappleEndName);

		// Make player rHand hold grapple end
		this.gameObject.AddComponent<FixedJoint>().connectedBody = grappleEnd.GetComponent<Rigidbody>();

		grappleHook = theGrapple.transform.FindChild(GrappleHookName);

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
	
	void ReleaseGrapple()
	{
		theGrapple = null;

		Destroy(this.GetComponent<FixedJoint>());
		this.gameObject.GetComponent<Rigidbody>().isKinematic = true;	 
	}
}