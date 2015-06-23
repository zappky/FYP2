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
	public Rect dialogBox;
	public Rect personBox;
	public List<string> outputList = new List<string>();
	public string dialogOutput = "";
	public string personName = "";
	public float delayTime = 5.0f;
	public float delayTimer = 0.0f;
	private bool delayFlag = false;

	// Use this for initialization
	void Start () {

		dialogBox = new Rect (0.0f, Screen.height * 0.70f, Screen.width, Screen.height * 0.30f);

		float personBoxHeight = dialogBox.height * 0.20f;
		float personBoxWidth = dialogBox.width * 0.20f;

		personBox = new Rect (dialogBox.xMin,dialogBox.yMin - personBoxHeight,personBoxWidth,personBoxHeight);

		outputList.Add("test sentnece 1");
		outputList.Add("test sentnece 2");
		outputList.Add("test sentnece 3");
		outputList.Add("test sentnece 4");

		
		dialogOutput = outputList[0];
		personName = "test name";
	}
	
	// Update is called once per frame
	void Update (){

//		Event currentevent = Event.current;
//
//		//check if mouse hover over the box
//		if (dialogBox.Contains (currentevent.mousePosition)) 
//		{
//			print ("dialog box hit");
//			++CurrentTextLine;
//			if(CurrentTextLine > outputList.Count)
//			{
//				CurrentTextLine = 0;
//			}
//			dialogOutput = outputList[CurrentTextLine];
//		}
	}

	void OnGUI()
	{
		if(display == true)
		{
			GUI.Box(dialogBox,dialogOutput);
			GUI.Box(personBox,personName);
		}


		//later shift this section to update//
		if (autoText == true) 
		{
			delayTimer += Time.deltaTime;
			if (delayTimer > delayTime) 
			{
				delayTimer = 0.0f;
				delayFlag = true;
			}
			if (delayFlag == true) 
			{
				++CurrentTextLine;
				if(CurrentTextLine >= outputList.Count)
				{
					CurrentTextLine = 0;
				}
				dialogOutput = outputList[CurrentTextLine];
				delayFlag = false;
			}
		}


		Event currentevent = Event.current;	
		//check if mouse hover over the box
		if (dialogBox.Contains (currentevent.mousePosition) ) //if mouse pointer is within dialog box
		{
			if(currentevent.button == 0 && currentevent.type == EventType.mouseUp )//left click and button up
			{
				//print ("dialog box hit");
				++CurrentTextLine;
				if(CurrentTextLine >= outputList.Count)
				{
					CurrentTextLine = 0;
				}
				dialogOutput = outputList[CurrentTextLine];
			}

		}


		//section ends	//
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
