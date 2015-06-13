using UnityEngine;
using System.Collections;

public class HideCursorScript : MonoBehaviour {
	
	public bool toggle = false;

	// Use this for initialization
	void Start () {
		Cursor.visible = toggle;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("CursorToggle")) 
		{
			ToggleVisiblity();
		}
	}

	public void ToggleVisiblity()
	{
		toggle = ! toggle;
		Cursor.visible = toggle;
	}
	
	public void SetVisiblity(bool mode)
	{
		toggle = mode;
		Cursor.visible = toggle;
	}
}
