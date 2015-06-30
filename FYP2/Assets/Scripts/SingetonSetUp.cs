using UnityEngine;
using System.Collections;



// this script is suppose to attach to a blank object and initalize the singeton in the game
public class SingetonSetUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		FileManager.Instance.Initialize();
		ItemDatabase.Instance.Initialize();
		VendorDatabase.Instance.Initialize();
		DialogDatabase.Instance.Initialize();
		QuestManager.Instance.Initialize();
	}
	public void OnApplicationQuit()//this will be auto called like start and update function
	{
		//FileManager.Instance.SaveItemDatabase();
		//FileManager.Instance.SaveCraftDatabase();
		//FileManager.Instance.SaveVendorDatabase();
	}

}
