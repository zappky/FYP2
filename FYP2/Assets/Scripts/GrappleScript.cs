// Debug test for grapple's hook (i.e. grapple w/o the rope)
//#define DEBUGTEST_HOOK
using UnityEngine;
using System.Collections;

public class GrappleScript : MonoBehaviour {

	public float throwForce = 100f;
	public float adSpeed = 2;						// player's ascend/descend spd
	
	public string RHandName = "RHand";				// name of gameobj.
	public string GrappleHookName = "GrappleHook";	// name of gameobj.
	public string GrappleEndName = "GrappleEnd";	// name of gameobj.

	// how far grapple will spawn from player's rHand (forward, y, right)
	public Vector3 grappleSpawnOffset = new Vector3(1.2f, 0, 0.5f);		
	public GameObject grapplePrefab;

	//bool inAir = false;
	GameObject theGrapple;
	Transform grappleHook; 
	Transform grappleEnd; 
	Transform rightHand;

	Vector3 grappleSize;
	Vector3 grappleAscend; 
	Vector3 grappleDescend; 
	
	FpsMovement FPSController;

	// Use this for initialization
	void Start () {
		FPSController = this.GetComponent<FpsMovement>();
		rightHand = Camera.main.transform.FindChild(RHandName);

		grappleAscend.Set(1, 1/adSpeed, 1);
		grappleDescend.Set(1, adSpeed, 1);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Grapple")) 	
		{
			if(theGrapple == null)			// if player havent launch grapple
			{
				FireGrapple();
				FPSController.fireGrapple();
			}
			else
			{
				ReleaseGrapple();
				FPSController.releaseGrapple();
			}
		}

		if(theGrapple != null)
		{
#if !DEBUGTEST_HOOK
			GrappleCollisionScript grappleHook = theGrapple.transform.GetChild(0).GetComponent<GrappleCollisionScript>();
#endif
			if(Input.GetButtonDown("Fire1")) 
			{
				Debug.Log("Ascend");
				//theGrapple.transform.localScale += grappleAscend;
			}

#if !DEBUGTEST_HOOK
			if(grappleHook.GetGrappleHooked())		// if player use grapple successfully (hooks sth)
			{
				this.gameObject.GetComponent<Rigidbody>().isKinematic = false;	// enable rigidbody

				// Ascend
				if(Input.GetButtonDown("Fire1")) 
				{
					Debug.Log("Ascend");
				}

				// Descend
				// scale max - grappleSize
			}
#endif
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

#if !DEBUGTEST_HOOK
		// Get grapple's end (to let player 'hold on' to)
		grappleEnd = theGrapple.transform.FindChild(GrappleEndName);

		// Make player rHand hold grapple end
		this.gameObject.AddComponent<FixedJoint>().connectedBody = grappleEnd.GetComponent<Rigidbody>();
		
		// Throw grapple (adding force to hook i.e. front of grapple)
		grappleHook = theGrapple.transform.FindChild(GrappleHookName);

#else
		grappleHook = theGrapple.transform;
#endif

		Rigidbody theGrappleHook_rb = grappleHook.GetComponent<Rigidbody>();
		theGrappleHook_rb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);

		// Just to get grapple size
//		grappleSize *= 0;
//		foreach(Transform child in theGrapple.transform)
//		{
//			grappleSize += child.GetComponent<Renderer>().bounds.size;
//		}
		// once we get size, use it for raycast dist
		// dist is how far player is to obj. being raycasted
		// if dist is > size, player cant use grapple (since his grapple wont reach)
	}
	
	void ReleaseGrapple()
	{
		theGrapple = null;

//		foreach(FixedJoint jt in rightHand.GetComponents<FixedJoint>())
//		{
//			if(jt.connectedBody == grappleEnd.GetComponent<Rigidbody>())
//				Destroy(jt); // remove joint from player's rHand with grapple
//		}
		Destroy(this.GetComponent<FixedJoint>());
		this.gameObject.GetComponent<Rigidbody>().isKinematic = true;	 

	}
}