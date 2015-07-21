using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script is suppose to make 2d dialog boxes of conversation between player and npc.
//desired result would be similar to visual novel style
public class DialogInterface : MonoBehaviour {

	public static DialogInterface instance = null;
	public static bool initedBefore = false;
	public DialogDatabase dialogDatabase = null;
	
	public bool display = false;

	public Rect dialogBox;
	public Rect personBox;
	private Event currentevent = null;
	public DialogTree dialogTree = null;
	
	public my_DialogNode CurrentDialogNode = null;

	private float personBoxHeight;
	private float personBoxWidth;
	private float optionHeight;
	private float optionWidth;
	private int reservedOptionRectCount = 4;
	private List<Rect>optionRects = new List<Rect>();
	private GameObject Player = null;
	
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
		Initialize(true);// allow reinitalize of this class by default
	}

	public void Initialize(bool re_init)
	{
		if(initedBefore == false || re_init == true)
		{
			Player = GameObject.FindGameObjectWithTag("Player");
			dialogDatabase = DialogDatabase.Instance;
			dialogTree = dialogDatabase.GetDialogTree(Application.loadedLevelName);

			for(int i = 0 ; i < reservedOptionRectCount; ++i)
			{
				//init some temp rect for later uses.
				optionRects.Add(new Rect(0,0,0,0));
			}

			dialogBox = new Rect (0.0f, Screen.height * 0.70f, Screen.width, Screen.height * 0.30f);

			personBoxHeight = dialogBox.height * 0.20f;
			personBoxWidth = dialogBox.width * 0.20f;

			personBox = new Rect (dialogBox.xMin,dialogBox.yMin - personBoxHeight,personBoxWidth,personBoxHeight);
			initedBefore = true;
		}
	}

	public void DestroyInstance()
	{
		instance = null;
	}
	
	public void OnApplicationQuit()
	{
		SaveDialogInterface();
		DestroyInstance();
	}

	// Update is called once per frame
	void Update () {

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
	void UpdateDisplayRect()
	{
		dialogBox = new Rect (0.0f, Screen.height * 0.70f, Screen.width, Screen.height * 0.30f);
		
		personBoxHeight = dialogBox.height * 0.20f;
		personBoxWidth = dialogBox.width * 0.20f;
		
		personBox = new Rect (dialogBox.xMin,dialogBox.yMin - personBoxHeight,personBoxWidth,personBoxHeight);
	}
	void OnGUI()
	{
		if(ScreenManager.Instance.CheckAspectChanged() == true)
		{
			UpdateDisplayRect();
		}
		if(CurrentDialogNode == null)
		{
			display = false;
			return;
		}
		if(display == true)
		{
			if(CurrentDialogNode.nodeId < 0)
			{
				Player.GetComponent<CastSlot>().display = true;
				//cursorScript.toggle = false;
				return;
			}

			Player.GetComponent<CastSlot>().display = false;
			//cursorScript.toggle = true;

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
						if(CurrentDialogNode == null)
						{
							display = false;
							Player.GetComponent<CastSlot>().display = true;
							//cursorScript.toggle = false;
							return;
						}
					}
				}
				optionHeight = dialogBox.height* 0.2f ;
				optionWidth = dialogBox.width ;
				optionRects[0] = new Rect( 0,dialogBox.yMax - optionHeight,optionWidth,optionHeight);
				GUI.Box(optionRects[0],CurrentDialogNode.options[0].text);
			}
		}
	}

	public void LoadDialogInterface()
	{
		FileManager.Instance.LoadDialogInterface();
	}
	public void SaveDialogInterface()
	{
		FileManager.Instance.SaveDialogInterface();
	}
}
