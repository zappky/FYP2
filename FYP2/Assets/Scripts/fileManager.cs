using UnityEngine;
using UnityEditor;

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
public class fileManager : MonoBehaviour {
	//singeton//it should persist until game ends
	private static fileManager instance = null;
	private string path = null;
	private string backupPath = null;
	public string backupFolderName = "backup";

	public enum UpdateFileMode
	{
		UPDATE_REPLACE,
		UPDATE_APPEND,
	}

	public static fileManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("fileManager").AddComponent<fileManager>();
			}
			return instance;
		}

	}

//	//for testing purpose
//	public void Update()
//	{
//		if(Input.GetKeyDown("m"))
//		{
//			MoveFile("gamedata/test.gamedata", "gamedata/xml/test.gamedata",true);
//		}
//		if(Input.GetKeyDown("c"))
//		{
//			CopyFile("gamedata/test.gamedata", "gamedata/xml/test.gamedata",true);
//		}
//		if(Input.GetKeyDown("d"))
//		{
//			DeleteFile("gamedata/xml/test.gamedata");
//		}
//		if(Input.GetKeyDown("e"))
//		{
//			DeleteFile("gamedata/test.gamedata");
//		}
//	}

	public void TestBuildXMLFile()
	{
		my_XmlEntry rootentry = new my_XmlEntry("player",null,null,null);
		
		List<my_XmlBuildEntry> inputlist = new List<my_XmlBuildEntry>();
		List<string> attkey = new List<string>();
		List<string> attvalue = new List<string>();
		
		List<string> parentlist = new List<string>();
		List<my_XmlEntry> entrylist = new List<my_XmlEntry>();
		
		parentlist.Add("player");
		entrylist.Add(new my_XmlEntry("profile",null));
		parentlist.Add("player");
		entrylist.Add(new my_XmlEntry("inventory",null));
		
		//the adding of player profile here is working file, but commented off to avoid flooding debug log
		parentlist.Add("profile");
		entrylist.Add(new my_XmlEntry("playername","Bob"));
		parentlist.Add("profile");
		entrylist.Add(new my_XmlEntry("hp","3333"));
		parentlist.Add("profile");
		entrylist.Add(new my_XmlEntry("mp","333"));
		parentlist.Add("profile");
		entrylist.Add(new my_XmlEntry("level","33"));
		
		
		parentlist.Add("profile");
		entrylist.Add(new my_XmlEntry("playername","billy"));
		parentlist.Add("profile");
		entrylist.Add(new my_XmlEntry("hp","1111"));
		parentlist.Add("profile");
		entrylist.Add(new my_XmlEntry("mp","111"));
		parentlist.Add("profile");
		entrylist.Add(new my_XmlEntry("level","11"));
		
		parentlist.Add("profile");
		entrylist.Add(new my_XmlEntry("playername","billy"));
		parentlist.Add("profile");
		entrylist.Add(new my_XmlEntry("hp","0000"));
		parentlist.Add("profile");
		entrylist.Add(new my_XmlEntry("mp","000"));
		parentlist.Add("profile");
		entrylist.Add(new my_XmlEntry("level","00"));
		
		parentlist.Add("inventory");
		attkey.Clear();
		attvalue.Clear();
		attkey.Add("name");
		attvalue.Add("item1");
		attkey.Add("qty");
		attvalue.Add("10");
		entrylist.Add(new my_XmlEntry("item","test1",attkey,attvalue));
		
		
		parentlist.Add("inventory");
		attkey.Clear();
		attvalue.Clear();
		attkey.Add("name");
		attvalue.Add("item2");
		attkey.Add("qty");
		attvalue.Add("20");
		entrylist.Add(new my_XmlEntry("item","test2",attkey,attvalue));
		
		inputlist = CreateXmlBuildEntryList(parentlist,entrylist);
		
		
		CreateXMLFile("gamedata/saves","mytest","xml",BuildXMLData(rootentry,inputlist),"plaintext");
	}
	public void Initialize()
	{
		//print ("file manager init");
		path = Application.dataPath; //get path to the game data directory (the assert folder)
		//print ("path " + path);
		backupPath = path + "/" + backupFolderName;


		if (CheckDirectory("gamedata") == false)
		{
			CreateDirectory("gamedata");
		}

		TestBuildXMLFile();
	}
	public void OnApplicationQuit()//this will be auto called like start and update function
	{
		DestroyInstance();
	}
	public void DestroyInstance()
	{
		//print ("file manager instance destroyed");
		instance = null;
	}
	public List<my_XmlBuildEntry> CreateXmlBuildEntryList(List<string>parentNameList,  List<my_XmlEntry> xmlEntryList)
	{
		List<my_XmlBuildEntry> entryList = new List<my_XmlBuildEntry>();
		//my_XmlBuildEntry a_entry = new my_XmlBuildEntry();
//		for(int i = 0 ; i<xmlEntryList.Count;++i)
//		{
//			print ("Checking value in construction: " + xmlEntryList[i].StringSelf());
//		}

		for(int i = 0 ; i<xmlEntryList.Count;++i)
		{
			//a_entry = CreateBuildEntry(parentNameList[i],xmlEntryList[i]);
			//a_entry.parentname = parentNameList[i];
			//a_entry.entry = new my_XmlEntry(xmlEntryList[i]);

			//print ("Constructing: " + xmlEntryList[i].StringSelf());
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
		//XmlElement a_parentnode = null;
		XmlNodeList a_parentlist = null;

		for(int i = 0 ; i < buildEntryList.Count;++i)
		{
			//xml.GetElementsByTagName(buildEntryList[i].parentname);

			//find the parent node
			a_parentlist = xml.GetElementsByTagName(buildEntryList[i].parentname);


			if(a_parentlist != null && buildEntryList[i].parentname != null && buildEntryList[i].parentname != "")//append to existing parent
			{
				//print ("Before : " + buildEntryList[i].entry.StringSelf() );

				a_element = CreateXMLElement(xml,buildEntryList[i].entry);

//				if(a_element.HasAttributes == true)
//				{
//					print ("Name: " + a_element.Name + " inner value: " + a_element.InnerText + " key: " + a_element.Attributes[0].Name + " value:" + a_element.Attributes[0].Value);
//				}

				for(int i2 = 0 ; i2< a_parentlist.Count; ++i2)
				{

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
//	private my_XmlBuildEntry CreateBuildEntry(string parentname, string entryname,string entryvalue,List<string> entry_attribute_key,List<string> entry_attribute_value)
//	{
//		return new my_XmlBuildEntry(parentname ,new my_XmlEntry(entryname,entryvalue,entry_attribute_key,entry_attribute_value));
//	}
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

		print ("Creating XML File in " + directory);

		if(CheckDirectory(directory) == true)
		{
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
			print ("Unable to create file as the directory" + directory + "does not exist");
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
	private bool CheckDirectory(string directory)
	{
		return Directory.Exists(path + "/" + directory);
	}
	public bool CheckFile(string filePath)
	{
		return File.Exists(path + "/" + filePath);
	}

}
