using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class my_QuestLog
{
	public int id = -1;
	public string questname = "";
	public bool statues = false;//true means done

	public my_QuestLog()
	{

	}

	public my_QuestLog(my_QuestLog another)
	{
		this.id = another.id;
		this.questname = another.questname;
		this.statues = another.statues;
	}
	public my_QuestLog(int questId , string questname)
	{
		this.id = questId;
		this.questname = questname;
		this.statues = false;
	}
	public my_QuestLog(string questname)
	{
		this.id = -1;
		this.questname = questname;
		this.statues = false;
	}
	
	public string StringStatues()
	{
		if (statues == true) 
		{
			//return "Quest Done";
			return "";
		} else 
		{
			//return "Not Done";
			return "";
		}
	}
	public void ClearQuest()
	{
		this.statues = true;
	}
	public void ResetStatues()
	{
		this.statues = false;
	}
}

public class QuestManager : MonoBehaviour {

	public static QuestManager instance = null;
	public static bool initedBefore = false;

	public bool display = true;

	public QuestLogDatabase questlogdatabase = null;
	public my_QuestLogList questLogListDatabase = null;
	public List<my_QuestLog> questLogs = new List<my_QuestLog>();

	public Rect questDisplayRect;
	public Rect notificationDisplayRect;
	public string notificationText = "";
	public List<Rect>questLogRects = new List<Rect>();
	public const int maxQuestLog = 1;
	private float questLogHeight = -1.0f;
	public int currentGameLevel = -1;
	public int currentQuestIndex = -1;
	public float notificationTimer  = 0.0f;
	public float notificaitonTimerLimit = 5.0f;//5 sec
	public GUIStyle notificationlabelStyle;
	public List<GUIStyle> questlogStyles = new List<GUIStyle>();
	public static QuestManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("Quest Manager").AddComponent<QuestManager>();
				DontDestroyOnLoad(instance);
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
			this.questlogdatabase = QuestLogDatabase.Instance;

			//temporary solution
			List<string> gamelevelnamelist = ApplicationLevelBoard.Instance.gameLevelNameList;
			for(int i = 0 ; i < gamelevelnamelist.Count; ++i)
			{
				if(gamelevelnamelist[i] == Application.loadedLevelName)
				{
					currentGameLevel = i;
					break;
				}
			}
			Debug.Log("current game level in quest manager:" + currentGameLevel);
			this.questLogListDatabase = questlogdatabase[currentGameLevel];
			
			questDisplayRect = new Rect (0.0f,Screen.height * 0.001f,Screen.width * 0.25f,Screen.height * 0.1f);
			questLogHeight = questDisplayRect.height / (maxQuestLog+1);
			for (int i = 0; i<maxQuestLog; ++i)//reserve some rect 
			{
				//questLogs.Add(new my_QuestLog());
				questLogRects.Add (new Rect (questDisplayRect.xMin,questDisplayRect.yMin + (i+1) *questLogHeight,questDisplayRect.width,questLogHeight));

				questlogStyles.Add(new GUIStyle());
				questlogStyles[i].normal.textColor = Color.white;
				questlogStyles[i].normal.background = Texture2D.blackTexture;
				questlogStyles[i].alignment = TextAnchor.MiddleCenter;

			}
			notificationDisplayRect = new Rect (questDisplayRect.xMin,questDisplayRect.yMin + (maxQuestLog+1) *questLogHeight,questDisplayRect.width,questLogHeight);
			currentQuestIndex = 0;
			questLogs.Clear();
			FetchCurrentQuest();

			notificationlabelStyle = new GUIStyle();
			notificationlabelStyle.normal.textColor = Color.white;
			notificationlabelStyle.normal.background = (Texture2D)Resources.Load("Fonts/green");
			notificationlabelStyle.alignment = TextAnchor.MiddleCenter;

			initedBefore = true;
		}
	}

	public void ClearQuest(my_QuestLog theQuest)
	{
		notificationText = "CLEARED: " + theQuest.questname;
		notificationTimer = 0;
		theQuest.ClearQuest();
		questLogs.Remove(theQuest);
		FetchNewQuest();
	}
	public void ClearQuest(int questId)
	{
		my_QuestLog theQuest = GetQuestLog(questId);
		if(theQuest != null)
		{
			notificationText = theQuest.questname + "IS CLEARED";
			notificationTimer = 0;
			theQuest.ClearQuest();
			questLogs.Remove(theQuest);
		}else
		{
			Debug.Log("ERROR: ClearQuest is using null theQuest reference");
		}
		FetchNewQuest();
	}
	public void ClearQuest(string questName)
	{
		my_QuestLog theQuest = GetQuestLog(questName);
		if(theQuest != null)
		{
			notificationText = theQuest.questname + "IS CLEARED";
			notificationTimer = 0;
			theQuest.ClearQuest();
			questLogs.Remove(theQuest);
		}else
		{
			Debug.Log("ERROR: ClearQuest is using null theQuest reference");
		}
		FetchNewQuest();
	}
	public void RemoveClearedQuestLogs()
	{
		RemoveQuestLogs(true);
	}
	public void RemoveQuestLogs(bool questStatues)
	{
		for(int i = 0 ; i < questLogs.Count; ++i)
		{
			if(questLogs[i].statues == questStatues)
			{
				questLogs.Remove(questLogs[i]);
			}
		}
	}
	public my_QuestLog GetCurrentQuest()
	{
		return questLogListDatabase[currentQuestIndex];
	}
	public void FetchCurrentQuest()
	{
		AddQuestLog( questLogListDatabase[currentQuestIndex]);
	}
	public void FetchNewQuest()
	{
		///this.questLogListDatabase = questlogdatabase[currentGameLevel];//should be reloaded whenever new game scene is loaded
		for(int i = currentQuestIndex+1 ; i < questLogListDatabase.questlogs.Count ; ++i)
		{
			if(AddQuestLog(questLogListDatabase[i] ) == true)//it is ok to use reference from database in this case
			{
				currentQuestIndex = i;
				break;
			}
		}



	}
	public void RemoveQuestLog(int questId)
	{
		if(questId <0)
		{
			Debug.Log("ERROR: RemoveQuestLog with questId is negative value");
			return;
		}

		if(questId <questLogs.Count)
		{
			//early prediction
			if(questLogs[questId].id == questId)
			{
				questLogs.Remove(questLogs[questId]);
				return;
				//questLogs[questId] = new my_QuestLog();
			}
			//if still cannot be found, brute force search
			for(int i = 0 ; i< questId; ++i)
			{
				if(questLogs[i].id == questId)
				{
					questLogs.Remove(questLogs[i]);
					return;
					//questLogs[i] = new my_QuestLog();
				}
			}
			for(int i = questId+1 ; i< questLogs.Count; ++i)
			{
				if(questLogs[i].id == questId)
				{
					questLogs.Remove(questLogs[i]);
					return;
					//questLogs[i] = new my_QuestLog();
				}
			}
		}else
		{
			Debug.Log("WARNING: RemoveQuestLog with quest id is over list count,brute force search");
			for(int i = 0 ; i< questLogs.Count; ++i)
			{
				if(questLogs[i].id == questId)
				{
					questLogs.Remove(questLogs[i]);
					return;
					//questLogs[i] = new my_QuestLog();
				}
			}
		}
		


	}
	public void RemoveQuestLog(string questLogName)
	{
		for(int i = 0 ; i< questLogs.Count; ++i)
		{
			if(questLogs[i].questname == questLogName)
			{
				questLogs.Remove(questLogs[i]);
				return;
				//questLogs[i] = new my_QuestLog();
			}
		}
	}
	public my_QuestLog GetQuestLog(int questId)
	{
		if(questId < 0 )
		{
			Debug.Log("ERROR: GetQuestLog with questId is negative value");
			return null;
		}

		if(questId <questLogs.Count)
		{
			//early prediction
			if(questLogs[questId].id == questId)
			{
				return questLogs[questId];
			}
			//if still cannot be found, brute force search
			for(int i = 0 ; i< questId; ++i)
			{
				if(questLogs[i].id == questId)
				{
					return questLogs[i];
				}
			}
			for(int i = questId+1 ; i< questLogs.Count; ++i)
			{
				if(questLogs[i].id == questId)
				{
					return questLogs[i];
				}
			}
		}else
		{
			Debug.Log("WARNING: GetQuestLog with questId is over list count,brute force search");
			for(int i = 0 ; i< questLogs.Count; ++i)
			{
				if(questLogs[i].id == questId)
				{
					return questLogs[i];
				}
			}
		}

		return null;
	}
	public my_QuestLog GetQuestLog(string questLogName)
	{
		for(int i = 0 ; i< questLogs.Count; ++i)
		{
			if(questLogs[i].questname == questLogName)
			{
				return questLogs[i];
			}
		}
		return null;
	}
	public bool AddQuestLog(int questid,string newQuestName)
	{
		return AddQuestLog(new my_QuestLog(questid,newQuestName));
	}
	public bool AddQuestLog(string newQuestName)
	{
		return AddQuestLog(new my_QuestLog(newQuestName));
	}
	public bool AddQuestLog(my_QuestLog a_questlog)
	{

		if(questLogs.Count >= maxQuestLog)
		{
			print ("ERROR: max quest log reached, cannot add anymore log");
			return false;
		}else
		{
			if(DuplicateQuestLogCheck(a_questlog) == false)
			{
				questLogs.Add(a_questlog);
				return true;
			}else
			{
				return false;
			}
		}
	}
	public bool DuplicateQuestLogCheck(my_QuestLog a_questlog)
	{
		if(a_questlog.id < 0)
		{
			Debug.Log("ERROR: DuplicateQuestLogCheck with quest log id is negative value: " + a_questlog.id );
			return false;
		}
		if(a_questlog.id < questLogs.Count)
		{
			if(questLogs[a_questlog.id].id == a_questlog.id)
			{
				return true;
			}
			for(int i = 0 ; i < a_questlog.id; ++i)
			{
				if(questLogs[i].id == a_questlog.id)
				{
					return true;
				}
			}
			for(int i = a_questlog.id+1 ; i < questLogs.Count; ++i)
			{
				if(questLogs[i].id == a_questlog.id)
				{
					return true;
				}
			}
		}else
		{
			Debug.Log("WARNING: DuplicateQuestLogCheck with quest log id is over list value: " + a_questlog.id);
			for(int i = 0 ; i <questLogs.Count ; ++i)
			{
				if(questLogs[i].id == a_questlog.id)
				{
					return true;
				}
			}
		}

		return false;
	}


	public void OnApplicationQuit()
	{
		SaveQuestManager();
		DestroyInstance();
	}
	
	public void DestroyInstance()
	{
		instance = null;
	}
	void Update()
	{
		//if(Input.GetButtonDown("QuestInterface"))
		//{
		//	ToggleDisplay();
		//}
	}
	void UpdateDisplayRect()
	{
		//questDisplayRect = new Rect (0.0f,Screen.height * 0.001f,Screen.width * 0.25f,Screen.height * 0.1f);
		questDisplayRect.x = 0.0f;
		questDisplayRect.y = Screen.height * 0.001f;
		questDisplayRect.width = Screen.width * 0.25f;
		questDisplayRect.height = Screen.height * 0.1f;
		questLogHeight = questDisplayRect.height / (maxQuestLog+1);
		for (int i = 0; i<maxQuestLog; ++i)//reserve some rect 
		{
			questLogRects[i] = new Rect (questDisplayRect.xMin,questDisplayRect.yMin + (i+1) *questLogHeight,questDisplayRect.width,questLogHeight);
	
		}


		notificationDisplayRect.x = questDisplayRect.xMin;
		notificationDisplayRect.y = questDisplayRect.yMin + (maxQuestLog+1) *questLogHeight;
		notificationDisplayRect.width = questDisplayRect.width;
		notificationDisplayRect.height = questLogHeight;

		//notificationlabelStyle.fontSize = (int)(notificationDisplayRect.width/notificationText.Length)*2;
	}
	void OnGUI()
	{
		if(Application.loadedLevelName == "main_menu")
			return;
		if(display == true)
		{


			if(ScreenManager.Instance.CheckAspectChanged() == true)
			{
				UpdateDisplayRect();
			}

			GUI.Box(questDisplayRect,"Objective");
			for (int i = 0; i < questLogs.Count; ++i) 
			{
				int predictedSize = (int)(questLogRects[i].width/questLogs[i].questname.Length)*2;//the calcution isnt good enough to determine the nice fitting
				if(predictedSize < questlogStyles[i].fontSize)//dont let it become bigger than default size
				{
					questlogStyles[i].fontSize = predictedSize;
				}
				GUI.Box(questLogRects[i],questLogs[i].questname/*+ " : "*/+ questLogs[i].StringStatues(),questlogStyles[i]);

		
				//Vector2 temp = questlogStyles[i].CalcSize(new GUIContent(questLogs[i].questname));
				//questlogStyles[i].fontSize = (int)temp.x;

			}
			if(notificationText != "")
			{
				notificationTimer += Time.deltaTime;
				if(notificationTimer < notificaitonTimerLimit)
				{
					int predictedSize = (int)(notificationDisplayRect.width/notificationText.Length)*2;
					if(predictedSize < notificationlabelStyle.fontSize)//dont let it become bigger than default size
					{
						notificationlabelStyle.fontSize = predictedSize;
					}
					GUI.Box(notificationDisplayRect,notificationText,notificationlabelStyle);
				}else
				{
					notificationText = "";
				}
			}
		}
	}
	public void LoadQuestManager()
	{
		FileManager.Instance.LoadQuestManager();
	}
	public void SaveQuestManager()
	{
		FileManager.Instance.SaveQuestManager();
	}
	public void ToggleDisplay()
	{
		display = !display;
	}

}
