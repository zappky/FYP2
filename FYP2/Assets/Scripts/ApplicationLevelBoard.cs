using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//this script will hold persistent data about the whole game about its levels
public class ApplicationLevelBoard : MonoBehaviour {

	public static ApplicationLevelBoard instance = null;

	public int maxLevelCount = -1;
	public List<string>gameLevelNameList = new List<string>();
	
	public static ApplicationLevelBoard Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("Application Level Board").AddComponent<ApplicationLevelBoard>();
				DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}
	
	public void Initialize()
	{
		LoadGameLevelInformation();
	}

	public void OnApplicationQuit()
	{
		SaveGameLevelInformation();
	}

	public void AddGameLevelNameList(string name)
	{
		gameLevelNameList.Add(name);
	}

	public void LoadGameLevelInformation()
	{
		FileManager.Instance.LoadGameLevelInformation();
	}

	public void SaveGameLevelInformation()
	{
		FileManager.Instance.SaveGameLevelInformation();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
