using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

public class LevelManager : MonoBehaviour {

	public List<CheckPoint> checkPointList = new List<CheckPoint>();

	public FileManager filemanager = null;
	public GameObject playerobj = null;
	public PlayerInfo playerinfo = null;
	public Inventory playerinventory = null;
	public CastSlot playercastslot = null;

	public string playerSaveFileName = "checkpoint_playersave";
	public string playerdataPath = "";
	public string predefinedInventorySaveFileName = "predefined_inventory";
	public string predefinedInventoryPath = "";

	public int currentLevel = -1;
	public int currentCheckPointIndex = 0; //the first check point should be at the starting point of the game


	// Use this for initialization
	void Start () {
		this.checkPointList = FindObjectsOfType<CheckPoint>().ToList();//find and insert all gameobject with checkpoint script
		this.playerobj = GameObject.FindGameObjectWithTag("Player");
		this.playerinfo = playerobj.GetComponent<PlayerInfo>();
		this.filemanager = FileManager.Instance;
		this.playerinventory = playerobj.GetComponent<Inventory>();
		this.playercastslot = playerobj.GetComponent<CastSlot>();
		this.playerdataPath = filemanager.GetGameDataPath() + "/" + filemanager.backupFolderName +"/"+ "backup_"+playerSaveFileName + ".xml";
		this.predefinedInventorySaveFileName = filemanager.GetGameDataPath() + "/" + predefinedInventorySaveFileName + ".xml";
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("m"))
		{
			LoadPlayerInfo();
		}
		if(Input.GetKeyDown("n"))
		{
			LoadPreDefinedInventory(currentLevel);
		}

	}

	public void OnApplicationQuit()
	{
		//for testing purpose,please remove the save playerinfo call when release
		SavePlayerInfo();
	}
	public void LoadPreDefinedInventory(int level)
	{
		if( filemanager.CheckFile(predefinedInventorySaveFileName,false) == false)
		{
			print ("ERROR: predefined Inventory save data cannot be found, load data aborted");
			return ;
			
		}
		print ("loading  predefined Inventory save data");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(predefinedInventorySaveFileName);
		
		XmlNodeList parentList = null;
		parentList = xmlDoc.GetElementsByTagName("database");
		parentList = filemanager.DigToDesiredChildNodeList(parentList,"inventory");
		

		bool isNumeric = false;
		int n = -1;
		foreach (XmlNode levelsection in parentList)
		{
			if(currentLevel != int.Parse(levelsection.Attributes["id"].Value))
			{
				continue;
			}

			foreach (XmlNode itemsection in levelsection.ChildNodes)
			{
				switch(itemsection.Name)
				{
					case "Item":
					case "item":

					int amountToAdd = int.Parse(itemsection.Attributes["amount"].Value);

					isNumeric = int.TryParse(itemsection.Attributes["id"].Value, out n);
					if(isNumeric == true)
					{
						playerinventory.AddItem(n,amountToAdd);
					}else
					{
						playerinventory.AddItem(itemsection.Attributes["id"].Value,amountToAdd);
					}



					break;

					default:
					print ("ERROR: Unhandled itemsection item field detected: " + itemsection.Name);
					break;
				}
			}
		}

	}
	public void LoadPlayerInfo()
	{
		LoadPlayerInfo(false);
	}
	public void LoadPlayerInfo(bool loadCheckPointLevel)
	{

		if( filemanager.CheckFile(playerdataPath,false) == false)
		{
			print ("ERROR: player checkpoint save data cannot be found, load data aborted");
			return ;
			
		}
		print ("loading  player checkpoint save data");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(playerdataPath);
				
		XmlNodeList parentList = null;
		parentList = xmlDoc.GetElementsByTagName("database");
		parentList = filemanager.DigToDesiredChildNodeList(parentList,"player");

		Item tempitem = null;


		XmlNodeList sectioncontent = null;
		foreach (XmlNode playerinfosection in parentList)
		{

			switch(playerinfosection.Name)
			{

			case "Level":
			case "level":
				sectioncontent = playerinfosection.ChildNodes;
				foreach (XmlNode sectionitem in sectioncontent)
				{
					switch (sectionitem.Name)
					{
					case "currentlevel_raw":
						//Application.LoadLevel(int.Parse(sectionitem.InnerText));
						break;
						
					case "currentlevel_unity":
						if(loadCheckPointLevel == true)
						{
							Application.LoadLevel( int.Parse(sectionitem.Attributes["number"].Value) );
						}
						break;
						
					default:
						print ("ERROR: Unhandled level section item field detected: " + sectionitem.Name);
						break;
					}
				}
				break;

			case "Info":
			case "info":
				if(playerinfo != null)
				{
					sectioncontent = playerinfosection.ChildNodes;
					foreach (XmlNode sectionitem in sectioncontent)
					{
						switch (sectionitem.Name)
						{
						case "Name":
						case "name":
							playerinfo.playername = sectionitem.InnerText;
							break;
							
						case "Weight":
						case "weight":
							playerinfo.weight = int.Parse(sectionitem.InnerText);
							break;
							
						default:
							print ("ERROR: Unhandled player info section item field detected: " + sectionitem.Name);
							break;
						}
					}
				}else
				{
					print ("ERROR: PlayerInfoScript null detected, playerinfo load skipped");
				}

				break;

			case "Quickcast":
			case "quickcast":
				if(playercastslot != null)
				{
					sectioncontent = playerinfosection.ChildNodes;
					foreach (XmlNode sectionitem in sectioncontent)
					{
						switch (sectionitem.Name)
						{
						case "layer":
							int layerindex = int.Parse(sectionitem.Attributes["index"].Value);
							foreach(XmlNode item in sectionitem.ChildNodes)
							{
								switch(item.Name)
								{
								case "item":
									tempitem = new Item();
									
									tempitem.id = int.Parse(item.Attributes["id"].Value);
									tempitem.itemname = item.Attributes["name"].Value;
									tempitem.SelfReloadIcon();
									playercastslot.AddItemKnown(tempitem,layerindex,int.Parse(item.Attributes["index"].Value) );
									
									break;
								default:
									print ("ERROR: Unhandled quick cast item field detected: " + item.Name);
									break;
								}
							}
							break;
							
						default:
							print ("ERROR: Unhandled quick cast section item field detected: " + sectionitem.Name);
							break;
						}
					}
				}else
				{
					print ("ERROR: Player castslot script null detected, castslot load skipped");
				}	

				break;

			case "Checkpoint":
			case "checkpoint":
				int checkpointindex = int.Parse(playerinfosection.Attributes["index"].Value);
				playerobj.transform.position = checkPointList[checkpointindex].transform.position;
				break;

			case "Inventory":
			case "inventory":
				if(playerinventory != null)
				{
					sectioncontent = playerinfosection.ChildNodes;
					foreach (XmlNode sectionitem in sectioncontent)
					{
						switch (sectionitem.Name)
						{
						case "item":
							tempitem = new Item();
							
							tempitem.id = int.Parse(sectionitem.Attributes["id"].Value);
							tempitem.itemname = sectionitem.Attributes["name"].Value;
							tempitem.amount = int.Parse(sectionitem.Attributes["amount"].Value);
							tempitem.itemindexinlist = int.Parse(sectionitem.Attributes["index"].Value);
							tempitem.description = sectionitem.Attributes["description"].Value;
							tempitem.SetItemType(sectionitem.Attributes["type"].Value);
							tempitem.SetStackable(sectionitem.Attributes["stackable"].Value);
							tempitem.SelfReloadIcon();
							playerinventory.AddItemKnown(tempitem,tempitem.itemindexinlist);
							
							break;
							
						default:
							print ("ERROR: Unhandled inventory section item field detected: " + sectionitem.Name);
							break;
						}
					}
				}else
				{
					print ("ERROR: Player inventory script null detected, PlayerInventory load skipped");
				}

				break;

			default:
				print ("ERROR: Player checkpoint load - Unhandled player info section detected: " + playerinfosection.Name);
				break;
			}

		}


	}
	public void SavePlayerInfo()
	{
		my_XmlEntry rootentry = new my_XmlEntry("database",null);
		
		List<my_XmlBuildEntry> inputlist = new List<my_XmlBuildEntry>();
		List<string> attkey = new List<string>();
		List<string> attvalue = new List<string>();
		
		List<string> parentlist = new List<string>();
		List<my_XmlEntry> entrylist = new List<my_XmlEntry>();

		parentlist.Add(rootentry.name);
		entrylist.Add(new my_XmlEntry("player",null,null,null));

		parentlist.Add("player");
		entrylist.Add(new my_XmlEntry("level",null,null,null));
		
		parentlist.Add("level");
		entrylist.Add(new my_XmlEntry("currentlevel_raw",this.currentLevel.ToString(),null,null));
		
		parentlist.Add("level");
		attkey.Clear();
		attvalue.Clear();
		attkey.Add("name");
		attvalue.Add(Application.loadedLevelName);
		attkey.Add("number");
		attvalue.Add(Application.loadedLevel.ToString());
		entrylist.Add(new my_XmlEntry("currentlevel_unity",null,attkey,attvalue));


		if(playerinfo != null)
		{
			parentlist.Add("player");
			entrylist.Add(new my_XmlEntry("info",null,null,null));

			parentlist.Add("info");
			entrylist.Add(new my_XmlEntry("name",playerinfo.playername,null,null));

			parentlist.Add("info");
			entrylist.Add(new my_XmlEntry("weight",playerinfo.weight.ToString(),null,null));
		}else
		{
			print ("ERROR: PlayerInfoScript null detected, playerinfo save skipped");
		}



		if(playercastslot != null)
		{
			parentlist.Add("player");
			entrylist.Add(new my_XmlEntry("quickcast",null,null,null));

			for( int i = 0 ; i< playercastslot.slotsLayerList.Count; ++i)
			{
				parentlist.Add("quickcast");
				attkey.Clear();
				attvalue.Clear();
				attkey.Add("index");
				attvalue.Add(i.ToString());
				entrylist.Add(new my_XmlEntry("layer",null,attkey,attvalue));

				for (int i2 = 0 ; i2< playercastslot.slotsLayerList[i].slots.Count; ++i2)
				{
					Item item =  playercastslot.slotsLayerList[i].slots[i2];
					if(item.id < 0 )
					{
						continue;
					}

					parentlist.Add("layer");
					attkey.Clear();
					attvalue.Clear();
					attkey.Add("id");
					attvalue.Add(item.id.ToString());
					attkey.Add("name");
					attvalue.Add(item.itemname);
					attkey.Add("index");
					attvalue.Add(i2.ToString());
					entrylist.Add(new my_XmlEntry("item",null,attkey,attvalue));
				}
			}

		}else
		{
			print ("ERROR: Player castslot script null detected, castslot save skipped");
		}

		
		
		if(checkPointList.Count > 0 && currentCheckPointIndex >= 0 && currentCheckPointIndex < checkPointList.Count)
		{
			parentlist.Add("player");
				attkey.Clear();
				attvalue.Clear();
				attkey.Add("id");
				attvalue.Add(checkPointList[currentCheckPointIndex].id.ToString());
				attkey.Add("name");
				attvalue.Add(checkPointList[currentCheckPointIndex].checkPointName);
				attkey.Add("index");
				attvalue.Add(currentCheckPointIndex.ToString());
				attkey.Add("x");
				attvalue.Add(checkPointList[currentCheckPointIndex].transform.position.x.ToString());	
				attkey.Add("y");
				attvalue.Add(checkPointList[currentCheckPointIndex].transform.position.y.ToString());	
				attkey.Add("z");
				attvalue.Add(checkPointList[currentCheckPointIndex].transform.position.z.ToString());	
			entrylist.Add(new my_XmlEntry("checkpoint",null,attkey,attvalue));
		}else
		{
			print ("ERROR: Invalid checkpoint save detected, checkpoint save skipped");
		}

		if(playerinventory != null)
		{
			if(playerinventory.inventory.Count >= 1)
			{
				parentlist.Add("player");
				entrylist.Add(new my_XmlEntry("inventory",null,null,null));
				
				for(int i = 0 ; i < playerinventory.inventory.Count ;++i )
				{
					Item item = playerinventory.inventory[i];

					if(item.id < 0 )
					{
						continue;
					}

					parentlist.Add("inventory");
					attkey.Clear();
					attvalue.Clear();
					attkey.Add("index");
					attvalue.Add(i.ToString());
					attkey.Add("id");
					attvalue.Add(item.id.ToString());
					attkey.Add("name");
					attvalue.Add(item.itemname);
					attkey.Add("amount");
					attvalue.Add(item.amount.ToString());
					attkey.Add("description");
					attvalue.Add(item.description);
					attkey.Add("stackable");
					attvalue.Add(item.stackable.ToString());
					attkey.Add("type");
					attvalue.Add(item.GetItemType());
					entrylist.Add(new my_XmlEntry("item",null,attkey,attvalue));
				}
			}else
			{
				print ("ERROR: Invalid PlayerInventory save detected, PlayerInventory save skipped");
			}
		}else
		{
			print ("ERROR: Player inventory script null detected, PlayerInventory save skipped");
		}


		inputlist = filemanager.CreateXmlBuildEntryList(parentlist,entrylist);	
		filemanager.CreateXMLFile("gamedata/" + filemanager.backupFolderName,"backup_"+ playerSaveFileName,"xml",filemanager.BuildXMLData(rootentry,inputlist),"plaintext");
		
	}
}
