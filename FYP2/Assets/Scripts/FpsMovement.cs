using UnityEngine;
using System.Collections;

public class FpsMovement : MonoBehaviour 
{
	public float moveSpeed = 7.0f; //movement speed
	public float jumpSpeed = 5.0f;
	public float paraSpeed = 1.0f; //Parachute speed
	public float airSpeed = 3.0f;  //on air speed
	public float swingSpeed = 0.1f;	//grapple swing speed

	//mouselook stuff
	public float mouseSensitivity = 5.0f; //balance mouse scrolling
	public float viewRange = 60.0f;
	float vertRotation = 0.0f;

	float vertVelo = 0.0f;

	CharacterController cc;

	bool paracheck = false;
	bool isGrappling = false;		//if true, player is using grapple //temp grapple test - Hazim

	// Use this for initialization
	void Start () 
	{
		cc = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//camera rotate left right
		float rotateLR = Input.GetAxis ("Mouse X") * mouseSensitivity;
		transform.Rotate (0, rotateLR, 0);

		//camera rotate up down
		vertRotation -= Input.GetAxis ("Mouse Y") * mouseSensitivity;
		vertRotation = Mathf.Clamp (vertRotation, -viewRange, viewRange);
		Camera.main.transform.localRotation = Quaternion.Euler (vertRotation, 0, 0);

		//movement
		float forwardSpeed = Input.GetAxis ("Vertical") * moveSpeed;
		float sideSpeed = Input.GetAxis ("Horizontal") * moveSpeed;

		updateGrappleCheck();

		if(!isGrappling)	//temp grapple test - Hazim
		{
			//jump
			if(!cc.isGrounded)
				vertVelo += Physics.gravity.y*2 * Time.deltaTime;

			if(cc.isGrounded && Input.GetButton("Jump"))
			{
				vertVelo = jumpSpeed;
			}
			else if(!cc.isGrounded && Input.GetButtonDown("parachute"))	//button v
			{
				paracheck = !paracheck;
			}

			//activate parachute
			if (paracheck) 
			{
				vertVelo = -paraSpeed;
				forwardSpeed = Input.GetAxis ("Vertical") * airSpeed;
				sideSpeed = Input.GetAxis ("Horizontal") * airSpeed;
			}

			//set back parachute to false
			if(cc.isGrounded && paracheck)
				paracheck = false;

			//lose from height
			if(vertVelo < -25.0f)
			{
				if(cc.isGrounded)
					Application.LoadLevel("losescreen");
			}

			Vector3 speed = new Vector3 (sideSpeed, vertVelo, forwardSpeed);
			speed = transform.rotation * speed;
			
			cc.Move (speed * Time.deltaTime);
		}		
		else
		{
			//temp grapple test - Hazim
//			Vector3 speed = new Vector3 (this.GetComponent<Rigidbody>().velocity.x+sideSpeed, 
//			                             this.GetComponent<Rigidbody>().velocity.y, 
//			                             this.GetComponent<Rigidbody>().velocity.z+forwardSpeed);

			Vector3 speed = new Vector3 (sideSpeed, this.GetComponent<Rigidbody>().velocity.y, forwardSpeed);
			speed = transform.rotation * speed;

			//if(!cc.isGrounded)
			this.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(sideSpeed*swingSpeed, 0, forwardSpeed*swingSpeed),
			                                                   ForceMode.Impulse);
			//else
			//cc.Move (speed * Time.deltaTime);
			Debug.Log(this.gameObject.GetComponent<Rigidbody>().velocity);
		}
	}

	//temp grapple test - Hazim		*switch to return bool if it works
	void updateGrappleCheck()
	{
		if(this.gameObject.GetComponent<Rigidbody>().isKinematic)
			isGrappling = false;
		else
			isGrappling = true;
	}

	//temp grapple test - Hazim *may nt be needed
	public void fireGrapple()
	{
		//isGrappling = true;
	}
	//temp grapple test - Hazim *may nt be needed
	public void releaseGrapple()
	{
		//isGrappling = false;
	}
}
