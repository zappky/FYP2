using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//just a container class to hold a list of questlog
[System.Serializable]
public class my_QuestLogList : IEnumerable<my_QuestLog>
{
	public List<my_QuestLog> questlogs = new List<my_QuestLog>();

	public my_QuestLogList()
	{

	}

	public IEnumerator<my_QuestLog> GetEnumerator()
	{
		return questlogs.GetEnumerator();
	}
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return questlogs.GetEnumerator();
	}

	public my_QuestLogList(my_QuestLogList another)
	{
		this.questlogs = another.questlogs;
	}
	public my_QuestLog this[int index]
	{
		get
		{
			return questlogs[index];
		}
	}
	public my_QuestLog GetQuestLog(string questName)
	{
		for(int i = 0 ; i< questlogs.Count; ++i)
		{
			if(questlogs[i].questname == questName)
			{
				return questlogs[i];
			}
		}

		return null;
	}
	public my_QuestLog GetQuestLog(int questId)
	{
		if(questId < 0 )
		{
			Debug.Log("ERROR: GetQuestLog with id is negative value");
			return null;
		}
		if(questId < questlogs.Count )
		{
			//early prediction
			if(questlogs[questId].id == questId)
			{
				return questlogs[questId];
			}
			//if still cannot be found,brute force search
			for(int i = 0 ; i< questId; ++i)
			{
				if(questlogs[i].id == questId)
				{
					return questlogs[i];
				}
			}
			for(int i = questId+1 ; i< questlogs.Count; ++i)
			{
				if(questlogs[i].id == questId)
				{
					return questlogs[i];
				}
			}
		}else
		{
			Debug.Log("ERROR: GetQuestLog with id is over list size, brute force will be performed");
			for(int i = 0 ; i< questlogs.Count; ++i)
			{
				if(questlogs[i].id == questId)
				{
					return questlogs[i];
				}
			}
		}


		return null;
	}
}

//this script is going hold all of the in game quest log data.
[System.Serializable]
public class QuestLogDatabase : MonoBehaviour {

	//the index of the list will be used to mark as level.,in game.
	public List<my_QuestLogList>questLogDatabase = new List<my_QuestLogList>();
	public static QuestLogDatabase instance = null;
	public static bool initedBefore = false;

	public static QuestLogDatabase Instance
	{
		get
		{
			if(instance == null)
			{

				instance = new GameObject("QuestLog Database").AddComponent<QuestLogDatabase>();
				DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}
	public my_QuestLogList this[int index]
	{
		get
		{
			return questLogDatabase[index];
		}
	}
	public my_QuestLogList GetQuestLogListByLevel(int level)
	{
		level--;
		if(level >= 0 && level <questLogDatabase.Count)
		{
			return questLogDatabase[level];
		}
		return null;
	}
	public void Initialize()
	{
		Initialize(false);//dont allow reinitalize of this class by default
	}
	
	public void Initialize(bool re_init)
	{
		if(initedBefore == false || re_init == true)
		{
			LoadQuestLogData();
			initedBefore = true;
		}
	}
	public void SaveQuestLogData()
	{
		FileManager.Instance.SaveQuestDatabase();
	}
	public void LoadQuestLogData()
	{
		this.questLogDatabase = FileManager.Instance.LoadQuestLogData();
	}
	public void OnApplicationQuit()//this will be auto called like start and update function
	{
		SaveQuestLogData();
		DestroyInstance();
	}
	public void DestroyInstance()
	{
		instance = null;
	}


}
