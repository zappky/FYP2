using UnityEngine;
using System.Collections;

public class HideCursorScript : MonoBehaviour {
	
	public bool display = false;
	public bool manualOverride = false;

	// Use this for initialization
	void Start () {
		Cursor.visible = display;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown ("CursorToggle") ) 
		{
			if(manualOverride == true)
			{
				ToggleVisiblity();
			}
			manualOverride = true;

		}
		if (Input.GetKeyDown ("right shift") ) 
		{
			manualOverride = false;
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
		if(manualOverride == false)
		{
			display = mode;
			Cursor.visible = display;
		}
	}
}
