using UnityEngine;
using System.Collections;

public class GrappleScript : MonoBehaviour {
	
	//public GameObject Player;
	public GameObject grapplePrefab;
	public float throwForce = 100f;

	//private bool inAir = false;
	private GameObject theGrapple;
	private Rigidbody theGrappleHook_rb;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1")) 
		{
			FireGrapple();
		}
	}

	void FireGrapple()
	{
		Camera cam = Camera.main;

		// Spawn grapple on player
		Vector3 grappleSpawnPos = cam.transform.position;
		grappleSpawnPos.x += 1;
		grappleSpawnPos.z -= 1;
		theGrapple = (GameObject)Instantiate(grapplePrefab, grappleSpawnPos, cam.transform.rotation);

		// Get grapple's end (to let player 'hold on' to)
		Transform grappleEnd = theGrapple.transform.GetChild(theGrapple.transform.childCount-1);	

		// 'Attach' player to grapple end
		grappleEnd.gameObject.AddComponent<FixedJoint>().connectedBody = this.GetComponent<Rigidbody>();

		// Throw grapple (adding force to hook i.e. front of grapple)
		theGrappleHook_rb = theGrapple.transform.GetChild(0).GetComponent<Rigidbody>();
		theGrappleHook_rb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
	}
	
	Transform GetEndChild(Transform transform)
	{
		Transform endChild;
		
		if(transform.childCount > 0)	//if gameobj still has child
		{
			endChild = GetEndChild(transform.GetChild(0));	
		}
		else
			endChild = transform;		
		
		return endChild;				
	}
}