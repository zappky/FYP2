using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script describle player inventory system

public class Inventory : MonoBehaviour {
	public int slotX = 5;//slot column
	public int slotY = 2;//slot rows
	public int slotXpadding = 1;
	public int slotYpadding = 1;
	public int slotsXstartposition = 0;
	public int slotsYstartposition = 0;
	public float slotsize = 20;
	public List<Item> inventory = new List<Item>();//to hold the actual item
	public List<Item> slots = new List<Item>();//to hold blank item for slot display and easing item movement
	public ItemDatabase database;
	public bool display = false;
	public GUISkin skin;
	
	// Use this for initialization
	void Start () {
		GUI.skin = skin;
		database = GameObject.FindGameObjectWithTag("Item Database").GetComponent<ItemDatabase>();//to input the script rference

		for (int i = 0; i<(slotX*slotY); ++i) //populate the slot list
		{
			slots.Add(new Item());
		}

		for(int i = 0 ; i <10; ++i)
		{
			inventory.Add(database.itemDatabase[0]);//add testing object
		}
	}
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Inventory"))
		{
			ToggleDisplay();
		}
	}
	public void ToggleDisplay()
	{
		display = !display;
	}
	void OnGUI()
	{

		if(display == true)
		{
			DrawInventory();
		}
	}
	void DrawInventory()
	{	
		for (int x = 0; x < slotX; ++x) 
		{
			for (int y = 0; y < slotY; ++y) 
			{
				GUI.Box(new Rect(x*slotsize*slotXpadding + slotsXstartposition, y*slotsize*slotYpadding + slotsYstartposition,slotsize,slotsize), y.ToString(),skin.GetStyle("slot"));
			}
		}

	}
	void SimplePrintInventory()
	{
		for(int i = 0 ; i < inventory.Count; ++i)//loop through all item in inventory to display them
			{
				GUI.Label(new Rect(10,i * 10,200,50),inventory[i].itemname);
			}
	}
}
