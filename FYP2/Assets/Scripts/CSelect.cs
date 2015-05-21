using UnityEngine;
using System.Collections;

// this script is to able player to "touch" item in the world
 
public class CSelect : MonoBehaviour {

	public RaycastHit hit;
	public float raylength = 10.0f;
	public bool display = true;
	public bool onguihelper = true;
	public string interacthelpertext = "'e' to interact";
	Ray ray;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width *0.5f,Screen.height *0.5f,0.0f)); // update the ray positon
		
		if (Physics.Raycast (ray, out hit, raylength)) 
		{ //ray cast testing
			onguihelper = true;

			//Debug.Log (hit.collider.gameObject.name);
			if (Input.GetButtonDown ("Interact")) 
			{
				if (hit.collider.gameObject.tag == "Alarm") 
				{
					hit.collider.gameObject.GetComponent<CTimer> ().OnLookInteract ();
				}
			}
		} else 
		{
			onguihelper = false;
		}
	}
	void ToggleDisplay()
	{
		display = !display;
	}
	void OnGUI()
	{
		if (display == true) 
		{
			if(onguihelper == true)
			{
				//Debug.Log (interacthelpertext.Length);
				GUI.Box(new Rect(Screen.width*0.5f,Screen.height*0.95f,interacthelpertext.Length*6.0f,20.0f), interacthelpertext);
			}
		}
	}
}