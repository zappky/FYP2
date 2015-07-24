using UnityEngine;
//using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

public class my_XmlBuildEntry//a class to describle a xml entry for building new xml node in the file
{
	public string parentname = null;
	public my_XmlEntry entry = null;

	public my_XmlBuildEntry()
	{
		this.parentname = null;
		this.entry = null;
	}
	public my_XmlBuildEntry(my_XmlBuildEntry another)
	{
		this.parentname = another.parentname;
		this.entry = another.entry;
	}
	public my_XmlBuildEntry(string parentname,my_XmlEntry entry)
	{
		this.parentname = parentname;
		this.entry = entry;
	}
	public string StringSelf()
	{
		return "Parent name: " + parentname + " " +entry.StringSelf();
	}
}
public class my_XmlEntry // a class to describle a xml node
{
	public string name = null;
	public string innerValue = null;
	public List<string> attributeKeys = new List<string>();
	public List<string> attributeValues = new List<string>();

	public my_XmlEntry()
	{
		this.name = null;
		this.innerValue = null;
		this.attributeKeys = new List<string>();
		this.attributeValues = new List<string>();
	}
	public my_XmlEntry(my_XmlEntry another)
	{
		this.name = another.name;
		this.innerValue = another.innerValue;
		this.attributeKeys = another.attributeKeys;
		this.attributeValues = another.attributeValues;
	}
	public my_XmlEntry(string name ,string innerValue)
	{
		this.name = name;
		this.innerValue = innerValue;
		this.attributeKeys = new List<string>();
		this.attributeValues = new List<string>();
	}

	public my_XmlEntry(string name ,string innerValue,List<string> newAttributeKeys,List<string> newAttributeValues)
	{
		this.name = name;
		this.innerValue = innerValue;

        if (newAttributeKeys != null)
        {
            for (int i = 0; i < newAttributeKeys.Count; ++i)
                this.attributeKeys.Add(newAttributeKeys[i]);
        }

        if (newAttributeValues != null)
        {
            for (int i = 0; i < newAttributeValues.Count; ++i)
                this.attributeValues.Add(newAttributeValues[i]);
        }
	}

	public string StringSelf()
	{
		string temp = "";

		for(int i = 0 ; i< attributeKeys.Count ; ++i)
		{
			temp += " " + this.attributeKeys[i]+ " = " + this.attributeValues[i];
		}
		return "Name: " + name + " inner value: " + innerValue + temp;
	}
}
//this class is suppose to management of folder and file ,xml file, using code.
public class FileManager : MonoBehaviour {
	//singeton//it should persist until game ends
	public static FileManager instance = null;
	public static bool initedBefore = false;

	public string path = null;
	public string backupPath = null;
	public string gamedataPath = null;
	public string itemdataPath = null;
	public string craftdataPath = null;
	public string vendordataPath = null;
	public string dialogdataPath = null;
	public string questlogdataPath = null;
	public string achievementdataPath = null;
	public string gameleveldataPath = null;
	public string playerdataPath = null;
	public string predefinedInventoryPath = null;
	public string questmanagerPath = null;
	public string dialoginterfacePath = null;

	public string backupFolderName = "backup";
	public string gamedataFolderName = "gamedata";
	public string itemDataFileName = "itemdatas";
	public string craftDataFileName = "craftdatas";
	public string dialogDataFileName = "dialogdatas";
	public string vendorDataFileName = "vendordatas";
	public string questlogDataFileName = "questlogdatas";
	public string achievementdataFileName = "achievementdatas";
	public string gameleveldataFileName = "gameleveldatas";
	public string playerSaveFileName = "checkpoint_playersave";
	public string predefinedInventorySaveFileName = "predefined_inventory";
	public string questmanagerSaveFileName = "questmanagerdatas";
	public string dialoginterfaceSaveFileName = "dialoginterfacedatas";

	public enum UpdateFileMode
	{
		UPDATE_REPLACE,
		UPDATE_APPEND,
	}

	public static FileManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("File Manager").AddComponent<FileManager>();
				DontDestroyOnLoad(instance);
			}
			return instance;
		}

	}
	public void Initialize()
	{
		Initialize(false);//dont allow reinitalize of this class by default
	}
	public void Initialize(bool re_init)
	{
		if(initedBefore == false || re_init == true)
		{
			//print ("file manager init");
			path = Application.dataPath; //get path to the game data directory (the assert folder)
			//print ("path " + path);
			gamedataPath = path + "/" + gamedataFolderName;
			backupPath = path + "/" + gamedataFolderName + "/" + backupFolderName;
			
			itemdataPath = gamedataPath + "/" + itemDataFileName + ".xml";
			craftdataPath = gamedataPath + "/" + craftDataFileName + ".xml";
			dialogdataPath = gamedataPath + "/" + dialogDataFileName + ".xml";
			vendordataPath = gamedataPath + "/" + vendorDataFileName + ".xml";
			questlogdataPath = gamedataPath + "/" + questlogDataFileName + ".xml";
			gameleveldataPath = gamedataPath + "/" + gameleveldataFileName + ".xml";
			achievementdataPath = gamedataPath + "/" + backupFolderName + "/" +"backup_"+ achievementdataFileName + ".xml";
			playerdataPath = gamedataPath + "/" + backupFolderName +"/"+ "backup_"+playerSaveFileName + ".xml";
			predefinedInventoryPath = gamedataPath + "/" + predefinedInventorySaveFileName + ".xml";
			questmanagerPath = gamedataPath + "/" + backupFolderName +"/"+ "backup_" + questmanagerSaveFileName + ".xml";
			dialoginterfacePath = gamedataPath + "/"+ backupFolderName +"/"+ "backup_"  + dialoginterfaceSaveFileName + ".xml";
			
			if (CheckDirectory(gamedataFolderName) == false)
			{
				if(CreateDirectory(gamedataFolderName) == true)
				{
					CreateDirectory(gamedataFolderName,"dump" );
					CreateDirectory(gamedataFolderName,"backup" );
				}
			}
			initedBefore = true;
		}
	}
	public void OnApplicationQuit()//this will be auto called like start and update function
	{
		print ("file manager get quited");
		DestroyInstance();
	}
	public void DestroyInstance()
	{
		//print ("file manager instance destroyed");
		instance = null;
	}
	public string GetGameDataPath()
	{
		return gamedataPath;
	}
	public XmlNodeList DigToDesiredChildNodeList(XmlNodeList startingList,string desiredParentName)
	{
		XmlNodeList checknodelist = startingList;
		XmlNode checknode = checknodelist.Item(0);
		
		while ( checknodelist.Count >= 1)
		{
			checknodelist = checknode.ChildNodes;
			checknode = checknodelist.Item(0);
			if(checknode.Name == desiredParentName)
			{
				return checknode.ChildNodes;
			}
		}
		
		return null;
	}
	public XmlNodeList DigToDesiredParentNodeList(XmlNodeList startingList,string desiredParentName)
	{
		XmlNodeList checknodelist = startingList;
		XmlNode checknode = checknodelist.Item(0);
		
		while ( checknodelist.Count >= 1)
		{
			checknodelist = checknode.ChildNodes;
			checknode = checknodelist.Item(0);
			if(checknode.Name == desiredParentName)
			{
				return checknode.ParentNode.ChildNodes;
			}
		}

		return null;
	}
	public void LoadDialogInterface()
	{
		DialogInterface targetobj = DialogInterface.instance;
		if(targetobj != null)
		{
			if( CheckFile(dialoginterfacePath,false) == false)
			{
				print ("ERROR: dialog interface data file cannot be found, load data aborted");
				return ;
				
			}
			print ("loading dialog interface datas");
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(dialoginterfacePath);
			
			XmlNodeList parentList = null;
			
			parentList = xmlDoc.GetElementsByTagName("database");
			parentList = DigToDesiredChildNodeList(parentList,"dialoginterface");
			int dialognodeIndex = -1;

			foreach(XmlNode childInfo in parentList)
			{
				//print ("loading achievement data , child info name: " + childInfo.Name);
				switch(childInfo.Name)
				{
				case "currentdialognode":
					dialognodeIndex = int.Parse(childInfo.Attributes["id"].Value);
					if(dialognodeIndex < 0)
					{
						targetobj.CurrentDialogNode = null;
					}else
					{
						targetobj.CurrentDialogNode = targetobj.dialogTree[dialognodeIndex];
						targetobj.StartNewDialogSession(targetobj.CurrentDialogNode );
					}

					break;
				default:
					Debug.Log("ERROR: Unhandled LoadDialogInterface childInfo field name: " + childInfo.Name);
					break;
				}
			}
		}
		else
		{
			Debug.Log("LoadDialogInterface has null targetobj ref");
		}
	}
	public void SaveDialogInterface()
	{
		DialogInterface targetobj = DialogInterface.instance;
		if(targetobj != null)
		{
			my_XmlEntry rootentry = new my_XmlEntry("database",null);
			
			List<my_XmlBuildEntry> inputlist = new List<my_XmlBuildEntry>();
			List<string> attkey = new List<string>();
			List<string> attvalue = new List<string>();
			
			List<string> parentlist = new List<string>();
			List<my_XmlEntry> entrylist = new List<my_XmlEntry>();

			parentlist.Add("database");
			entrylist.Add(new my_XmlEntry("dialoginterface",null,null,null));

			parentlist.Add("dialoginterface");
			attkey.Clear();
			attvalue.Clear();

			if(targetobj.CurrentDialogNode != null)
			{
				Debug.Log("some current dialog node save");
				attkey.Add("id");
				attvalue.Add(targetobj.CurrentDialogNode.nodeId.ToString());
				attkey.Add("text");
				attvalue.Add(targetobj.CurrentDialogNode.text);

			}else
			{
				Debug.Log("null current dialog node save");
				attkey.Add("id");
				attvalue.Add( (-1).ToString());
				attkey.Add("text");
				attvalue.Add("");
			}
			entrylist.Add(new my_XmlEntry("currentdialognode",null,attkey,attvalue));
			
			inputlist = CreateXmlBuildEntryList(parentlist,entrylist);	
			CreateXMLFile("gamedata/" + backupFolderName,"backup_"+ dialoginterfaceSaveFileName,"xml",BuildXMLData(rootentry,inputlist),"plaintext");
		}
		else
		{
			Debug.Log("SaveDialogInterface has null targetobj ref");
		}
	}
	public void LoadQuestManager()
	{
		QuestManager targetobj = QuestManager.instance;
		if(targetobj != null)
		{
			if( CheckFile(questmanagerPath,false) == false)
			{
				print ("ERROR: questmanager data file cannot be found, load data aborted");
				return ;
				
			}
			print ("loading QuestManager datas");
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(questmanagerPath);
			
			XmlNodeList parentList = null;
			
			parentList = xmlDoc.GetElementsByTagName("database");
			parentList = DigToDesiredChildNodeList(parentList,"questmanager");
			foreach(XmlNode childInfo in parentList)
			{
				//print ("loading achievement data , child info name: " + childInfo.Name);
				switch(childInfo.Name)
				{
				case "currentgamelevel":
					//targetobj.currentGameLevel = int.Parse(childInfo.Attributes["index"].Value);
					//no need do anything
					break;
				case "currentquestindex":
					targetobj.currentQuestIndex = int.Parse(childInfo.Attributes["index"].Value);
					targetobj.questLogs.Clear();
					targetobj.FetchCurrentQuest();
					targetobj.questlogdatabase.ResetAllQuestForward(targetobj.currentGameLevel,targetobj.currentQuestIndex);
					break;
				default:
					Debug.Log("ERROR: Unhandled LoadQuestManager childInfo field name: " + childInfo.Name);
					break;
				}
			}

		}
		else
		{
			Debug.Log("LoadQuestManager has null targetobj ref");
		}
	}
	public void SaveQuestManager()
	{
		QuestManager targetobj = QuestManager.instance;
		if(targetobj != null)
		{
			my_XmlEntry rootentry = new my_XmlEntry("database",null);
			
			List<my_XmlBuildEntry> inputlist = new List<my_XmlBuildEntry>();
			List<string> attkey = new List<string>();
			List<string> attvalue = new List<string>();
			
			List<string> parentlist = new List<string>();
			List<my_XmlEntry> entrylist = new List<my_XmlEntry>();

			parentlist.Add("database");
			entrylist.Add(new my_XmlEntry("questmanager",null,null,null));
			
			parentlist.Add("questmanager");
			attkey.Clear();
			attvalue.Clear();
			attkey.Add("index");
			attvalue.Add(targetobj.currentGameLevel.ToString());
			attkey.Add("name");
			attvalue.Add(ApplicationLevelBoard.Instance.gameLevelNameList[targetobj.currentGameLevel]);
			entrylist.Add(new my_XmlEntry("currentgamelevel",null,attkey,attvalue));

			parentlist.Add("questmanager");
			attkey.Clear();
			attvalue.Clear();
			attkey.Add("index");
			attvalue.Add(targetobj.currentQuestIndex.ToString());
			entrylist.Add(new my_XmlEntry("currentquestindex",null,attkey,attvalue));

			inputlist = CreateXmlBuildEntryList(parentlist,entrylist);	
			CreateXMLFile("gamedata/" + backupFolderName,"backup_"+ questmanagerSaveFileName,"xml",BuildXMLData(rootentry,inputlist),"plaintext");
		}
		else
		{
			Debug.Log("SaveQuestManager has null targetobj ref");
		}
	}
	public void SaveQuestDatabase()
	{
		if(QuestLogDatabase.Instance.questLogDatabase.Count == 0)
		{
			print ("QuestLogDatabase list is empty,saving aborted");
			return;
		}

		my_XmlEntry rootentry = new my_XmlEntry("database",null);
		
		List<my_XmlBuildEntry> inputlist = new List<my_XmlBuildEntry>();
		List<string> attkey = new List<string>();
		List<string> attvalue = new List<string>();
		
		List<string> parentlist = new List<string>();
		List<my_XmlEntry> entrylist = new List<my_XmlEntry>();

		foreach (my_QuestLogList loglist in QuestLogDatabase.Instance.questLogDatabase)
		{
			parentlist.Add(rootentry.name);
			entrylist.Add(new my_XmlEntry("level",null,null,null));

			foreach (my_QuestLog log in loglist)
			{
				parentlist.Add("level");
				attkey.Clear();
				attvalue.Clear();
				attkey.Add("id");
				attvalue.Add(log.id.ToString());
				attkey.Add("name");
				attvalue.Add(log.questname);
				entrylist.Add(new my_XmlEntry("quest",null,attkey,attvalue));
			}
		}
		inputlist = CreateXmlBuildEntryList(parentlist,entrylist);	
		CreateXMLFile("gamedata/" + backupFolderName,"backup_"+ questlogDataFileName,"xml",BuildXMLData(rootentry,inputlist),"plaintext");
	}
	public void SaveDialogDatabase()
	{
		if(DialogDatabase.Instance.dialogDatabase.Count == 0)
		{
			print ("DialogDatabase list is empty,saving aborted");
			return;
		}
		
		my_XmlEntry rootentry = new my_XmlEntry("database",null);
		
		List<my_XmlBuildEntry> inputlist = new List<my_XmlBuildEntry>();
		List<string> attkey = new List<string>();
		List<string> attvalue = new List<string>();
		
		List<string> parentlist = new List<string>();
		List<my_XmlEntry> entrylist = new List<my_XmlEntry>();

		foreach (DialogTree a_tree in DialogDatabase.Instance.dialogDatabase)
		{
			//one dialog tree goes into one game level
			parentlist.Add(rootentry.name);
			entrylist.Add(new my_XmlEntry("level",null,null,null));
			parentlist.Add("level");
			attkey.Clear();
			attvalue.Clear();
			attkey.Add("id");
			attvalue.Add(a_tree.id.ToString());
			attkey.Add("gamelevelparent");
			attvalue.Add(a_tree.gameLevelParent);
			entrylist.Add(new my_XmlEntry("dialogtree",null,null,null));

			foreach (my_DialogNode a_node in a_tree)
			{
				parentlist.Add("dialogtree");
				attkey.Clear();
				attvalue.Clear();
				attkey.Add("id");
				attvalue.Add(a_node.nodeId.ToString());
				attkey.Add("actorname");
				attvalue.Add(a_node.actorName);
				entrylist.Add(new my_XmlEntry("dialog",null,attkey,attvalue));

				parentlist.Add("dialog");
				entrylist.Add(new my_XmlEntry("test",a_node.text,null,null));

				for(int i = 0 ; i < a_node.options.Count ; ++i)
				{
					parentlist.Add("dialog");
					attkey.Clear();
					attvalue.Clear();
					attkey.Add("nextdialogid");
					if( a_node.options[i].nextDialog != null)
					{
						attvalue.Add(a_node.options[i].nextDialog.nodeId.ToString());
					}else
					{
						attvalue.Add("");
					}
					attkey.Add("nextdialogid_raw");
					attvalue.Add(a_node.options[i].nextDialogId.ToString());

					attkey.Add("resultid");
					attvalue.Add(a_node.options[i].returnStatus.ToString());
					entrylist.Add(new my_XmlEntry("option",a_node.options[i].text,attkey,attvalue));
				}

			}

		}
			
			
		inputlist = CreateXmlBuildEntryList(parentlist,entrylist);	
		CreateXMLFile("gamedata/" + backupFolderName,"backup_"+ dialogDataFileName,"xml",BuildXMLData(rootentry,inputlist),"plaintext");
	}
	public void SaveCraftDatabase()
	{
		if(ItemDatabase.Instance.craftDatabase.Count == 0)
		{
			print ("Craftdatabase list is empty,saving aborted");
			return;
		}

		my_XmlEntry rootentry = new my_XmlEntry("database",null);
		
		List<my_XmlBuildEntry> inputlist = new List<my_XmlBuildEntry>();
		List<string> attkey = new List<string>();
		List<string> attvalue = new List<string>();
		
		List<string> parentlist = new List<string>();
		List<my_XmlEntry> entrylist = new List<my_XmlEntry>();
		
		
		foreach (CraftingRecipe item in ItemDatabase.Instance.craftDatabase)
		{

			parentlist.Add(rootentry.name);
			attkey.Clear();
			attvalue.Clear();
			attkey.Add("id");
			attvalue.Add(item.id.ToString());
			entrylist.Add(new my_XmlEntry("recipe",null,attkey,attvalue));
			
			parentlist.Add("recipe");
			entrylist.Add(new my_XmlEntry("name",item.recipe_name,null,null));
			parentlist.Add("recipe");
			entrylist.Add(new my_XmlEntry("description",item.description,null,null));
			parentlist.Add("recipe");
			entrylist.Add(new my_XmlEntry("recipeIconName",item.recipeIconName,null,null));

			foreach ( Item_Proxy input in item.ingrediant)
			{
				parentlist.Add("recipe");
				attkey.Clear();
				attvalue.Clear();
				attkey.Add("id");
				attvalue.Add(input.itemid.ToString());
				attkey.Add("name");
				attvalue.Add(input.itemname);
				attkey.Add("amount");
				attvalue.Add(input.amount.ToString());
				entrylist.Add(new my_XmlEntry("ingrediant",null,attkey,attvalue));
			}

			foreach ( Item_Proxy output in item.output)
			{
				parentlist.Add("recipe");
				attkey.Clear();
				attvalue.Clear();
				attkey.Add("id");
				attvalue.Add(output.itemid.ToString());
				attkey.Add("name");
				attvalue.Add(output.itemname);
				attkey.Add("amount");
				attvalue.Add(output.amount.ToString());
				entrylist.Add(new my_XmlEntry("output",null,attkey,attvalue));
			}
			
		}	
		inputlist = CreateXmlBuildEntryList(parentlist,entrylist);	
		CreateXMLFile("gamedata/" + backupFolderName,"backup_"+ craftDataFileName,"xml",BuildXMLData(rootentry,inputlist),"plaintext");
	}
	public void SaveVendorDatabase()
	{
		if(VendorDatabase.Instance.vendorList.Count == 0)
		{
			print ("VendorDatabase list is empty,saving aborted");
			return;
		}

		my_XmlEntry rootentry = new my_XmlEntry("database",null);
		
		List<my_XmlBuildEntry> inputlist = new List<my_XmlBuildEntry>();
		List<string> attkey = new List<string>();
		List<string> attvalue = new List<string>();
		
		List<string> parentlist = new List<string>();
		List<my_XmlEntry> entrylist = new List<my_XmlEntry>();
		
		
		foreach (my_VendorEntry item in VendorDatabase.Instance.vendorList)
		{
			parentlist.Add(rootentry.name);
			attkey.Clear();
			attvalue.Clear();
			attkey.Add("id");
			attvalue.Add(item.id.ToString());
			entrylist.Add(new my_XmlEntry("vendor",null,attkey,attvalue));
			
			parentlist.Add("vendor");
			entrylist.Add(new my_XmlEntry("name",item.vendorName,null,null));

			for( int i = 0 ; i< item.outputItemList.Count; ++i)
			{
				parentlist.Add("vendor");
				attkey.Clear();
				attvalue.Clear();
				attkey.Add("id");
				attvalue.Add(item.outputItemList[i].itemid.ToString());
				attkey.Add("name");
				attvalue.Add(item.outputItemList[i].itemname);
				attkey.Add("amount");
				attvalue.Add(item.outputItemList[i].amount.ToString());
				entrylist.Add(new my_XmlEntry("output",null,attkey,attvalue));
			}
		}	
		inputlist = CreateXmlBuildEntryList(parentlist,entrylist);	
		CreateXMLFile("gamedata/" + backupFolderName,"backup_"+ vendorDataFileName,"xml",BuildXMLData(rootentry,inputlist),"plaintext");
	}
	public void SaveItemDatabase()
	{
		if(ItemDatabase.Instance.itemDatabase.Count == 0)
		{
			print ("ItemDatabase list is empty,saving aborted");
			return;
		}

		my_XmlEntry rootentry = new my_XmlEntry("database",null);
		
		List<my_XmlBuildEntry> inputlist = new List<my_XmlBuildEntry>();
		List<string> attkey = new List<string>();
		List<string> attvalue = new List<string>();
		
		List<string> parentlist = new List<string>();
		List<my_XmlEntry> entrylist = new List<my_XmlEntry>();
		
		
		foreach (Item item in ItemDatabase.Instance.itemDatabase)
		{
			parentlist.Add(rootentry.name);
			attkey.Clear();
			attvalue.Clear();
			attkey.Add("id");
			attvalue.Add(item.id.ToString());
			entrylist.Add(new my_XmlEntry("item",null,attkey,attvalue));
			
			parentlist.Add("item");
			entrylist.Add(new my_XmlEntry("name",item.itemname,null,null));
			parentlist.Add("item");
			entrylist.Add(new my_XmlEntry("type",item.GetItemType(),null,null));
			parentlist.Add("item");
			entrylist.Add(new my_XmlEntry("description",item.description,null,null));
			parentlist.Add("item");
			entrylist.Add(new my_XmlEntry("weight",item.weight.ToString(),null,null));
			parentlist.Add("item");
			entrylist.Add(new my_XmlEntry("stackable",item.stackable.ToString(),null,null));
			
		}	
		inputlist = CreateXmlBuildEntryList(parentlist,entrylist);	
		CreateXMLFile("gamedata/" + backupFolderName,"backup_"+ itemDataFileName,"xml",BuildXMLData(rootentry,inputlist),"plaintext");
	}

	public void SaveAchievementData()
	{
		AchievementManager targetobj =  AchievementManager.Instance;
		if(targetobj.collectibleAchievedList.Count == 0)
		{
			print ("collectible Achieved List is empty,saving aborted");
			return;
		}
		
		my_XmlEntry rootentry = new my_XmlEntry("database",null);
		
		List<my_XmlBuildEntry> inputlist = new List<my_XmlBuildEntry>();
		List<string> attkey = new List<string>();
		List<string> attvalue = new List<string>();
		
		List<string> parentlist = new List<string>();
		List<my_XmlEntry> entrylist = new List<my_XmlEntry>();
		
		//each list represent one level.
		foreach (my_CollectibleList item in targetobj.collectibleAchievedList)
		{
			parentlist.Add(rootentry.name);
			attkey.Clear();
			attvalue.Clear();
			attkey.Add("id");
			attvalue.Add(item.id.ToString());
			attkey.Add("name");
			attvalue.Add(item.listName);
			entrylist.Add(new my_XmlEntry("level",null,attkey,attvalue));
			
			parentlist.Add("level");
			entrylist.Add(new my_XmlEntry("collectibles",null,null,null));

			foreach(my_Collectible a_collectible in item)
			{
				parentlist.Add("collectibles");
				attkey.Clear();
				attvalue.Clear();
				attkey.Add("id");
				attvalue.Add(a_collectible.id.ToString());
				attkey.Add("name");
				attvalue.Add(a_collectible.collectibleName);
				entrylist.Add(new my_XmlEntry("collectible",null,attkey,attvalue));
			}

		}	
		inputlist = CreateXmlBuildEntryList(parentlist,entrylist);	
		CreateXMLFile("gamedata/" + backupFolderName,"backup_"+ achievementdataFileName,"xml",BuildXMLData(rootentry,inputlist),"plaintext");

	}
	
	public void LoadAchievementData()
	{
		if( CheckFile(achievementdataPath,false) == false)
		{
			print ("ERROR: acheivement data file cannot be found, load data aborted");
			return ;
			
		}
		print ("loading acheivement datas");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(achievementdataPath);
		
		XmlNodeList parentList = null;
		
		parentList = xmlDoc.GetElementsByTagName("database");
		parentList = DigToDesiredParentNodeList(parentList,"level");
		AchievementManager targetobj =  AchievementManager.Instance;
		
		foreach(XmlNode childInfo in parentList)
		{
			//print ("loading achievement data , child info name: " + childInfo.Name);
			switch(childInfo.Name)
			{

			case "level":

				string levelname = childInfo.Attributes["name"].Value;
				int levelId = int.Parse(childInfo.Attributes["id"].Value);
				XmlNodeList childContent = childInfo.ChildNodes;
				targetobj.AddCollectibleLevel(new my_CollectibleList(levelname));
				foreach (XmlNode childItem in childContent)
				{
					//print ("loading achievement data , child item name: " + childItem.Name);
					switch(childItem.Name)
					{
					case "collectibles":
						XmlNodeList innerchildContent = childItem.ChildNodes;
						foreach (XmlNode innerchildItem in innerchildContent)
						{
							//print ("loading achievement data , inner child item name: " + innerchildItem.Name);
							switch(innerchildItem.Name)
							{
							case "collectible":
								targetobj.AddCollectible(levelId,new my_Collectible(innerchildItem.Attributes["name"].Value));
								//targetobj.AddCollectibleLevel(levelname,int.Parse(innerchildItem.InnerText));	
								break;
							default:
								print("ERROR: Unknown load achievement inner childItem data field detected: " + innerchildItem.Name);
								break;
							}
						}
						break;
					default:
						print("ERROR: Unknown load achievement childItem data field detected: " + childItem.Name);
						break;
					}
				}
				break;
				
			default:
				print("ERROR: Unknown load achievement childinfo data field detected: " + childInfo.Name);
				break;
			}
		}
	}

	public void SaveGameLevelInformation()
	{
		ApplicationLevelBoard targetobj =  ApplicationLevelBoard.Instance;

		if(targetobj.gameLevelNameList.Count == 0)
		{
			print ("gameLevelName List  is empty,saving aborted");
			return;
		}
		
		my_XmlEntry rootentry = new my_XmlEntry("database",null);
		
		List<my_XmlBuildEntry> inputlist = new List<my_XmlBuildEntry>();
		List<string> attkey = new List<string>();
		List<string> attvalue = new List<string>();
		
		List<string> parentlist = new List<string>();
		List<my_XmlEntry> entrylist = new List<my_XmlEntry>();
		
		parentlist.Add(rootentry.name);
		entrylist.Add(new my_XmlEntry("maxlevelcount",targetobj.maxLevelCount.ToString(),null,null));
		foreach (string item in targetobj.gameLevelNameList)
		{
			parentlist.Add(rootentry.name);
			attkey.Clear();
			attvalue.Clear();
			attkey.Add("name");
			attvalue.Add(item);
			entrylist.Add(new my_XmlEntry("level",null,attkey,attvalue));
			
		}	
		inputlist = CreateXmlBuildEntryList(parentlist,entrylist);	
		CreateXMLFile("gamedata/" + backupFolderName,"backup_"+ gameleveldataFileName,"xml",BuildXMLData(rootentry,inputlist),"plaintext");
	}

	public void LoadGameLevelInformation()
	{
		if( CheckFile(gameleveldataPath,false) == false)
		{
			print ("ERROR: game level data file cannot be found, load data aborted");
			return;
			
		}
		print ("loading game level datas");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(gameleveldataPath);
		
		XmlNodeList parentList = null;

		parentList = xmlDoc.GetElementsByTagName("database");
		//parentList = DigToDesiredChildNodeList(parentList,"database");
		parentList = parentList.Item(0).ChildNodes;
		ApplicationLevelBoard targetobj =  ApplicationLevelBoard.Instance;

		foreach(XmlNode childInfo in parentList)
		{
			switch(childInfo.Name)
			{

			case "maxlevelcount":
				targetobj.maxLevelCount = int.Parse(childInfo.InnerText);
				break;

			case "level":
				targetobj.AddGameLevelNameList(childInfo.Attributes["name"].Value);

				break;

			default:
				print("ERROR: Unknown load game level childInfo data field detected: " + childInfo.Name);
				break;
			}
		}
	}

	public List<my_QuestLogList> LoadQuestLogData()
	{
		List<my_QuestLogList> resultlist = new List<my_QuestLogList>();

		if( CheckFile(questlogdataPath,false) == false)
		{
			print ("ERROR: Quest log data file cannot be found, load data aborted");
			return null;
			
		}
		print ("loading Quest log datas");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(questlogdataPath);

		XmlNodeList parentList = null;
		
		parentList = xmlDoc.GetElementsByTagName("database");
		parentList = DigToDesiredParentNodeList(parentList,"level");
		my_QuestLogList tempquestloglist = null;
		my_QuestLog tempquestlog = null;

		foreach(XmlNode childInfo in parentList)
		{
			//print ("looking at childinfo name:"+ childInfo.Name );
			tempquestloglist = new my_QuestLogList();

			XmlNodeList childContent = childInfo.ChildNodes;
			
			foreach (XmlNode childItem in childContent)
			{
				tempquestlog = new my_QuestLog();

				switch (childItem.Name)
				{
				case "quest":
					tempquestlog.questname = childItem.Attributes["name"].Value;
					tempquestlog.id = int.Parse(childItem.Attributes["id"].Value);
					break;
				default:
					print("ERROR: Unknown load questlog data field detected: " + childItem.Name);
					break;
				}

				tempquestloglist.questlogs.Add(tempquestlog);
			}

			resultlist.Add(tempquestloglist);
		}
		
		return resultlist;
	}

	public List<DialogTree> LoadDialogTreeData()
	{
		List<DialogTree> resultlist = new List<DialogTree>();
		DialogTree tempentry =null;
		my_DialogNode tempdialog =null;
		my_DialogOption tempdialogoption =null;

		if( CheckFile(dialogdataPath,false) == false)
		{
			print ("ERROR: Dialog tree data file cannot be found, load data aborted");
			return null;
			
		}
		print ("loading Dialog Tree datas");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(dialogdataPath);
		
		
		XmlNodeList parentList = null;

		parentList = xmlDoc.GetElementsByTagName("database");
		parentList = DigToDesiredParentNodeList(parentList,"level");

		foreach(XmlNode childInfo in parentList)
		{
			//XmlNodeList childContent = DigToDesiredChildNodeList(parentList,"dialogtree");
			//print ("looking at LoadDialogTreeData childinfo name:"+ childInfo.Name );
			tempentry = new DialogTree();
			//XmlNode dialogtreenode = childContent.Item(0).ParentNode;


			foreach (XmlNode childItem in childInfo)
			{
				//print ("looking at LoadDialogTreeData childItem name: "+ childItem.Name );
				tempentry.id = int.Parse(childItem.Attributes["id"].Value);
				tempentry.gameLevelParent = childItem.Attributes["gamelevelparent"].Value;

				foreach (XmlNode BigInnerchildItem in childItem.ChildNodes)
				{
					//print ("looking at LoadDialogTreeData BigInnerchildItem name: "+ BigInnerchildItem.Name );
					tempdialog = new my_DialogNode();
					tempdialog.actorName = BigInnerchildItem.Attributes["actorname"].Value;
					tempdialog.nodeId = int.Parse(BigInnerchildItem.Attributes["id"].Value);
					if(BigInnerchildItem.Attributes["newdialogsection"] != null)
					{
						switch (BigInnerchildItem.Attributes["newdialogsection"].Value)
						{
							case "true":
							case "True":
								tempentry.AddDialogBookmark(tempdialog);
								break;
						}

					}
					foreach (XmlNode innerchildItem in BigInnerchildItem.ChildNodes)
					{
						//print ("looking at LoadDialogTreeData innerchildItem name: "+ innerchildItem.Name );
						//print ("looking at LoadDialogTreeData innerchildItem text: "+ innerchildItem.InnerText );
						switch(innerchildItem.Name)
						{
						case "text":
						case "Text":
						case "texts":
						case "Texts":
							tempdialog.text = innerchildItem.InnerText;
							break;

						case "Option":
						case "option":
						case "Options":
						case "options":
							tempdialogoption = new my_DialogOption();
							tempdialogoption.text = innerchildItem.InnerText;
							string tempstring = innerchildItem.Attributes["nextdialogid"].Value;
							if( tempstring == "" || tempstring == " ")
							{
								tempdialogoption.nextDialogId = -1;
							}else
							{
								tempdialogoption.nextDialogId = int.Parse(innerchildItem.Attributes["nextdialogid"].Value);
							}

							tempdialogoption.returnStatus = int.Parse(innerchildItem.Attributes["resultid"].Value);
							tempdialog.AddDialogOption(tempdialogoption);
							break;

						default:
							print("ERROR: Unknown load dialog tree data field detected: " + innerchildItem.Name);
							break;
						}
					}
					//tempentry.dialogtree.AddDialog(tempdialog);
					tempentry.AddDialog(tempdialog);
				}
			}
			resultlist.Add(tempentry);
		}
		return resultlist;
	}
	public List<my_VendorEntry> LoadVendorsData()
	{

		List<my_VendorEntry> resultlist = new List<my_VendorEntry>();

		my_VendorEntry tempitem = new my_VendorEntry();
		Item_Proxy tempitemproxy = new Item_Proxy();

		if( CheckFile(vendordataPath,false) == false)
		{
			print ("ERROR: Vendor data file cannot be found, load data aborted");
			return null;
			
		}
		print ("loading vendor datas");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(vendordataPath);
		
		
		XmlNodeList parentList = null;

		parentList = xmlDoc.GetElementsByTagName("database");
		parentList = DigToDesiredParentNodeList(parentList,"vendor");

		int inputid = -1;
		int inputamount = -1;
		string inputname = "";
		
		foreach(XmlNode childInfo in parentList)
		{
			//print ("looking at child info "+ childInfo.Name );
			tempitem.id = int.Parse(childInfo.Attributes["id"].Value);
			XmlNodeList childContent = childInfo.ChildNodes;
			
			foreach (XmlNode childItem in childContent)
			{
				//print ("looking at child item "+ childItem.Name );
				switch(childItem.Name)
				{
					
				case "Name":
				case "name":
					tempitem.vendorName = childItem.InnerText;
					break;
					
				case "Output":
				case "output":
					inputid = int.Parse(childItem.Attributes["id"].Value);
					inputname = childItem.Attributes["name"].Value;
					inputamount  = int.Parse(childItem.Attributes["amount"].Value);
					tempitem.AddOutputItem(inputid,inputname,inputamount);
					break;

				default:
					print("ERROR: Unknown load vendor data field detected: " + childItem.Name);
					break;
				}
			}
			resultlist.Add(new my_VendorEntry(tempitem));
		}
		return resultlist;

	}
	public List<CraftingRecipe> LoadCraftsData()
	{
		if( CheckFile(craftdataPath,false) == false)
		{
			print ("ERROR: Craft data file cannot be found, load data aborted");
			return null;
			
		}
		print ("loading craft datas");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(craftdataPath);
		
		
		XmlNodeList parentList = null;
		//XmlNodeList innerchildContent = null;
		//XmlNode innerchildItem = null;
		List<CraftingRecipe> resultlist = new List<CraftingRecipe>();
		CraftingRecipe tempitem = new CraftingRecipe();
		
		parentList = xmlDoc.GetElementsByTagName("database");
		parentList = DigToDesiredParentNodeList(parentList,"recipe");
		int n = -1;
		bool isNumeric = false;
		foreach(XmlNode childInfo in parentList)
		{
			//print ("looking at child INFO "+ childInfo.Name );
			tempitem.ClearAll();

			tempitem.id = int.Parse(childInfo.Attributes["id"].Value);
			XmlNodeList childContent = childInfo.ChildNodes;
			
			foreach (XmlNode childItem in childContent)
			{
				//print ("looking at child ITEM "+ childItem.Name + " : " + childItem.InnerText );

				switch (childItem.Name)
				{
				case "Name":
				case "name":
					tempitem.recipe_name = childItem.InnerText;
					break;

				case "recipeIconName":
				case "recipeiconname":
					tempitem.RecipeIconName = childItem.InnerText;

					break;

				case "Descriptions":
				case "descriptions":
				case "Description":
				case "description":
					tempitem.description = childItem.InnerText;
					break;

				case "Ingrediants":
				case "ingrediants":
				case "Ingrediant":
				case "ingrediant":
					isNumeric = int.TryParse(childItem.Attributes["id"].Value, out n);
					if(isNumeric == true)
					{
						tempitem.AddIngrediant(n,ItemDatabase.Instance.GetItem(n).itemname,int.Parse(childItem.Attributes["amount"].Value));
					}else
					{
						tempitem.AddIngrediant(ItemDatabase.Instance.GetItem(childItem.Attributes["id"].Value));
					}
					break;

				case "Outputs":
				case "outputs":
				case "Output":
				case "output":
					isNumeric = int.TryParse(childItem.Attributes["id"].Value, out n);
					if(isNumeric == true)
					{
						tempitem.AddOutputItem(n,ItemDatabase.Instance.GetItem(n).itemname,int.Parse(childItem.Attributes["amount"].Value));
					}else
					{
						tempitem.AddOutputItem(ItemDatabase.Instance.GetItem(childItem.Attributes["id"].Value));
					}

					break;

				default:
					print("ERROR: Unknown load item data field detected: " + childItem.Name);
					break;
				}
			}

			resultlist.Add(new CraftingRecipe(tempitem));

		}

		return resultlist;
	}
	public List<Item> LoadItemsData()
	{
		//print( itemdataPath.Remove( 0,(path+"/").Length)  );

		if( CheckFile(itemdataPath,false) == false)
		{
			print ("ERROR: Item data file cannot be found, load data aborted");
			return null;

		}
		print ("loading item datas");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(itemdataPath);


		XmlNodeList parentList = null;

		List<Item> resultlist = new List<Item>();
		Item tempitem = new Item();

		parentList = xmlDoc.GetElementsByTagName("database");
		parentList = DigToDesiredParentNodeList(parentList,"item");
			
		foreach(XmlNode childInfo in parentList)
		{
			//print ("looking at child info "+ childInfo.Name );
			tempitem.id = int.Parse(childInfo.Attributes["id"].Value);
			XmlNodeList childContent = childInfo.ChildNodes;

			foreach (XmlNode childItem in childContent)
			{

			
				switch(childItem.Name)
				{

				case "Name":
				case "name":
					tempitem.itemname = childItem.InnerText;
					break;

				case "Type":
				case "type":
					tempitem.SetItemType(childItem.InnerText);
					break;

				case "Description":
				case "description":
					tempitem.description = childItem.InnerText;
					break;

				case "Weight":
				case "weight":
					tempitem.weight = int.Parse(childItem.InnerText);
					break;

				case "Stackable":
				case "stackable":
					switch (childItem.InnerText)
					{
					case "True":
					case "true":
						tempitem.stackable = true;
						break;

					case"False":
					case"false":
						default:
						tempitem.stackable = false;
						break;
					}
					break;
				default:
					print("ERROR: Unknown load item data field detected: " + childItem.Name);
					break;
				}
			}
			resultlist.Add(new Item(tempitem));
		}
		return resultlist;
	}

	public void LoadPreDefinedInventory(int levelIndex)
	{
		if( CheckFile(predefinedInventoryPath,false) == false)
		{
			print ("ERROR: predefined Inventory save data cannot be found, load data aborted");
			print ("LOG:"+ predefinedInventoryPath);
			return ;
			
		}
		print ("loading  predefined Inventory save data");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(predefinedInventoryPath);
		
		XmlNodeList parentList = null;
		parentList = xmlDoc.GetElementsByTagName("database");
		parentList = DigToDesiredChildNodeList(parentList,"inventory");

		Inventory playerinventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();


		bool isNumeric = false;
		int n = -1;
		foreach (XmlNode levelsection in parentList)
		{
			isNumeric = int.TryParse(levelsection.Attributes["id"].Value, out n);
			if(isNumeric == true)
			{
				if(levelIndex != n)
				{
					continue;
				}
			}else
			{
				if(Application.loadedLevelName != levelsection.Attributes["id"].Value)
				{
					continue;
				}
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
						print ("load predefined inventory add item amount: " + amountToAdd);
						playerinventory.AddItem(n,amountToAdd);
					}else
					{
						print ("load predefined inventory add item amount: " + amountToAdd);
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
	public void LoadPreDefinedInventory(string levelName)
	{
		if(CheckFile(predefinedInventoryPath,false) == false)
		{
			print ("ERROR: predefined Inventory save data cannot be found, load data aborted");
			print ("LOG: "+ predefinedInventoryPath);
			return ;
			
		}
		print ("loading  predefined Inventory save data");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(predefinedInventoryPath);
		
		XmlNodeList parentList = null;
		parentList = xmlDoc.GetElementsByTagName("database");
		parentList = DigToDesiredChildNodeList(parentList,"inventory");

		Inventory playerinventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
		
		bool isNumeric = false;
		int n = -1;
		foreach (XmlNode levelsection in parentList)
		{
			isNumeric = int.TryParse(levelsection.Attributes["id"].Value, out n);
			if(isNumeric == true)
			{
				if(Application.loadedLevel != n)
				{
					continue;
				}
			}else
			{
				if(levelName != levelsection.Attributes["id"].Value)
				{
					continue;
				}
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
						//print ("load predefined inventory add item amount: " + amountToAdd);
						playerinventory.AddItem(n,amountToAdd);
					}else
					{
						//print ("load predefined inventory add item amount: " + amountToAdd);
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
	public string GetLastCheckPointScene()
	{
		Debug.Log("player data path: " + playerdataPath);
		if(CheckFile(playerdataPath,false) == false)
		{
			print ("ERROR: player checkpoint save data cannot be found, load data aborted");
			return "";
			
		}
		print ("loading  player checkpoint save data");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(playerdataPath);
		
		XmlNodeList parentList = null;
		parentList = xmlDoc.GetElementsByTagName("database");
		parentList = DigToDesiredChildNodeList(parentList,"player");
		
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
						
					case "currentlevel_unity":
					{
						return sectionitem.Attributes["name"].Value;
					}
						
						
					default:
						print ("ERROR: Unhandled level section item field detected: " + sectionitem.Name);
						break;
					}
				}
				break;
			}
		}
		
		return "";
	}
	public void LoadPlayerInfo()
	{
		LoadPlayerInfo(false);
	}
	public void LoadPlayerInfo(bool loadCheckPointLevel)
	{
		
		if( CheckFile(playerdataPath,false) == false)
		{
			print ("ERROR: player checkpoint save data cannot be found, load data aborted");
			print ("LOG:"+ playerdataPath);
			return ;
			
		}
		print ("loading  player checkpoint save data");
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(playerdataPath);
		
		XmlNodeList parentList = null;
		parentList = xmlDoc.GetElementsByTagName("database");
		parentList = DigToDesiredChildNodeList(parentList,"player");
		
		Item tempitem = null;

		GameObject playerobj = GameObject.FindGameObjectWithTag("Player");
		PlayerInfo playerinfo = playerobj.GetComponent<PlayerInfo>();
		Inventory playerinventory = playerobj.GetComponent<Inventory>();
		CastSlot playercastslot = playerobj.GetComponent<CastSlot>();

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
						
					case "currentlevel_unity":
						if(loadCheckPointLevel == true)
						{
							Application.LoadLevel( sectionitem.Attributes["name"].Value );
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
				//int checkpointindex = int.Parse(playerinfosection.Attributes["index"].Value);
				//playerobj.transform.position = checkPointList[checkpointindex].transform.position;
				float playerX = float.Parse(playerinfosection.Attributes["x"].Value);
				float playerY = float.Parse(playerinfosection.Attributes["y"].Value);
				float playerZ = float.Parse(playerinfosection.Attributes["z"].Value);
				playerobj.transform.position = new Vector3(playerX,playerY,playerZ);
				playerobj.transform.rotation = Quaternion.identity;
				Camera.main.transform.rotation = Quaternion.identity;
				break;
				
			case "Inventory":
			case "inventory":
				if(playerinventory != null)
				{
					playerinventory.currentweight = int.Parse(playerinfosection.Attributes["weight"].Value);
					
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
							tempitem.weight = int.Parse(sectionitem.Attributes["weight"].Value);
							tempitem.itemindexinlist = int.Parse(sectionitem.Attributes["index"].Value);
							tempitem.description = sectionitem.Attributes["description"].Value;
							tempitem.SetItemType(sectionitem.Attributes["type"].Value);
							tempitem.SetStackable(sectionitem.Attributes["stackable"].Value);
							tempitem.SelfReloadIcon();
							//Debug.Log("trying add item :" + tempitem.ItemName);
							//Debug.Log("trying add item index :" + tempitem.itemindexinlist);
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
		LoadDialogInterface();
		LoadQuestManager();
		
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
		attkey.Clear();
		attvalue.Clear();
		attkey.Add("name");
		attvalue.Add(Application.loadedLevelName);
		attkey.Add("number");
		attvalue.Add(Application.loadedLevel.ToString());
		entrylist.Add(new my_XmlEntry("currentlevel_unity",null,attkey,attvalue));

		GameObject playerobj = GameObject.FindGameObjectWithTag("Player");
		PlayerInfo playerinfo = playerobj.GetComponent<PlayerInfo>();
		Inventory playerinventory = playerobj.GetComponent<Inventory>();
		CastSlot playercastslot = playerobj.GetComponent<CastSlot>();
		
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

		List<CheckPoint>checkPointList = LevelManager.Instance.checkPointList;
		int currentCheckPointIndex = LevelManager.Instance.currentCheckPointIndex;
		
		if(checkPointList.Count > 0 && currentCheckPointIndex >= 0 && currentCheckPointIndex < checkPointList.Count)
		{
			parentlist.Add("player");
			attkey.Clear();
			attvalue.Clear();
			attkey.Add("id");
			Debug.Log ("suspect value index check: " + currentCheckPointIndex);
			Debug.Log ("suspect obj check: " + checkPointList[currentCheckPointIndex]);
			attvalue.Add(checkPointList[currentCheckPointIndex].id.ToString());
			attkey.Add("name");
			attvalue.Add(checkPointList[currentCheckPointIndex].name);
			attkey.Add("index");
			attvalue.Add(currentCheckPointIndex.ToString());
			attkey.Add("x");
			attvalue.Add(checkPointList[currentCheckPointIndex].transform.position.x.ToString());	
			attkey.Add("y");
			attvalue.Add(checkPointList[currentCheckPointIndex].transform.position.y.ToString());	
			attkey.Add("z");
			attvalue.Add(checkPointList[currentCheckPointIndex].transform.position.z.ToString());	
			attkey.Add("rx");
			attvalue.Add(checkPointList[currentCheckPointIndex].transform.rotation.x.ToString());	
			attkey.Add("ry");
			attvalue.Add(checkPointList[currentCheckPointIndex].transform.rotation.y.ToString());		
			attkey.Add("rz");
			attvalue.Add(checkPointList[currentCheckPointIndex].transform.rotation.z.ToString());	
			attkey.Add("rw");
			attvalue.Add(checkPointList[currentCheckPointIndex].transform.rotation.w.ToString());
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
				attkey.Clear();
				attvalue.Clear();
				attkey.Add("weight");
				attvalue.Add(playerinventory.currentweight.ToString());
				entrylist.Add(new my_XmlEntry("inventory",null,attkey,attvalue));
				
				
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
					attkey.Add("weight");
					attvalue.Add(item.weight.ToString());
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
		
		
		inputlist = CreateXmlBuildEntryList(parentlist,entrylist);	
		CreateXMLFile("gamedata/" + backupFolderName,"backup_"+ playerSaveFileName,"xml",BuildXMLData(rootentry,inputlist),"plaintext");

		SaveDialogInterface();
		SaveQuestManager();
		
	}

	public List<my_XmlBuildEntry> CreateXmlBuildEntryList(List<string>parentNameList,  List<my_XmlEntry> xmlEntryList)
	{
		List<my_XmlBuildEntry> entryList = new List<my_XmlBuildEntry>();
		
		for(int i = 0 ; i<xmlEntryList.Count;++i)
		{
			entryList.Add( CreateBuildEntry(parentNameList[i],xmlEntryList[i]));
		}
		return entryList;
	}

	//found on stackoverflow//to indent the xml file
	private string IndentXML(string xml)
	{
		var doc = new XmlDocument();
		doc.LoadXml(xml);
		
		var settings = new XmlWriterSettings
		{
			Indent = true,
			IndentChars = "\t",
			NewLineChars = Environment.NewLine,
			NewLineHandling = NewLineHandling.Replace,
			Encoding = new UTF8Encoding(false)
		};
		
		using (var ms = new MemoryStream())
			using (var writer = XmlWriter.Create(ms, settings))
		{
			doc.Save(writer);
			var xmlString = Encoding.UTF8.GetString(ms.ToArray());
			return xmlString;
		}
	}

	//build a xml file
	public string BuildXMLData(my_XmlEntry rootEntry, List<my_XmlBuildEntry> buildEntryList)
	{
		XmlDocument xml = new XmlDocument();

		XmlElement rootElement = CreateXMLElement(xml,rootEntry);
		xml.AppendChild(rootElement);//insert the root node

		//varable preparation
		XmlElement a_element = null;
		XmlNodeList a_parentlist = null;

		for(int i = 0 ; i < buildEntryList.Count;++i)
		{
			//xml.GetElementsByTagName(buildEntryList[i].parentname);

			//find the parent node
			a_parentlist = xml.GetElementsByTagName(buildEntryList[i].parentname);


			if(a_parentlist != null && buildEntryList[i].parentname != null && buildEntryList[i].parentname != "")//append to existing parent
			{

				//prepare the xmlelement
				a_element = CreateXMLElement(xml,buildEntryList[i].entry);


				for(int i2 = 0 ; i2< a_parentlist.Count; ++i2)
				{
					//append to the parent node
					a_parentlist[i2].AppendChild(a_element);
				}

			}else
			{
				//if parent is not found, create new one.

				a_element = CreateXMLElement(xml,buildEntryList[i].entry);
				rootElement.AppendChild(a_element);
			}

		}

		
		
		return IndentXML(xml.OuterXml);
	}

	//return my_XmlBuildEntry assigned with the value passed in
	private my_XmlBuildEntry CreateBuildEntry(string parentname, string entryname,string entryvalue)
	{
		return new my_XmlBuildEntry(parentname ,new my_XmlEntry(entryname,entryvalue));
	}
	private my_XmlBuildEntry CreateBuildEntry(string parentname, my_XmlEntry a_entry)
	{
		return new my_XmlBuildEntry(parentname ,a_entry);
	}

	//return xmlElement assigned with the value passed in
	private XmlElement CreateXMLElement(XmlDocument xmlobject, my_XmlEntry a_entry)
	{
		XmlElement element= xmlobject.CreateElement(a_entry.name);

		int index = 0;
		
		if(a_entry.innerValue != null)
		{
			element.InnerText = a_entry.innerValue;
		}
		
		if(a_entry.attributeKeys != null)
		{
			foreach(string attribute in a_entry.attributeKeys)
			{
				element.SetAttribute(attribute,a_entry.attributeValues[index]);
				++index;
			}
		}
		
		return element;
	}

	//return xmlElement assigned with the value passed in
	private XmlElement CreateXMLElement(XmlDocument xmlobject, string elementName, string innerValue, string[] attributeList, string[] attributeValues)
	{
		XmlElement element= xmlobject.CreateElement(elementName);

		int index = 0;

		if(innerValue != null)
		{
			element.InnerText = innerValue;
		}

		if(attributeList != null)
		{
			foreach(string attribute in attributeList)
			{
				element.SetAttribute(attribute,attributeValues[index]);
				++index;
			}
		}

		return element;
	}

	//read and print a xml file with specific attribute key
	public void ParseXMLFile(string directory , string filename , string filetype, string mode,List<string> listOfParent,List<string> listOfAttributeKey)
	{
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(path + "/" + directory + "/" + filename + "." + filetype);
		XmlNodeList parentList = null;
		
		for(int i = 0 ; i< listOfParent.Count; ++i)
		{
			parentList = xmlDoc.GetElementsByTagName(listOfParent[i]);
			
			foreach(XmlNode childInfo in parentList)
			{
				XmlNodeList childContent = childInfo.ChildNodes;
				
				foreach (XmlNode childItem in childContent)
				{
					string attributestring = "";
					
					for(int i2 = 0 ; i2 < listOfAttributeKey.Count; ++i2)
					{
						attributestring += childItem.Attributes[listOfAttributeKey[i2]].Name +" = " + childItem.Attributes[listOfAttributeKey[i2]].Value + " ";
					}
					
					print( "< "+ childItem.Name + " " + " innerValue: "+ childItem.InnerText + " | " + attributestring + " >");
				}
			}
		}
	}

	//read and print a xml file
	public void ParseXMLFile(string directory , string filename , string filetype, string mode,List<string> listOfParent)
	{
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.Load(path + "/" + directory + "/" + filename + "." + filetype);
		XmlNodeList parentList = null;

		for(int i = 0 ; i< listOfParent.Count; ++i)
		{
			parentList = xmlDoc.GetElementsByTagName(listOfParent[i]);

			foreach(XmlNode childInfo in parentList)
			{
				XmlNodeList childContent = childInfo.ChildNodes;

				foreach (XmlNode childItem in childContent)
				{
					string attributestring = "";

					for(int i2 = 0 ; i2 < childItem.Attributes.Count; ++i2)
					{
						attributestring += childItem.Attributes.Item(i2).Name +" = " + childItem.Attributes.Item(i2).Value + " ";
					}

					print( "< "+ childItem.Name + " " + " innerValue: "+ childItem.InnerText + " | " + attributestring + " >");
				}
			}
		}
			
	}
	public bool CreateXMLFile(string directory, string filename,string filetype,string filedata,string mode)
	{
		if(directory == "" || filename == "" || filetype == "" || mode == "")
		{
			print ("ERROR: Invalid value detected, Operation aborted");
			return false;
		}

		if(CheckDirectory(directory) == true)
		{
			print ("Creating XML File in " + directory);

			switch (mode)
			{
				case "plaintext":
				{
					File.WriteAllText(path + "/" + directory + "/" + filename + "." + filetype,filedata);
					return true;
				}//break;

				case "xml":
				{
					return true;
				}//break;

				default:
					break;
			}
		}else
		{
			print ("Unable to create XML file as the directory: " + directory + " does not exist");
		}
		return false;
	}

	private string GetFullFileName(string filename , string filetype)
	{
		return (filename + "." + filetype);
	}
	public bool UpdateFile(string directory, string filename , string filetype, string filedata, UpdateFileMode mode)
	{
		switch(mode)
		{
		case UpdateFileMode.UPDATE_APPEND:
			return UpdateFile(directory,filename,filetype,filedata,"append");
			//break;
		case UpdateFileMode.UPDATE_REPLACE:
			return UpdateFile(directory,filename,filetype,filedata,"replace");
			//break;
		default:
			break;
		}
		return false;
	}

	public bool UpdateFile(string directory, string filename , string filetype, string filedata, string mode)
	{
		if(directory == "" || filename == "" || filetype == "" || mode == "")
		{
			print ("ERROR: Invalid value detected, Operation aborted");
			return false;
		}
		if(CheckDirectory(directory) == true)
		{
			if(CheckFile(directory + "/" + filename + "." + filetype) == true)
			{
				switch(mode)
				{
					case "replace":
					{
						File.WriteAllText(path + "/" + directory + "/" + filename + "." + filetype , filedata);
						return true;
					}
					//break;
					case "append":
					{
						File.AppendAllText(path + "/" + directory + "/" + filename + "." + filetype, filedata);
						return true;
					}
					//break;
					
				default:
					{
						print ("ERROR: Unknown mode of update");
					}break;
				}
			}else
			{
				print ("ERROR: The file " + filename + "does not exist in" + path + "/" + directory );
			}
		}else
		{
			print ("ERROR: Unable to create file as the directory " + directory + "does not exist");
		}

		return false;
	}
	public void ProcessFile(string filePath)//read and print a file
	{
		print ("processing "+ filePath);
		string fileContent = File.ReadAllText(filePath);
		print ("Read File which contains: " + fileContent);

	}
	public string ReadFile(string directory, string filename ,string filetype)
	{
		if(directory == "" || filename == "" || filetype == "")
		{
			print ("Invalid value detected, Operation aborted");
			return null;
		}

		print ("Reading " + directory + "/" + filename + "." + filetype);

		if(CheckDirectory(directory) == true)
		{
			if(CheckFile (directory + "/" + filename + "." + filetype) == true)
			{
				return File.ReadAllText(path + "/" + directory + "/" + filename + "." + filetype);
			}else
			{
				print ("The file " + filename + "does not exist in "+ path + "/" + directory);
				return null;
			}
		}else
		{
			print("Unable to read the file as the directory " + directory + " does not exist"); 
			return null;
		}

	}
	public string[] FindFiles(string directory)
	{
		if(directory == "")
		{
			print ("Invalid value detected, Operation aborted");
			return null;
		}
		print ("Checking directory " + directory + "for files");
			
		string [] fileList = Directory.GetFiles(path + "/" + directory);
		return fileList;

	}
	public string[]FindSubDirectories(string directory)
	{
		if(directory == "")
		{
			print ("Invalid value detected, Operation aborted");
			return null;
		}

		print ("Checking directory" + directory + "for sub directories");

		string [] subDirList = Directory.GetDirectories(path + "/" + directory);
		return subDirList;
	}
	private bool CreateDirectory(string parentDirectory ,string filename)//create folder
	{
		string newDirectory = parentDirectory + "/" + filename;

		if(parentDirectory == "" || filename == "")
		{
			print ("Invalid value detected, Operation aborted");
			return false;
		}
		
		if (CheckDirectory(newDirectory) == false)
		{
			print ("creating directory: " + filename);
			Directory.CreateDirectory(path + "/" + newDirectory);
			return true;
		}else
		{
			print ("ERROR: You are trying to create the directory" + filename + "but it already exists");
		}
		return false;
	}
	private bool CreateDirectory(string directory)//create folder
	{
		if(directory == "")
		{
			print ("Invalid value detected, Operation aborted");
			return false;
		}

		if (CheckDirectory(directory) == false)
		{
			print ("creating directory: " + directory);
			Directory.CreateDirectory(path + "/" + directory);
			return true;
		}else
		{
			print ("ERROR: You are trying to create the directory" + directory + "but it already exists");
		}
		return false;
	}
	public bool CreateFile(string directory,string filename, string filetype, string filedata)//create a file
	{

		if(directory == "" || filename == "" || filetype == "")
		{
			print ("Invalid value detected, Operation aborted");
			return false;
		}

		print("Creating " + directory + "/" + filename + "." + filetype);

		if (CheckDirectory(directory) == true)
		{
			if(CheckFile(directory + "/" + filename + "." + filetype) == false)
			{
				File.WriteAllText(path + "/" + directory + "/" + filename + "." + filetype, filedata);
				return true;
			}else
			{
				print("The file " + filename + " already exists in " + path + "/" + directory);
			}
		}else
		{
			print("ERROR: Unable to create file as the directory " + directory + " does not exist");
		}
		return false;
	}
	public bool DeleteFile(string filePath)//delete a file
	{
		if(CheckFile(filePath) == true)
		{
			File.Delete(path + "/" + filePath);
			return true;
		}else
		{
			print("ERROR: Unable to delete file as it does not exist");
		}
		return false;
	}
	private bool DeleteDirectory(string directory)//delete folder
	{
		if(directory == "")
		{
			print ("Invalid value detected, Operation aborted");
			return false;
		}

		if (CheckDirectory(directory) == true)
		{
			print ("deleting directory: " + directory);
			Directory.Delete(path + "/" + directory,true);
			return true;
		}else
		{
			print ("ERROR: You are trying to delete the directory" + directory + "but it does not exists");
		}
		return false;
	}
	public bool MoveFile(string originalDestination, string newDestination)//cut and paste
	{
		return MoveFile(originalDestination,newDestination,false);
	}
	public bool MoveFile(string originalDestination, string newDestination,bool overwriteConflict)//cut and paste
	{
		if(originalDestination == "" || newDestination == "")
		{
			print ("Invalid value detected, Operation aborted");
			return false;
		}

		if(CheckFile(originalDestination) == true)
		{
			if(CheckFile(newDestination) == false)
			{
				File.Move(path + "/" +originalDestination,path + "/" +newDestination);
				return true;
			}else
			{
				if(overwriteConflict == true)
				{
					File.Replace(path + "/" + originalDestination,path + "/" + newDestination,backupPath);
				}else
				{
					print ("ERROR: destinationated file already exists");
				}

			}
		}else
		{
			print ("ERROR: the file to be moved does not exist");
		}

		return false;
	}
	public bool CopyFile(string originalDestination, string newDestination)//duplciate file
	{
		return CopyFile(originalDestination,newDestination,false);
	}

	public bool CopyFile(string originalDestination, string newDestination,bool overwriteConflict)//duplciate file
	{
		if(originalDestination == "" || newDestination == "")
		{
			print ("Invalid value detected, Operation aborted");
			return false;
		}

		if(CheckFile(originalDestination) == true)
		{
			if(CheckFile(newDestination) == false)
			{
				File.Copy(path + "/" +originalDestination,path + "/" +newDestination);
				return true;
			}else
			{
				if(overwriteConflict == true)
				{
					File.WriteAllText(path + "/" + newDestination, File.ReadAllText(path + "/" + originalDestination) );
					//File.Replace(path + "/" + originalDestination,path + "/" + newDestination,backupPath);
				}else
				{
					print ("ERROR: destinationated file already exists");
				}
			}
		}else
		{
			print ("ERROR: the file to be copied does not exist");
		}
		
		return false;
	}

	private bool MoveDirectory (string originalDestination, string newDestination)//move folder
	{
		if(originalDestination == "" || newDestination == "")
		{
			print ("Invalid value detected, Operation aborted");
			return false;
		}

		if(CheckDirectory(originalDestination) == true && CheckDirectory(newDestination) == false)
		{
			print ("moving directory:" + originalDestination);
			Directory.Move(path + "/" + originalDestination, path + "/" + newDestination);
			return true;
		}else
		{
			print("ERROR: You are trying to move a directory but it either does not exist or a folder of the same name already exists"); 
		}
		return false;
	}
	public bool CheckDirectory(string directory,bool raw)
	{
		if(raw == false)
		{
			return Directory.Exists(directory);
		}
		return Directory.Exists(path + "/" + directory);
	}
	public bool CheckFile(string filePath,bool raw)
	{
		if(raw == false)
		{
			return File.Exists(filePath);
		}
		return File.Exists(path + "/" + filePath);
	}
	public bool CheckDirectory(string directory)
	{
		return Directory.Exists(path + "/" + directory);
	}
	public bool CheckFile(string filePath)
	{
		return File.Exists(path + "/" + filePath);
	}

}
