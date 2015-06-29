using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script is suppose to make 2d dialog boxes of conversation between player and npc.
//desired result would be similar to visual novel style
public class DialogInterface : MonoBehaviour {

	public DialogDatabase dialogDatabase = null;

	public bool display = false;
	public float displayTransparency = 1.0f;
	public bool autoText = false;
	public float autoTextSpeed = 1.0f;
	//public int CurrentTextLine = 0;

	public Rect dialogBox;
	//public string dialogOutput = "";
	public Rect personBox;
	//public string personName = "";

	//public float delayTime = 5.0f;
	//public float delayTimer = 0.0f;
	//private bool delayFlag = false;

	private Event currentevent = null;
	public DialogTree dialogTree = null;
	public int CurrentGameLevel = 1;
	//public int CurrentDialogNodeIndex = 0;
	public my_DialogNode CurrentDialogNode = null;

	private float personBoxHeight;
	private float personBoxWidth;
	private float optionHeight;
	private float optionWidth;
	private int reservedOptionRectCount = 4;
	private List<Rect>optionRects = new List<Rect>();

	// Use this for initialization
	void Start () {

		dialogDatabase = DialogDatabase.Instance;
		dialogTree = dialogDatabase.GetDialogTreeWithLevel(CurrentGameLevel);
		CurrentDialogNode = dialogTree.GetDialog(0);
		for(int i = 0 ; i < reservedOptionRectCount; ++i)
		{
			//init some temp rect for later uses.
			optionRects.Add(new Rect(0,0,0,0));
		}


		dialogBox = new Rect (0.0f, Screen.height * 0.70f, Screen.width, Screen.height * 0.30f);

		personBoxHeight = dialogBox.height * 0.20f;
		personBoxWidth = dialogBox.width * 0.20f;

		personBox = new Rect (dialogBox.xMin,dialogBox.yMin - personBoxHeight,personBoxWidth,personBoxHeight);
	}
	// Update is called once per frame
	void Update () {
	}

	void OnGUI()
	{

		if(display == true && CurrentDialogNode != null)
		{
			currentevent = Event.current;	

			GUI.Box(dialogBox,CurrentDialogNode.text);
			GUI.Box(personBox,CurrentDialogNode.actorName);
			
			if(CurrentDialogNode.options.Count >1)
			{
				//render the options
				for (int i = 0 ; i < CurrentDialogNode.options.Count; ++i)
				{
					if(CurrentDialogNode.options.Count > optionRects.Count)//pad out the difference,else will cause argument out of range
					{
						int difference = CurrentDialogNode.options.Count - optionRects.Count;
						for (int d = 0 ; d < difference ; ++d)
						{
							optionRects.Add(new Rect(0,0,0,0));
						}
					}
					
					optionHeight = dialogBox.height* 0.2f ;
					optionWidth = dialogBox.width /CurrentDialogNode.options.Count;
					
					optionRects[i] = new Rect( (i) * optionWidth,dialogBox.yMax - optionHeight,optionWidth,optionHeight);
					GUI.Box(optionRects[i],CurrentDialogNode.options[i].text);
					
					if (optionRects[i].Contains (currentevent.mousePosition) ) //if mouse pointer is within option box
					{
						if(currentevent.button == 0 && currentevent.type == EventType.mouseUp )//left click and button up
						{
							CurrentDialogNode = CurrentDialogNode.options[i].nextDialog;
							//CurrentDialogNodeIndex = CurrentDialogNode.nodeId;
							break;
						}
					}
				}		
			}else
			{
				if(CurrentDialogNode.options.Count == 0)
				{
					//end of conversation

					if (dialogBox.Contains (currentevent.mousePosition) ) //if mouse pointer is within option box
					{
						if(currentevent.button == 0 && currentevent.type == EventType.mouseUp )//left click and button up
						{
							CurrentDialogNode = null;
							display = false;
						}
					}
				}else
				{
					//there is one option to link to another dialog node

					if (dialogBox.Contains (currentevent.mousePosition) ) //if mouse pointer is within option box
					{
						if(currentevent.button == 0 && currentevent.type == EventType.mouseUp )//left click and button up
						{
							CurrentDialogNode = CurrentDialogNode.options[0].nextDialog;
						}
					}
					//optionHeight = dialogBox.height* 0.2f ;
					optionWidth = dialogBox.width ;
						
					optionRects[0] = new Rect( 0,dialogBox.yMax - optionHeight,optionWidth,optionHeight);
					GUI.Box(optionRects[0],CurrentDialogNode.options[0].text);

				}
			}
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
