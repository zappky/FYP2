using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrappleScript : MonoBehaviour {

	public bool isGrappling = false;
	public float ascSpeed = 100f;			// player's ascend/descend spd 
	public float toleranceLength = 2f;		// tolerance length for when player is moving on the rope
	
	public GameObject grapplePrefab;
	public float throwForce = 100f;			// throwing force of grapple	

	GameObject theGrapple;
	Transform grappleHook; 
	Transform grappleEnd; 
	Transform grappleRope;
	Transform rightHand;

	bool reeling;
	int currentRopeIndex;					// stores index of current rope pt player is on
	Transform targetRopePt;					// targeted pt to move to on rope, when grappling (for asc movement)
	List<Transform> targetRopePtList = new List<Transform>();	// list of pts on rope for asc movement on rope

	void Start() {
		rightHand = Camera.main.transform.GetChild(0);
	}


	void Update() {
		if(Input.GetButtonDown("Grapple")
		&& this.GetComponent<Inventory>().CheckContainsItem("Grapple")
		&& !DialogInterface.Instance.display) 	
		{
			if(theGrapple == null)				
				FireGrapple();
			else
				ReleaseGrapple();
		}

		if(theGrapple == null)
			return;

		// check if player use grapple successfully (hooks sth)
		GrappleCollisionScript hook = grappleHook.GetComponent<GrappleCollisionScript>();
		if(hook.GetGrappleHooked())		
		{
			if(!GetComponent<CharacterController>().isGrounded)
				GetComponent<Rigidbody>().isKinematic = false;			// enable player rb
			else
				GetComponent<Rigidbody>().isKinematic = true;			// disable player rb

			// Ascend
			if(Input.GetButton("Fire2") && GetComponent<FixedJoint>() != null)
			{	
				reeling = true;
				Destroy(GetComponent<FixedJoint>());
			}

			if(reeling)
			{
				GetComponent<Rigidbody>().isKinematic = true;
				Vector3 targetPos = targetRopePtList[currentRopeIndex].position;

				transform.position = Vector3.MoveTowards(transform.position, targetPos, 
				                                         ascSpeed * Time.deltaTime);
				
				// if player reached targeted pt on rope, move to next
				if( (targetPos - transform.position).sqrMagnitude < toleranceLength )
				{
					if(currentRopeIndex > 0)
						--currentRopeIndex;
					else
					{
						reeling = false;
						gameObject.transform.position = grappleHook.position;
						gameObject.AddComponent<HingeJoint>().connectedBody = grappleHook.GetComponent<Rigidbody>();
						Destroy(grappleRope.gameObject);
						Destroy(theGrapple.transform.FindChild("Render").gameObject);
						Destroy(grappleEnd.gameObject);

						if(GetComponent<CharacterController>().isGrounded)
							ReleaseGrapple();
					}
				}
			}
		}
	}

	void TraverseHierarchy(Transform root)
	{
		targetRopePtList.Add(root);

		// move to next transform(child) in hierarchy (if any)
		if(root.childCount > 0)
			TraverseHierarchy(root.GetChild(0));
	}

	public void FireGrapple()
	{
		isGrappling = true;

		// Spawn grapple on player
		theGrapple = (GameObject)Instantiate(grapplePrefab, rightHand.position, rightHand.rotation);

		// make sure grapple's prefab's hook, rope, end is ordered
		grappleHook = theGrapple.transform.GetChild(0);
		grappleRope = theGrapple.transform.GetChild(1);
		grappleEnd = theGrapple.transform.GetChild(2);
		grappleEnd.transform.position = rightHand.position;

		// add pts of the rope by traversing the rope hierarchy
		TraverseHierarchy(grappleRope);

		if(targetRopePtList.Count > 0)
		{
			reeling = false;
			currentRopeIndex = targetRopePtList.Count-1;
		}
		
		gameObject.AddComponent<FixedJoint>().connectedBody = grappleEnd.GetComponent<Rigidbody>();

		// Throw grapple (adding force to hook i.e. front of grapple)
		Rigidbody theGrappleHook_rb = grappleHook.GetComponent<Rigidbody>();
		theGrappleHook_rb.AddForce(Camera.main.transform.forward*throwForce-Camera.main.transform.up*throwForce*0.5f,
		                           ForceMode.Impulse);
	}
	
	public void ReleaseGrapple()
	{
		targetRopePt = null;
		targetRopePtList.Clear();
		if(GetComponent<FixedJoint>() != null)
			Destroy(GetComponent<FixedJoint>());
		if(GetComponent<HingeJoint>() != null)
			Destroy(GetComponent<HingeJoint>());
		Destroy(theGrapple);

		theGrapple = null;
		isGrappling = false;
		this.gameObject.GetComponent<Rigidbody>().isKinematic = true;	
	}
}