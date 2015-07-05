using UnityEngine;
using System.Collections;

public class Interact : MonoBehaviour {

	public float range 		= 10.0f;	// how far player can pickup obj
	public float DRAGSPD 	= 15.0f;		

	bool isInteracting;			// true when interacting w obj
	bool interactToggle;		 
	float dragSpeed;
	Transform dragMark;			// child of main cam (helps dragging of objs with cam)
	//Transform camPos;
	Rigidbody interact_rb;		// rb of interacted obj

	void Start () 
	{
		isInteracting = false;
		interactToggle = false;
		dragSpeed = DRAGSPD;
		dragMark = Camera.main.transform.FindChild("DragMark");
		//camPos = Camera.main.transform;
	}

	void Update() 
	{
		UpdateInput();

		// shoot ray from camera ctr
		Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width*0.5f, Screen.height*0.5f));
		RaycastHit hit;
		Debug.DrawRay(ray.origin, ray.direction*range, Color.blue);

		// Interact w obj
		if(interactToggle) 	
		{
			if(Physics.Raycast(ray, out hit, range))								// if ray hit sth within rng
			{
				interact_rb = hit.collider.attachedRigidbody;
				if(interact_rb != null && !interact_rb.isKinematic)					// if obj can be moved
				{
					Debug.Log("RB DETECTED");
					// check size (if small enuf let player move else dont)
					interact_rb.constraints = RigidbodyConstraints.FreezeRotation;	// so obj wont keep rot when dragged
					dragMark.position = interact_rb.position;
					isInteracting = true;
				}
			}
			//else
			//	isInteracting = false;		// if ray no longer touches obj interacted

			if(!isInteracting)	
			{
				interactToggle = false;
				return;	
			}
			
			// interact movement
			float dist = (dragMark.position - interact_rb.position).magnitude;
			Vector3 dir = (dragMark.position - interact_rb.position).normalized;
			
			dist = Mathf.Clamp (dist, 0, 3);
			dragSpeed = DRAGSPD*dist;
			interact_rb.velocity = dir*dragSpeed*Time.deltaTime;
		}
		// End interaction w obj
		else 	
		{
			if(!isInteracting)
				return;	

			// reset
			interact_rb.constraints = RigidbodyConstraints.None;		
			isInteracting = false;
		}
	}

	void UpdateInput()
	{
		if(Input.GetButtonDown("Grapple")) 	
			interactToggle = !interactToggle;
	}
}
