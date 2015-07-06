using UnityEngine;
using System.Collections;

public class HideCursorScript : MonoBehaviour {
	
	public bool display = false;

	// Use this for initialization
	void Start () {
		Cursor.visible = display;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("CursorToggle")) 
		{
			ToggleVisiblity();
		}
		//Cursor.visible = display;
	}

	public void ToggleVisiblity()
	{
		display = ! display;
		Cursor.visible = display;
	}
	
	public void SetVisiblity(bool mode)
	{
		display = mode;
		Cursor.visible = display;
	}
}
