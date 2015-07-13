using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script is to describle a collectible item//not a in game collectible behavoiour
[System.Serializable]
public class my_Collectible//may not be needed as using a list int of count would be enough
{
	// a icon variable?
	public my_Collectible()
	{

	}

	public my_Collectible(my_Collectible another)
	{

	}
}
[System.Serializable]
public class my_CollectibleList
{
	public string listName = "";

	public List<my_Collectible> collectibleList = new List<my_Collectible>();

	public my_CollectibleList()
	{
		
	}

	public my_CollectibleList(string name)
	{
		listName = name;
	}

	public my_CollectibleList(string name,int collectiblecount)
	{
		listName = name;
		for(int i = 0 ; i < collectiblecount; ++i)
		{
			collectibleList.Add(new my_Collectible());
		}
	}
	
	public my_CollectibleList(my_CollectibleList another)
	{
		this.listName = another.listName;
		this.collectibleList = another.collectibleList;
	}
}

//a singeton class to hold information about a player collectibe achievement
[System.Serializable]
public class AchievementManager : MonoBehaviour {

	public static AchievementManager instance = null;
	public ApplicationLevelBoard levelboard = null;
	public List<my_CollectibleList> collectibleAchievedList = new List<my_CollectibleList>();//index of this list represent the level, each level for its list of collectible

	public static AchievementManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("Achievement Manager").AddComponent<AchievementManager>();
				DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}

	public void Initialize()
	{
		levelboard = ApplicationLevelBoard.Instance;

//		int stopper = levelboard.maxLevelCount;
//		if(stopper > levelboard.gameLevelNameList.Count)//to prevent case where the max level is over the name provided, stopping argument out of range error//non essential coding
//		{
//			stopper = levelboard.gameLevelNameList.Count;
//		}
//
//		for(int i = 0; i< stopper; ++i )//init the max amount of collectible in each stage
//		{
//			collectibleAchievedList.Add( new my_CollectibleList( levelboard.gameLevelNameList[i] ) );
//		}
		LoadAchievementData();
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void OnApplicationQuit()
	{
		SaveAchievementData();
	}

	public void SaveAchievementData()
	{
		FileManager.Instance.SaveAchievementData();
	}

	public void LoadAchievementData()
	{
		FileManager.Instance.LoadAchievementData();
	}

	public void AddCollectibleLevel(string collectiblelevelname, int collectiblecount)
	{
		collectibleAchievedList.Add(new my_CollectibleList(collectiblelevelname,collectiblecount));
	}

	public void AddCollectibleLevel(my_CollectibleList level)
	{
		collectibleAchievedList.Add(level);
	}

	public void AddCollectible(string level , int amount)
	{
		int failure = 0;
		for(int i = 0 ; i < amount ;++i)
		{
			if(AddCollectible(level) == false)
			{
				++failure;
			}
		}
		
		if(failure > 0)
		{
			Debug.Log("Amount of failure to add collectible: " + failure);
		}
	}
	public bool AddCollectible(string level)
	{
		return AddCollectible(level,new my_Collectible());
	}
	public bool AddCollectible(string level,my_Collectible a_collectible)
	{
		foreach(my_CollectibleList a_level_collectibles in collectibleAchievedList)
		{
			if(a_level_collectibles.listName == level)
			{
				a_level_collectibles.collectibleList.Add(a_collectible);
				return true;
			}
		}

		return false;
	}

	public void AddCollectible(int level , int amount)
	{
		int failure = 0;
		for(int i = 0 ; i < amount ;++i)
		{
			if(AddCollectible(level) == false)
			{
				++failure;
			}
		}

		if(failure > 0)
		{
			Debug.Log("Amount of failure to add collectible: " + failure);
		}

	}
	public bool AddCollectible(int level)
	{
		return AddCollectible(level,new my_Collectible());
	}
	public bool AddCollectible(int level,my_Collectible a_collectible)
	{
		if(level > collectibleAchievedList.Count || level < 0)
		{
			Debug.Log("ERROR: accessing out of range collectibleAchievedList");
			return false;
		}
		collectibleAchievedList[level].collectibleList.Add(a_collectible);
		return true;
	}
}
