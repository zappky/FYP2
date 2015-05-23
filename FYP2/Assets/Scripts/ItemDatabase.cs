using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script will manage and hold all unique item data in the world in here
//this script will interact closely with player's inventory .
public class ItemDatabase : MonoBehaviour {
	public List<Item> itemDatabase = new List<Item>();
	public CTimer alarmitem;
	void Start()
	{
		PopulateTestObject();
	}
	void PopulateTestObject()
	{
		itemDatabase.Add(new Item(0,"testobject",Item.ItemType.Undefined,"just a test object",1,1,true));
		itemDatabase.Add(new Item(1,"testobject",Item.ItemType.Consumable,"just a test alarm effect item",1,1,true));
		itemDatabase.Add(new Item(2,"testobject",Item.ItemType.Consumable,"just a test item",1,1,false));
		alarmitem = GameObject.FindGameObjectWithTag("Alarm").GetComponent<CTimer>();//to input the script rference
	}
	public Item GetItem(string name)
	{
		for(int i = 0 ; i < itemDatabase.Count; ++i)
		{
			if(name == itemDatabase[i].itemname)
			{
				return itemDatabase[i];
			}
		}
		return new Item();
	}
	public Item GetItem(int id)
	{
		for(int i = 0 ; i < itemDatabase.Count; ++i)
		{
			if(id == itemDatabase[i].id)
			{
				return itemDatabase[i];
			}
		}
		return new Item();
	}
	public bool CheckValidItemId(int id)
	{
		for(int i = 0 ; i < itemDatabase.Count; ++i)
		{
			if(id == itemDatabase[i].id)
			{
				return true;
			}
		}
		return false;
	}
	public bool CheckValidItemName(string name)
	{
		for(int i = 0 ; i < itemDatabase.Count; ++i)
		{
			if(name == itemDatabase[i].itemname)
			{
				return true;
			}
		}
		
		return false;
	}
	public bool UseItemEffect(int id)
	{
		bool result = false;
		
		switch(id)
		{
		default:
		case 0:
		{
			Debug.Log("nill effect");
		}break;
			
		case 1:
		{
			Debug.Log("alarm effect");
			alarmitem.OnLookInteract();
			result = true;		
		}
			break;
		}
		
		return result;
	}
}
