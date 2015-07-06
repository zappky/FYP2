using UnityEngine;
using System.Collections;



// this script is suppose to attach to a blank object and initalize the singeton in the game
public class SingetonSetUp : MonoBehaviour {

	public bool doneSetUp = false;
	// Use this for initialization
	void Awake () {

	}
	void Start()
	{
		//FindObjectOfType<FinalizeSetUp>().Activate();
		SingetonInitialize();
	}

	public bool SingetonInitialize()
	{
		if(doneSetUp == false)
		{
			FileManager.Instance.Initialize();
			ScreenManager.Instance.Initialize();
			ItemDatabase.Instance.Initialize();
			VendorDatabase.Instance.Initialize();
			DialogDatabase.Instance.Initialize();
			QuestLogDatabase.Instance.Initialize();
			LevelManager.Instance.Initialize();
			QuestManager.Instance.Initialize();
			DialogInterface.Instance.Initialize();
			doneSetUp = true;
		}


		return doneSetUp;

	}
	public void OnApplicationQuit()//this will be auto called like start and update function
	{
		//FileManager.Instance.SaveItemDatabase();
		//FileManager.Instance.SaveCraftDatabase();
		//FileManager.Instance.SaveVendorDatabase();
	}

}
