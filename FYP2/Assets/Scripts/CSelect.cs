using UnityEngine;
using System.Collections;

public class CSelect : MonoBehaviour {

	public RaycastHit hit;
	Ray ray;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width *0.5f,Screen.height *0.5f,0.0f)); // update the ray positon
		if (Physics.Raycast (ray, out hit, 10)) //ray cast testing
		{
			Debug.Log(hit.collider.gameObject.name);
			if(hit.collider.gameObject.name.Equals("TestAlarm") == true)//will be changed in future
			{

				hit.collider.gameObject.GetComponent<CTimer>().OnLookInteract();
			}
		}
	}
}
