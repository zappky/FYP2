using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script is to describle a collectible item//not a in game collectible behavoiour
[System.Serializable]
public class my_Collectible//may not be needed as using a list int of count would be enough
{
	// a icon variable?
	public string collectibleName = "";
	public int id = -1;

	public my_Collectible()
	{

	}
	public my_Collectible(int id , string name)
	{
		this.id = id;
		this.collectibleName = name;
	}
	public my_Collectible(string name)
	{
		this.collectibleName = name;
	}
	public my_Collectible(int id)
	{
		this.id = id;
	}

	public my_Collectible(my_Collectible another)
	{
		this.id = another.id;
		this.collectibleName = another.collectibleName;
	}
}
[System.Serializable]
public class my_CollectibleList: IEnumerable<my_Collectible>
{
	public string listName = "";
	public int id = -1;
	public List<my_Collectible> collectibleList = new List<my_Collectible>();

	public my_Collectible this[int index]
	{
		get
		{
			return collectibleList[index];
		}
	}

	public IEnumerator<my_Collectible> GetEnumerator()
	{
		return collectibleList.GetEnumerator();
	}
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return collectibleList.GetEnumerator();
	}

	public my_CollectibleList()
	{
		
	}

	public my_CollectibleList(my_CollectibleList another)
	{
		this.id = another.id;
		this.listName = another.listName;
		this.collectibleList = another.collectibleList;
	}

	public my_CollectibleList(int id)
	{
		this.id = id;
	}

	public my_CollectibleList(string name)
	{
		this.listName = name;
	}
		
	public my_CollectibleList(int id,string name)
	{
		this.listName = name;
		this.id = id;
	}

	public my_CollectibleList(int id,int collectiblecount)
	{
		this.id  = id;
		for(int i = 0 ; i < collectiblecount; ++i)
		{
			collectibleList.Add(new my_Collectible(i));
		}
	}

	public my_CollectibleList(string name,int collectiblecount)
	{
		this.listName = name;
		for(int i = 0 ; i < collectiblecount; ++i)
		{
			collectibleList.Add(new my_Collectible(i));
		}
	}

	public my_CollectibleList(int id,string name,int collectiblecount)
	{
		this.id = id;
		this.listName = name;
		for(int i = 0 ; i < collectiblecount; ++i)
		{
			collectibleList.Add(new my_Collectible(i));
		}
	}
	
	public my_Collectible GetCollectible(int collectibleId)
	{
		if(collectibleId < 0)
		{
			Debug.Log("ERROR:  Trying to access list collectible List agrument out of range");
			return null;
		}
		
		if(collectibleId > collectibleList.Count)
		{
			//brute force check
			for(int i = 0; i < collectibleList.Count; ++i)
			{
				if(collectibleList[i].id == collectibleId)
				{
					return collectibleList[i];
				}
			}
		}else
		{
			//if collectible id is withing range
			if(collectibleList[collectibleId].id == collectibleId)
			{
				return collectibleList[collectibleId];
			}
			
		}
		return null;
	}
	public my_Collectible GetCollectible(string collectibleName)
	{
		//brute force check
		for(int i = 0; i < collectibleList.Count; ++i)
		{
			if(collectibleList[i].collectibleName == collectibleName)
			{
				return collectibleList[i];
			}
		}
		return null;
	}
	
	public void AddCollectible(my_Collectible a_collectible)
	{
		a_collectible.id = collectibleList.Count;
		this.collectibleList.Add(a_collectible);
	}
	public void AddCollectible(int collectibleId)
	{
		this.collectibleList.Add(new my_Collectible(collectibleId));
	}
	public void AddCollectible(string collectibleName)
	{
		this.collectibleList.Add(new my_Collectible(collectibleList.Count,collectibleName));
	}
	public void AddCollectible(int collectibleId,string collectibleName)
	{
		this.collectibleList.Add(new my_Collectible(collectibleId,collectibleName));
	}
	public bool DuplicateCollectibleCheck(my_Collectible a_collectible)//passing in a sample
	{
		if(a_collectible.id < 0)
		{
			Debug.Log("ERROR:  Trying to access list collectible List agrument out of range");
			return false;
		}
		
		if(a_collectible.id  > collectibleList.Count)
		{
			//brute force check
			if(a_collectible.collectibleName == "")//if sample name is invalid then skip the checking of name
			{
				for(int i = 0; i < collectibleList.Count; ++i)
				{
					if(collectibleList[i].id == a_collectible.id )
					{
						return true;
					}
				}
			}else
			{
				for(int i = 0; i < collectibleList.Count; ++i)//else check both id and name
				{
					if(collectibleList[i].id == a_collectible.id || collectibleList[i].collectibleName == a_collectible.collectibleName)
					{
						return true;
					}
				}
			}

		}else
		{
			//if collectible id is withing range//perform early check
			if(collectibleList[a_collectible.id].id == a_collectible.id || collectibleList[a_collectible.id].collectibleName == a_collectible.collectibleName)
			{
				return true;
			}
			
		}
		return false;
	}
	public bool DuplicateCollectibleCheck(int collectibleId)
	{
		if(GetCollectible(collectibleId) != null)
		{
			return true;
		}
		return false;
	}

	public bool DuplicateCollectibleCheck(string collectibleName)
	{
		if(GetCollectible(collectibleName) != null)
		{
			return true;
		}
		return false;
	}
}

//a singeton class to hold information about a player collectibe achievement
[System.Serializable]
public class AchievementManager : MonoBehaviour {

	public static AchievementManager instance = null;
	public static bool initedBefore = false;

	public ApplicationLevelBoard levelboard = null;
	public List<my_CollectibleList> collectibleAchievedList = new List<my_CollectibleList>();//index of this list represent the level, each level for its list of collectible

	public my_CollectibleList this[int index]
	{
		get
		{
			return collectibleAchievedList[index];
		}
	}

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
		Initialize(false);//don allow reinitalize of this class by default
	}
	public void Initialize(bool re_init)
	{
		if(initedBefore == false || re_init == true)
		{
			Debug.Log("acheivement system init");
			levelboard = ApplicationLevelBoard.Instance;
			LoadAchievementData();
			initedBefore = true;
		}
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
	
	public my_CollectibleList GetCollectibleLevel(int collectibleLevelId)
	{
		if(collectibleLevelId < 0)
		{
			Debug.Log("ERROR: accessing out of range collectible Achieved List");
			return null;
		}
		
		if(collectibleLevelId > collectibleAchievedList.Count)
		{
			for (int i = 0 ; i<collectibleAchievedList.Count;++i )//brute force loop check , cos there still a chance the id is correct;
			{
				if(collectibleAchievedList[i].id == collectibleLevelId)
				{
					collectibleAchievedList[i].id = i;//auto correcting id
					return collectibleAchievedList[i];
				}
			}
		}else
		{
			if(collectibleAchievedList[collectibleLevelId].id == collectibleLevelId)//if early check found
			{
				return collectibleAchievedList[collectibleLevelId];
			}
		}
		
		return null;
	}
	public my_CollectibleList GetCollectibleLevel(string collectibleLevelName)
	{
		for (int i = 0 ; i<collectibleAchievedList.Count;++i )//brute force loop check , cos there still a chance the id is correct;
		{
			if(collectibleAchievedList[i].listName == collectibleLevelName)
			{
				return collectibleAchievedList[i];
			}
		}

		return null;
	}

	public void AddCollectibleLevel(string collectiblelevelname, int collectiblecount)
	{
		collectibleAchievedList.Add(new my_CollectibleList(collectibleAchievedList.Count,collectiblelevelname,collectiblecount));
	}

	public void AddCollectibleLevel(my_CollectibleList collectibleLevel)
	{
		collectibleLevel.id = collectibleAchievedList.Count;
		collectibleAchievedList.Add(collectibleLevel);
	}

	public void AddCollectible(int collectibleLevelId,int a_collectibleId,string a_collectibleName)
	{
		GetCollectibleLevel(collectibleLevelId).AddCollectible(a_collectibleId,a_collectibleName);
	}
	public void AddCollectible(string collectibleLevelName,int a_collectibleId,string a_collectibleName)
	{
		GetCollectibleLevel(collectibleLevelName).AddCollectible(a_collectibleId,a_collectibleName);
	}
	public void AddCollectible(int collectibleLevelId,string a_collectibleName)
	{
		GetCollectibleLevel(collectibleLevelId).AddCollectible(a_collectibleName);
	}
	public void AddCollectible(string collectibleLevelName,string a_collectibleName)
	{
		GetCollectibleLevel(collectibleLevelName).AddCollectible(a_collectibleName);
	}
	public void AddCollectible(int collectibleLevelId,int a_collectibleId)
	{
		GetCollectibleLevel(collectibleLevelId).AddCollectible(a_collectibleId);
	}
	public void AddCollectible(string collectibleLevelName,int a_collectibleId)
	{
		GetCollectibleLevel(collectibleLevelName).AddCollectible(a_collectibleId);
	}
	public void AddCollectible(int collectibleLevelId,my_Collectible a_collectible)
	{
		GetCollectibleLevel(collectibleLevelId).AddCollectible(a_collectible);
	}
	public void AddCollectible(string collectibleLevelName,my_Collectible a_collectible)
	{
		GetCollectibleLevel(collectibleLevelName).AddCollectible(a_collectible);
	}
	public bool DuplicateCollectibleLevelCheck(string levelName)
	{
		if(GetCollectibleLevel(levelName) != null)
		{
			return true;
		}
		return false;
	}
	public bool DuplicateCollectibleLevelCheck(int levelId)
	{
		if(GetCollectibleLevel(levelId) != null)
		{
			return true;
		}
		return false;
	}
}
