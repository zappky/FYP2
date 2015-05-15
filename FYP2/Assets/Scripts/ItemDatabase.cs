using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script will manage and hold all unique item data in the world in here
//this script will interact closely with player's inventory .
public class ItemDatabase : MonoBehaviour {
	public List<Item> itemDatabase = new List<Item>();
	
	void Start()
	{
		PopulateTestObject();
	}
	void PopulateTestObject()
	{
		itemDatabase.Add(new Item(0,"testobject",Item.ItemType.Undefined,"just a test object",1));
	}
}
