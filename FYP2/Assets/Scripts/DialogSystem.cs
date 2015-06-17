using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script is suppose to make 2d dialog boxes of conversation between player and npc.
//desired result would be similar to visual novel style
public class DialogSystem : MonoBehaviour {

	public bool display = false;
	public float displayTransparency = 1.0f;
	public bool autoText = false;
	public float autoTextSpeed = 1.0f;
	public int CurrentTextLine = 0;
	public int CurrentParagraph = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if(display == true)
		{

		}
	}
	void LoadTextData()
	{
		//load from file manager using xml file
	}
	void ToggleAutoText()
	{
		autoText = !autoText;
	}
}
