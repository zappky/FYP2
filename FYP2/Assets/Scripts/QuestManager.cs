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
}

public class QuestManager : MonoBehaviour {
	public bool display = false;
	public QuestLogDatabase questlogdatabase = null;
	public my_QuestLogList questLogListDatabase = null;
	public List<my_QuestLog> questLogs = new List<my_QuestLog>();
	public static QuestManager instance = null;
	public Rect questDisplayRect;
	public List<Rect>questLogRects = new List<Rect>();
	public int maxQuestLog = 5;
	public int currentGameLevel = 1;
	private float questLogHeight = -1.0f;

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

	public void FetchNewQuest()
	{
		this.questLogListDatabase = questlogdatabase.GetQuestLogListByLevel(currentGameLevel);

		for(int i = 0 ; i < questLogListDatabase.questlogs.Count ; ++i)
		{
			AddQuestLog(questLogListDatabase[i]);
		}
	}
	public void RemoveQuestLog(int questId)
	{
		if(questId >= 0 && questId <questLogs.Count)
		{
			if(questLogs[questId].id == questId)
			{
				questLogs[questId] = new my_QuestLog();
			}
		}
		
		for(int i = 0 ; i< questLogs.Count; ++i)
		{
			if(questLogs[i].id == questId)
			{
				questLogs[i] = new my_QuestLog();
			}
		}

	}
	public my_QuestLog RemoveQuestLog(string questLogName)
	{
		for(int i = 0 ; i< questLogs.Count; ++i)
		{
			if(questLogs[i].questname == questLogName)
			{
				questLogs[i] = new my_QuestLog();
			}
		}
		return null;
	}
	public my_QuestLog GetQuestLog(int questId)
	{
		if(questId >= 0 && questId <questLogs.Count)
		{
			if(questLogs[questId].id == questId)
			{
				return questLogs[questId];
			}
		}
		
		for(int i = 0 ; i< questLogs.Count; ++i)
		{
			if(questLogs[i].id == questId)
			{
				return questLogs[i];
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
		foreach (my_QuestLog a_log in questLogs)
		{
			if(a_log.id == a_questlog.id)
			{
				print ("ERROR: duplicate quest add detected, abort operation");
				return true;
			}
		}

		return false;
	}
	public void Initialize()
	{
		questlogdatabase = QuestLogDatabase.instance;
		questLogListDatabase = questlogdatabase.GetQuestLogListByLevel(currentGameLevel);

		questDisplayRect = new Rect (0.0f,Screen.height * 0.01f,Screen.width * 0.25f,Screen.width * 0.25f);
		questLogHeight = questDisplayRect.height / (maxQuestLog+1);
		for (int i = 0; i<maxQuestLog; ++i)//reserve some rect 
		{
			questLogRects.Add (new Rect (questDisplayRect.xMin,questDisplayRect.yMin + (i+1) *questLogHeight,questDisplayRect.width,questLogHeight));
		}

		FetchNewQuest();
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
