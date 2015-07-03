using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (CharacterController))] 
public class Parachute : MonoBehaviour {
	//private CharacterMotor characScript;
	CharacterController cc;
	//float characterH;
	float DAspeed = 0.0f;	//Downward air speed
	float FAspeed = 0.0f;   //forward air speed
	float SAspeed = 0.0f;   //side air speed
	float airspeed = 0.005f;

	// Use this for initialization
	void Start () 
	{
		cc = GetComponent<CharacterController>();
		//characScript = GetComponent<CharacterMotor>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		FAspeed = Input.GetAxis ("Vertical") * airspeed;   
		SAspeed = Input.GetAxis ("Horizontal") * airspeed; 

		DAspeed = Physics.gravity.y * Time.deltaTime;

		if (!cc.isGrounded && Input.GetButton("parachute")) 
		{
			//print ("pressed");
			DAspeed = airspeed;
			Vector3 onair = new Vector3(SAspeed, DAspeed, FAspeed);
			onair = transform.rotation * onair;

			cc.Move(onair * Time.deltaTime);

			//characScript.movement.velocity = 0;
		}

	}
}
