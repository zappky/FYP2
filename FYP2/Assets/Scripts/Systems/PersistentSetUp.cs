using UnityEngine;
using System.Collections;

public class PersistentSetUp : MonoBehaviour {

	public bool doneSetUp = false;
	// Use this for initialization
	void Awake () {
		SingetonInitialize(true);
	}
	void Start()
	{
		//FindObjectOfType<FinalizeSetUp>().Activate();
		//SingetonInitialize();
		//DontDestroyOnLoad(this);
	}
	public bool SingetonInitialize(bool stayPersistent)
	{
		bool result = SingetonInitialize();
		if(stayPersistent == true)
		{
			DontDestroyOnLoad(this.gameObject);
		}else
		{
			Destroy(this.gameObject);
		}
		return result;
	}
	public bool SingetonInitialize()
	{
		if(doneSetUp == false)
		{
			FileManager.Instance.Initialize();

			ApplicationLevelBoard.Instance.Initialize();
			ApplicationPlayerBoard.Instance.Initialize();
			AchievementManager.Instance.Initialize();


			LevelManager.Instance.Initialize();
			Debug.Log ("All persistent gameobject initialize complete =D ");
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
