using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))] 
public class Parachute : MonoBehaviour {
	//private CharacterMotor characScript;
	CharacterController cc;
	//float characterH;

	// Use this for initialization
	void Start () 
	{
		cc = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!cc.isGrounded && Input.GetKeyDown ("p")) 
		{
			print ("pressed");
		}
	}
}
