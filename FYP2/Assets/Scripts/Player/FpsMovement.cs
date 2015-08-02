using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
[RequireComponent (typeof (FallResponse))]
public class FpsMovement : MonoBehaviour 
{
	public bool isRunning = false;
	public bool isGrappling = false;	// true whn grappling && inAir			 
	public bool useParachute = false;
	public float moveSpeed = 7.0f; 		//movement speed
	public float jumpSpeed = 5.0f;
	public float runSpeed = 1.2f;
	public float paraSpeed = 1.0f; 		//Parachute speed
	public float airSpeed = 3.0f;  		//on air speed
	public float swingSpeed = 0.1f;		//grapple swing speed
	public float pushPower = 2.0f;		// force applied whn pushing object
	float forwardSpeed = 0.0f;
	float sideSpeed = 0.0f;

	//mouselook stuff
	public float mouseSensitivity = 3.0f; //balance mouse scrolling
	public float viewRange = 60.0f;
	float vertRotation = 0.0f;

	float vertVelo = 0.0f;

	CapsuleCollider playerCol;	// active when player using grapple
	CharacterController cc;

	//public AudioSource runSound;
	public AudioClip runSound;
	public float SFX_DELAY = 2.0f;
	float runSFXdelay = 0.0f;

	Inventory inventory;
	DialogInterface dialoginterface;

	public bool debugFlyMode = false;		// rmb to turn off after use

	// Use this for initialization
	void Start () 
	{
		cc = GetComponent<CharacterController>();
		playerCol = GetComponentInChildren<CapsuleCollider>();
		inventory = GetComponent<Inventory>();
		dialoginterface = DialogInterface.Instance;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.timeScale == 0.0)
			return;

		//camera rotate left right
		float rotateLR = Input.GetAxis ("Mouse X") * mouseSensitivity;

		if(inventory.display == true || dialoginterface.display == true)
			isRunning = false;

		if(inventory.display == false && dialoginterface.display == false)
		{
			transform.Rotate (0, rotateLR, 0);
			//Camera.main.transform.Rotate (0, rotateLR, 0);
			//camera rotate up down
			vertRotation -= Input.GetAxis ("Mouse Y") * mouseSensitivity;
			vertRotation = Mathf.Clamp (vertRotation, -viewRange, viewRange);
			Camera.main.transform.localRotation = Quaternion.Euler (vertRotation, 0, 0);

			if(Input.GetButton("Run") && inventory.CheckContainsItem("Running Shoes"))
			{
				// prevent player from running in air
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
		}

		if(isRunning && !cc.isGrounded)	// reduce spd when in air after a run jump
		{
			forwardSpeed = Input.GetAxis ("Vertical") * moveSpeed * runSpeed*0.5f;	
			sideSpeed = Input.GetAxis ("Horizontal") * moveSpeed * runSpeed*0.5f;
		}

		isGrappling = updateGrappleCheck();
		playerCol.isTrigger = !isGrappling;

		if(!isGrappling)	 
		{
			// in air
			if(!cc.isGrounded)
			{
				if(!debugFlyMode)
					vertVelo += Physics.gravity.y * 2 * Time.deltaTime;
				else
					vertVelo = Input.GetAxis("Fly") * moveSpeed;	

				if(Input.GetButtonDown("parachute"))
				{
					if(!useParachute)
					{
						if(inventory.CheckContainsItem("Parachute"))
							ToggleParachute(true);
					}
					else 										 
						inventory.UseItem("Parachute");		// player fin used parachute alr, so remove it
				}

				// parachute in use
				if (useParachute) 
				{
					vertVelo = -paraSpeed;
					forwardSpeed = Input.GetAxis ("Vertical") * airSpeed;
					sideSpeed = Input.GetAxis ("Horizontal") * airSpeed;
				}
			}
			else
			{
				if (useParachute) 
					inventory.UseItem("Parachute");			// player fin used parachute alr, so remove it

				if(Input.GetButton("Jump") && !(inventory.display || dialoginterface.display))
					vertVelo = jumpSpeed;
			}

			Vector3 speed = new Vector3 (sideSpeed, vertVelo, forwardSpeed);

			if(inventory.display || dialoginterface.display)
				speed = new Vector3 (0, vertVelo, 0);

			speed = transform.rotation * speed;

			cc.Move (speed * Time.deltaTime);
		}		
		else
		{
			float vertSpeed = 0;
			if(!PlayerRBIsGrounded())
				vertSpeed = Physics.gravity.y * 2 * Time.deltaTime;

			Vector3 speed = new Vector3 (sideSpeed*5, vertSpeed, forwardSpeed*5);
			speed = Camera.main.transform.rotation * speed;
			this.gameObject.GetComponent<Rigidbody>().AddForce(speed, ForceMode.Force);
		}
	}

	public void ToggleParachute(bool usePara)
	{
		useParachute = usePara;
		// deploy parachute 'model'
		print (useParachute);
		transform.FindChild("Parachute").gameObject.SetActive(useParachute);
	}
	
	bool updateGrappleCheck()
	{
		if(!this.gameObject.GetComponent<Rigidbody>().isKinematic)
			return true;
		else
			return false;
	}


	// to check if player's rb is grounded (used when grappling only)
	public bool PlayerRBIsGrounded()
	{
		return Physics.Raycast(transform.position, -Vector3.up, 
		                       GetComponentInChildren<CapsuleCollider>().bounds.extents.y+0.1f);
	}


	void OnControllerColliderHit(ControllerColliderHit col)
	{
		//check win
		if (col.transform.tag == "WinObjective")
		{
			//print ("hited");
			// check level, if 1, go to 2 else win
			if(LevelManager.Instance.CurrentLevelName == "Level1")
			{
				LevelManager.Instance.loadFromContinue = false;
				Application.LoadLevel("Level2");
			}
			else
				Application.LoadLevel("winscreen");
		}

		if(col.transform.tag == "Enemy")
			col.transform.GetComponent<EnemyAlert>().AlertIncr(transform.position);

		if(col.transform.tag == "Puzzle"						// if obj is puzzle element
		|| col.rigidbody == null 
		|| col.rigidbody.isKinematic 
		|| col.rigidbody.mass > GetComponent<Rigidbody>().mass)	// if obj too heavy		
			return;
		
		// to not push obj below player
		if (col.moveDirection.y < -0.3)
			return;
		
		// calc push dir from move dir (push obj to sides only)
		Vector3 pushDir = new Vector3(col.moveDirection.x, 0, col.moveDirection.z);
		
		col.rigidbody.velocity = pushDir * pushPower;

		// play sfx
		col.gameObject.GetComponent<ObjectCollisionResponse>().playCollisionSFX();

		// pass noise info to enemy mgr
		EnemyManager.Instance.AddAlertGlobally(transform.position);
	}
}
