using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class my_QuestLog
{
	public string questname = "";
	public bool statues = false;//true means done

	public my_QuestLog()
	{

	}

	public my_QuestLog(my_QuestLog another)
	{
		this.questname = another.questname;
		this.statues = another.statues;
	}

	public my_QuestLog(string questname)
	{
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
	public List<my_QuestLog> questLogs = new List<my_QuestLog>();
	public static QuestManager instance = null;
	public Rect questDisplayRect;
	public List<Rect>questLogRects = new List<Rect>();
	public int maxQuestLog = 5;

	public static QuestManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("Quest Manager").AddComponent<QuestManager>();
				instance.transform.parent = GameObject.Find("UI").transform;
			}
			return instance;
		}
	}
	public bool AddQuestLog(string newQuestName)
	{
		return AddQuestLog(new my_QuestLog(newQuestName));
	}
	public bool AddQuestLog(my_QuestLog a_questlog)
	{
		if(questLogs.Count >= maxQuestLog)
		{
			print ("max quest log reached, cannot add anymore log");
			return false;
		}else
		{
			questLogs.Add(a_questlog);
			return true;
		}
	}

	public void TestInitData()
	{
		AddQuestLog("test quest log 1");
		AddQuestLog("test quest log 2");
		AddQuestLog("test quest log 3");
		AddQuestLog("test quest log 4");
		AddQuestLog("test quest log 5");
	}
	public void Initialize()
	{
		questDisplayRect = new Rect (0.0f,Screen.height * 0.3f,Screen.width * 0.3f,Screen.width * 0.3f);
		float questLogHeight = questDisplayRect.height / (maxQuestLog+1);
		for (int i = 0; i<maxQuestLog; ++i)//reserve some rect 
		{
			questLogRects.Add (new Rect (questDisplayRect.xMin,questDisplayRect.yMin + (i+1) *questLogHeight,questDisplayRect.width,questLogHeight));
		}
		TestInitData();
	}
	
	public void OnApplicationQuit()
	{
		DestroyInstance();
	}
	
	public void DestroyInstance()
	{
		instance = null;
	}
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		questDisplayRect = new Rect (0.0f,Screen.height * 0.3f,Screen.width * 0.3f,Screen.width * 0.3f);
		float questLogHeight = questDisplayRect.height / (maxQuestLog+1);
		for (int i = 0; i<maxQuestLog; ++i)//reserve some rect 
		{
			questLogRects[i] = new Rect(questDisplayRect.xMin,questDisplayRect.yMin + (i+1) *questLogHeight,questDisplayRect.width,questLogHeight);
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
