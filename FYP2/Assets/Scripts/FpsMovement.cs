using UnityEngine;
using System.Collections;

public class FpsMovement : MonoBehaviour 
{
	public float moveSpeed = 7.0f; //movement speed
	public float jumpSpeed = 5.0f;
	public float paraSpeed = 1.0f; //Parachute speed
	public float airSpeed = 3.0f;  //on air speed

	//mouselook stuff
	public float mouseSensitivity = 5.0f; //balance mouse scrolling
	public float viewRange = 60.0f;
	float vertRotation = 0.0f;

	float vertVelo = 0.0f;

	CharacterController cc;

	bool paracheck = false;

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

		//jump
		if(!cc.isGrounded)
			vertVelo += Physics.gravity.y * Time.deltaTime;

		if(cc.isGrounded && Input.GetButton("Jump"))
		{
			vertVelo = jumpSpeed;
		}
		else if(!cc.isGrounded && Input.GetButtonDown("parachute"))//button v
		{
			paracheck = true;
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

		Vector3 speed = new Vector3 (sideSpeed, vertVelo, forwardSpeed);

		//lose from height
		if(vertVelo < -25.0f)
			Application.LoadLevel("losescreen");

		speed = transform.rotation * speed;

		cc.Move (speed * Time.deltaTime);
	}
}
