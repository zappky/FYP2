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

	//public int CurrentDialogNodeIndex = 0;
	public my_DialogNode CurrentDialogNode = null;

	private float personBoxHeight;
	private float personBoxWidth;
	private float optionHeight;
	private float optionWidth;
	private int reservedOptionRectCount = 4;
	private List<Rect>optionRects = new List<Rect>();
	private LevelManager levelmanager = null;

	public static DialogInterface instance = null;
		
	public static DialogInterface Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("Dialog Interface").AddComponent<DialogInterface>();
				//DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}
	public void Initialize()
	{
		dialogDatabase = DialogDatabase.Instance;
		levelmanager = LevelManager.Instance;
		dialogTree = dialogDatabase.GetDialogTree("main-scene");
		CurrentDialogNode = dialogTree.GetDialogBookmarkedWithIndex(0);
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

	public void DestroyInstance()
	{
		instance = null;
	}
	
	public void OnApplicationQuit()
	{
		DestroyInstance();
	}
//	// Use this for initialization
//	void Start () {
//
//		dialogDatabase = DialogDatabase.Instance;
//		levelmanager = GameObject.FindObjectOfType<LevelManager>();
//		dialogTree = dialogDatabase.GetDialogTree("main-scene");
//		CurrentDialogNode = dialogTree.GetDialog(0);
//		for(int i = 0 ; i < reservedOptionRectCount; ++i)
//		{
//			//init some temp rect for later uses.
//			optionRects.Add(new Rect(0,0,0,0));
//		}
//
//
//		dialogBox = new Rect (0.0f, Screen.height * 0.70f, Screen.width, Screen.height * 0.30f);
//
//		personBoxHeight = dialogBox.height * 0.20f;
//		personBoxWidth = dialogBox.width * 0.20f;
//
//		personBox = new Rect (dialogBox.xMin,dialogBox.yMin - personBoxHeight,personBoxWidth,personBoxHeight);
//	}
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("b"))
		{
			StartNewDialogSessionUsingBookmark("main-scene","1");
		}
		if(Input.GetKeyDown("v"))
		{
			StartNewDialogSessionUsingBookmark("main-scene","0");
		}
	}
	public bool StartNewDialogSessionUsingBookmark(string gameLevelName,string bookmark_node_name)
	{
		CurrentDialogNode = dialogDatabase.GetBookmarkDialogNode(gameLevelName,bookmark_node_name);
		if(CurrentDialogNode != null)
		{
			display = true;
			return true;
		}
		return false;

	}
	public bool StartNewDialogSessionUsingBookmark(int dialogTreeId,string bookmark_node_name)
	{
		CurrentDialogNode = dialogDatabase.GetBookmarkDialogNode(dialogTreeId,bookmark_node_name);
		if(CurrentDialogNode != null)
		{
			display = true;
			return true;
		}
		return false;
	}
	public bool StartNewDialogSessionUsingBookmark(string gameLevelName,int bookmark_node_id)
	{
		CurrentDialogNode = dialogDatabase.GetBookmarkDialogNode(gameLevelName,bookmark_node_id);
		if(CurrentDialogNode != null)
		{
			display = true;
			return true;
		}
		return false;
	}
	public bool StartNewDialogSessionUsingBookmark(int dialogTreeId,int bookmark_node_id)
	{
		CurrentDialogNode = dialogDatabase.GetBookmarkDialogNode(dialogTreeId,bookmark_node_id);
		if(CurrentDialogNode != null)
		{
			display = true;
			return true;
		}
		return false;
	}
	public bool StartNewDialogSession(string gameLevelName,int dialogNodeId)
	{
		CurrentDialogNode = dialogDatabase.GetDialogNode(gameLevelName,dialogNodeId);
		if(CurrentDialogNode != null)
		{
			display = true;
			return true;
		}
		return false;
	}
	public bool StartNewDialogSession(int dialogTreeId,int dialogNodeId)
	{
		CurrentDialogNode = dialogDatabase.GetDialogNode(dialogTreeId,dialogNodeId);
		if(CurrentDialogNode != null)
		{
			display = true;
			return true;
		}
		return false;
	}
	public bool StartNewDialogSession(my_DialogNode a_dialogNode)
	{
		CurrentDialogNode = a_dialogNode;
		if(CurrentDialogNode != null)
		{
			display = true;
			return true;
		}
		return false;
	}
	void OnGUI()
	{
		if(display == true && CurrentDialogNode != null)
		{
			if(CurrentDialogNode.nodeId < 0)
			{
				return;
			}

			currentevent = Event.current;	

			GUI.Box(dialogBox,CurrentDialogNode.text);
			GUI.Box(personBox,CurrentDialogNode.actorName);
			
			if(CurrentDialogNode.options.Count >1)//meaning there are other link than a single process link
			{
				//multi option section
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
				//single link section
				if (dialogBox.Contains (currentevent.mousePosition) ) //if mouse pointer is within option box
				{
					if(currentevent.button == 0 && currentevent.type == EventType.mouseUp )//left click and button up
					{
						CurrentDialogNode = CurrentDialogNode.options[0].nextDialog;
						if(CurrentDialogNode.options.Count == 0)//stop the display once the node has no more linkage
						{
							CurrentDialogNode = null;
							display = false;
							return;
						}
					}
				}

				optionWidth = dialogBox.width ;
				optionRects[0] = new Rect( 0,dialogBox.yMax - optionHeight,optionWidth,optionHeight);
				GUI.Box(optionRects[0],CurrentDialogNode.options[0].text);
			}
		}
	}

	void ToggleAutoText()
	{
		autoText = !autoText;
	}
}
