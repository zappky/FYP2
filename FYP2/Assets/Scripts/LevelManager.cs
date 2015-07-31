using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

[System.Serializable]
public class LevelManager : MonoBehaviour {

	public static LevelManager instance = null;
	public static bool initedBefore = false;

	public List<CheckPoint> checkPointList = new List<CheckPoint>();
	public bool loadFromContinue = false;

	public AchievementManager achievementmanager = null;
	public FileManager filemanager = null;
	public GameObject playerobj = null;
	public PlayerInfo playerinfo = null;
	public Inventory playerinventory = null;
	public CastSlot playercastslot = null;
	public DialogInterface playerdialoginferface = null;
	public HideCursorScript playercursor = null;
	public PauseMenu pauseMenu = null;

	public string playerSaveFileName = "checkpoint_playersave";
	public string playerdataPath = "";
	public string predefinedInventorySaveFileName = "predefined_inventory";
	public string predefinedInventoryPath = "";
	public int currentCheckPointIndex = -1; //the first check point should be at the starting point of the game


	
	public static LevelManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("Level Manager").AddComponent<LevelManager>();
				DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}
	public void Initialize()
	{
		Initialize(true);// allow reinitalize of this class by default
	}
	public void Initialize(bool re_init)
	{
		if(initedBefore == false || re_init == true)
		{
			//this.checkPointList = FindObjectsOfType<CheckPoint>().ToList();//find and insert all gameobject with checkpoint script
			this.filemanager = FileManager.Instance;
			this.achievementmanager = AchievementManager.Instance;
			this.playercursor = FindObjectOfType<HideCursorScript>();
			this.pauseMenu = FindObjectOfType<PauseMenu>();
			this.checkPointList.Clear();
			this.currentCheckPointIndex = -1;
			if(ApplicationLevelBoard.Instance.CheckValidGameLevelName(Application.loadedLevelName) == true)
			{
				List<CheckPoint> tempCheckPointList = new List<CheckPoint>();
				tempCheckPointList = FindObjectsOfType<CheckPoint>().OrderBy(go=>go.orderPlacement).ToList();
				for(int index = 0 ; index < tempCheckPointList.Count; ++index)
				{
					tempCheckPointList[index].indexInList = index;
					this.checkPointList.Add(tempCheckPointList[index]);
				}
				
				this.playerobj = GameObject.FindGameObjectWithTag("Player");
				this.playerinfo = playerobj.GetComponent<PlayerInfo>();
				this.playerinventory = playerobj.GetComponent<Inventory>();
				this.playercastslot = playerobj.GetComponent<CastSlot>();
				this.playerdialoginferface = DialogInterface.Instance;
			}
			
			this.playerdataPath = filemanager.GetGameDataPath() + "/" + filemanager.backupFolderName +"/"+ "backup_"+playerSaveFileName + ".xml";
			this.predefinedInventoryPath = filemanager.GetGameDataPath() + "/" + predefinedInventorySaveFileName + ".xml";
			initedBefore = true;
		}
	}

	public void DestroyInstance()
	{
		instance = null;
	}

	public void OnApplicationQuit()
	{
		//for testing purpose,please remove the save playerinfo call when release
		if(CurrentLevelName == "main-scene")
		{
			SavePlayerInfo();
		}

		DestroyInstance();
	}
	public void AddCheckPoint(CheckPoint a_checkpoint)
	{
		a_checkpoint.indexInList = checkPointList.Count+1;
		a_checkpoint.orderPlacement = a_checkpoint.indexInList;
		if(a_checkpoint.id < 0)
		{
			a_checkpoint.id = a_checkpoint.orderPlacement;
		}
		this.checkPointList.Add(a_checkpoint);
	}
	public int CurrentLevelIndex
	{
		get
		{
			return Application.loadedLevel;
		}
	}
	public string CurrentLevelName
	{
		get
		{
			return Application.loadedLevelName;
		}
	}
	public void LoadLevelWithCheckPoint(string levelname)
	{
		Application.LoadLevel(levelname);

		LoadPlayerInfo(false);
	}
	public void LoadLevel(string levelname,bool loadPredefinedInventory)
	{
		Application.LoadLevel(levelname);
		if(loadPredefinedInventory == true)
		{
			LoadPreDefinedInventory(levelname);
		}
	}
	public void LoadLevel(int levelindex,bool loadPredefinedInventory)
	{
		Application.LoadLevel(levelindex);
		if(loadPredefinedInventory == true)
		{
			LoadPreDefinedInventory(levelindex);
		}
	}

	// Update is called once per frame
	void Update () {
		if(playercursor != null)
		{
			switch(CurrentLevelName)
			{
				case "Level1":
				case "Level2":
					if(playerinventory.display == true  || playerdialoginferface.display == true
				   	|| pauseMenu.paused == true)
						//|| playercastslot.display == true
					{
						playercursor.SetVisiblity(true);
					}
					else
					{
						playercursor.SetVisiblity(false);
					}
					break;
				default:
					Cursor.visible = true;
					break;
			}
		}else
		{
			Debug.Log("ERROR: cursor script is null");
		}

	}
	public CheckPoint GetCurrentCheckPoint()
	{
		return checkPointList[currentCheckPointIndex];
	}
	public void LoadPreDefinedInventory(int levelIndex)
	{
		FileManager.Instance.LoadPreDefinedInventory(levelIndex);
	}
	public void LoadPreDefinedInventory(string levelName)
	{
		FileManager.Instance.LoadPreDefinedInventory(levelName);
	}
	public string GetLastCheckPointScene()
	{
		return FileManager.Instance.GetLastCheckPointScene();
	}
	public void LoadPlayerInfo()
	{
		LoadPlayerInfo(false);
	}
	public void LoadPlayerInfo(bool loadCheckPointLevel)
	{
		FileManager.Instance.LoadPlayerInfo(loadCheckPointLevel);
	}
	public void SavePlayerInfo()
	{
		FileManager.Instance.SavePlayerInfo();
	}
}
