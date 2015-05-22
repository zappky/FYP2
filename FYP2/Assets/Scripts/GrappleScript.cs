using UnityEngine;
using System.Collections;

public class GrappleScript : MonoBehaviour {
	
	//public GameObject Player;
	public GameObject grapplePrefab;
	public Vector3 grappleSpawnPosOffset = new Vector3();
	public float throwForce = 100f;
	public float adSpeed = 2;				// player's ascend/descend spd

	//private bool inAir = false;
	private GameObject theGrapple;
	private Rigidbody theGrappleHook_rb;
	private Vector3 grappleSize;
	private Vector3 grappleAscend; 
	private Vector3 grappleDescend; 
	
	private FpsMovement FPSController;

	// Use this for initialization
	void Start () {
		FPSController = this.GetComponent<FpsMovement>();

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
			GrappleCollisionScript grappleHook = theGrapple.transform.GetChild(0).GetComponent<GrappleCollisionScript>();
			
			if(Input.GetButtonDown("Fire1")) 
			{
				Debug.Log("Ascend");
				//theGrapple.transform.localScale += grappleAscend;
			}
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
		}
	}

	void FireGrapple()
	{
		Camera cam = Camera.main;

		// Spawn grapple on player
		theGrapple = (GameObject)Instantiate(grapplePrefab, cam.transform.position+grappleSpawnPosOffset, cam.transform.rotation);

		// Get grapple's end (to let player 'hold on' to)
		Transform grappleEnd = theGrapple.transform.GetChild(theGrapple.transform.childCount-1);	

		// 'Attach' player to grapple end
		this.gameObject.AddComponent<FixedJoint>().connectedBody = grappleEnd.GetComponent<Rigidbody>();

		// Throw grapple (adding force to hook i.e. front of grapple)
		theGrappleHook_rb = theGrapple.transform.GetChild(0).GetComponent<Rigidbody>();
		theGrappleHook_rb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);

		// Just to get grapple size
		grappleSize *= 0;
		foreach(Transform child in theGrapple.transform)
		{
			grappleSize += child.GetComponent<Renderer>().bounds.size;
		}
	}
	
	void ReleaseGrapple()
	{
		if(this.GetComponent<FixedJoint>() != null)
		{
			theGrapple = null;
			Destroy(this.GetComponent<FixedJoint>()); // remove joint from player with grapple
			this.gameObject.GetComponent<Rigidbody>().isKinematic = true;	 
		}
	}
}