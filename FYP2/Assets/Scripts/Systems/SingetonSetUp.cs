using UnityEngine;
using System.Collections;



// this script is suppose to attach to a blank object and initalize the singeton in the game
public class SingetonSetUp : MonoBehaviour {

	public bool doneSetUp = false;
	// Use this for initialization
	void Awake () {
		SingetonInitialize(false);
	}
	void Start()
	{
		//SingetonInitialize(false);
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
			//checking
			//PersistentSetUp persistobj =GameObject.FindObjectOfType<PersistentSetUp>();

			FileManager.Instance.Initialize();

			PersistentSetUp persistobj = FindObjectOfType<PersistentSetUp>();
			if(persistobj == null)
			{
				persistobj = this.gameObject.AddComponent<PersistentSetUp>();
				persistobj.SingetonInitialize(false);
			}else
			{
				Destroy(persistobj.gameObject);
			}


			ScreenManager.Instance.Initialize();
			ItemDatabase.Instance.Initialize();
			VendorDatabase.Instance.Initialize();
			DialogDatabase.Instance.Initialize();
			QuestLogDatabase.Instance.Initialize();
			LevelManager.Instance.Initialize();
			QuestManager.Instance.Initialize();
			DialogInterface.Instance.Initialize();

			DebugControl.Instance.Initialize();//let it be last
			Debug.Log ("All Game singeton gameobject initialize complete =D ");

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
