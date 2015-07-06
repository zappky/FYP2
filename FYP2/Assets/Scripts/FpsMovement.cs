using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
[RequireComponent (typeof (FallResponse))]
public class FpsMovement : MonoBehaviour 
{
	public bool isRunning = false;
	public bool isGrappling = false;				 
	public bool useParachute = false;
	public float moveSpeed = 7.0f; //movement speed
	public float jumpSpeed = 5.0f;
	public float runSpeed = 1.2f;
	public float paraSpeed = 1.0f; //Parachute speed
	public float airSpeed = 3.0f;  //on air speed
	public float swingSpeed = 0.1f;	//grapple swing speed
	float forwardSpeed = 0.0f;
	float sideSpeed = 0.0f;

	//mouselook stuff
	public float mouseSensitivity = 5.0f; //balance mouse scrolling
	public float viewRange = 60.0f;
	float vertRotation = 0.0f;

	float vertVelo = 0.0f;

	CharacterController cc;

	//public AudioSource runSound;
	public AudioClip runSound;
	public float SFX_DELAY = 2.0f;
	float runSFXdelay = 0.0f;


	public bool debugFlyMode = false;		// rmb to turn off after use

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
		//Camera.main.transform.Rotate (0, rotateLR, 0);
		//camera rotate up down
		vertRotation -= Input.GetAxis ("Mouse Y") * mouseSensitivity;
		vertRotation = Mathf.Clamp (vertRotation, -viewRange, viewRange);
		Camera.main.transform.localRotation = Quaternion.Euler (vertRotation, 0, 0);

		if(Input.GetButton("Run"))
		{
			if(cc.isGrounded)
			{
				forwardSpeed = Input.GetAxis ("Vertical") * moveSpeed * runSpeed;	
				sideSpeed = Input.GetAxis ("Horizontal") * moveSpeed * runSpeed;

				if(forwardSpeed != 0 || sideSpeed != 0)
				{
					isRunning = true;

					runSFXdelay -= Time.deltaTime;
					if(runSFXdelay <= 0)	 
					{
						runSFXdelay = SFX_DELAY;
						AudioSource.PlayClipAtPoint(runSound, transform.position);
					}
				}
			}
		}
		else
		{
			//movement
			isRunning = false;
			forwardSpeed = Input.GetAxis ("Vertical") * moveSpeed;	
			sideSpeed = Input.GetAxis ("Horizontal") * moveSpeed;
		}

		if(isRunning && !cc.isGrounded)	// reduce spd when in air after a run jump
		{
			forwardSpeed = Input.GetAxis ("Vertical") * moveSpeed * runSpeed*0.5f;	
			sideSpeed = Input.GetAxis ("Horizontal") * moveSpeed * runSpeed*0.5f;
		}

		isGrappling = updateGrappleCheck();

		if(!isGrappling)	//temp grapple test - Hazim
		{
			//jump
			if(!debugFlyMode)
			{
				if(!cc.isGrounded)
					vertVelo += Physics.gravity.y * 2 * Time.deltaTime;
			}
			else
			{
				vertVelo = Input.GetAxis("Fly") * moveSpeed;	//dunno how do real fly mode
			}

			if(cc.isGrounded && Input.GetButton("Jump"))
			{
				vertVelo = jumpSpeed;
			}
			else if(!cc.isGrounded && Input.GetButtonDown("parachute"))	//button v
			{
				useParachute = !useParachute;
			}

			//activate parachute
			if (useParachute) 
			{
				vertVelo = -paraSpeed;
				forwardSpeed = Input.GetAxis ("Vertical") * airSpeed;
				sideSpeed = Input.GetAxis ("Horizontal") * airSpeed;
			}

			//set back parachute to false
			if(cc.isGrounded && useParachute)
				useParachute = false;

			//lose from height
//			if(vertVelo < -25.0f)
//			{
//				if(cc.isGrounded)
//					Application.LoadLevel("losescreen");
//			}

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
			//Debug.Log(this.gameObject.GetComponent<Rigidbody>().velocity);
		}
	}
	
	bool updateGrappleCheck()
	{
		if(!this.gameObject.GetComponent<Rigidbody>().isKinematic)
			return true;
		else
			return false;
	}
}
