using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//this script will hold persistent data about the whole game about its levels
public class ApplicationLevelBoard : MonoBehaviour {

	public static ApplicationLevelBoard instance = null;
	public static bool initedBefore = false;

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
		Initialize(false);//don allow reinitalize of this class by default
	}
	public void Initialize(bool re_init)
	{
		if(initedBefore == false || re_init == true)
		{
			gameLevelNameList.Clear();
			LoadGameLevelInformation();
			initedBefore = true;
		}
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
