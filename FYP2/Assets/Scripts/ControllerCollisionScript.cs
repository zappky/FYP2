using UnityEngine;
using System.Collections;

public class ControllerCollisionScript : MonoBehaviour {

	//private FPSInputController FPSControllerScript;
	public float pushPower = 2.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnControllerColliderHit(ControllerColliderHit col)
	{
		//check win
		if (col.transform.tag == "WinObjective")
		{
			//print ("hited");
			Application.LoadLevel("winscreen");
		}

		//collision to other objects in the world
		if(col.rigidbody == null || col.rigidbody.isKinematic)
			return;

		//if (col.gameObject.name == "Ball") //hardcode
		//{
		//}

		// to not push obj below player
		if (col.moveDirection.y < -0.3)
			return;

		// calc push dir from move dir (push obj to sides only)
		Vector3 pushDir = new Vector3(col.moveDirection.x, 0, col.moveDirection.z);
		
		// get js
		//FPSControllerScript = this.GetComponent<FPSInputController>();
		// apply push to obj * player's force (vel)
		col.rigidbody.velocity = pushDir * pushPower;
	}
}