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
			return "Quest Done";
		} else 
		{
			return "Not Done";
		}
	}
	public void ClearQuest()
	{
		this.statues = true;
	}
}

public class QuestManager : MonoBehaviour {

	public static QuestManager instance = null;
	public static bool initedBefore = false;

	public bool display = false;

	public QuestLogDatabase questlogdatabase = null;
	public my_QuestLogList questLogListDatabase = null;
	public List<my_QuestLog> questLogs = new List<my_QuestLog>();

	public Rect questDisplayRect;
	public List<Rect>questLogRects = new List<Rect>();
	public const int maxQuestLog = 1;
	private float questLogHeight = -1.0f;
	public int currentGameLevel = -1;
	public int currentQuestIndex = 0;
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
			
			questDisplayRect = new Rect (0.0f,Screen.height * 0.01f,Screen.width * 0.25f,Screen.height * 0.25f);
			questLogHeight = questDisplayRect.height / (maxQuestLog+1);
			for (int i = 0; i<maxQuestLog; ++i)//reserve some rect 
			{
				//questLogs.Add(new my_QuestLog());
				questLogRects.Add (new Rect (questDisplayRect.xMin,questDisplayRect.yMin + (i+1) *questLogHeight,questDisplayRect.width,questLogHeight));
			}
			FetchCurrentQuest();
			initedBefore = true;
		}
	}

	public void ClearQuest(my_QuestLog theQuest)
	{
		theQuest.ClearQuest();
		questLogs.Remove(theQuest);
		FetchNewQuest();
	}
	public void ClearQuest(int questId)
	{
		my_QuestLog theQuest = GetQuestLog(questId);
		if(theQuest != null)
		{
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
		DestroyInstance();
	}
	
	public void DestroyInstance()
	{
		instance = null;
	}
	void Update()
	{
		if(Input.GetButtonDown("QuestInterface"))
		{
			ToggleDisplay();
		}
	}
	void UpdateDisplayRect()
	{
		questDisplayRect = new Rect (0.0f,Screen.height * 0.01f,Screen.width * 0.25f,Screen.width * 0.25f);
		questLogHeight = questDisplayRect.height / (maxQuestLog+1);
		for (int i = 0; i<maxQuestLog; ++i)//reserve some rect 
		{
			questLogRects[i] = new Rect(questDisplayRect.xMin,questDisplayRect.yMin + (i+1) *questLogHeight,questDisplayRect.width,questLogHeight);
		}
	}
	void OnGUI()
	{
		if(ScreenManager.Instance.CheckAspectChanged() == true)
		{
			UpdateDisplayRect();
		}

		if(display == true)
		{


			GUI.Box(questDisplayRect,"Quest Logs");
			for (int i = 0; i < questLogs.Count; ++i) 
			{
				GUI.Box(questLogRects[i],questLogs[i].questname + " : "+ questLogs[i].StringStatues());
			}
		}

	}
	public void ToggleDisplay()
	{
		display = !display;
	}
}
