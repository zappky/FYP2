using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script describle player inventory system

public class Inventory : MonoBehaviour {

	public List<Item> inventory = new List<Item>();
	public ItemDatabase database;
	public bool display = false;
	
	// Use this for initialization
	void Start () {
		database = GameObject.FindGameObjectWithTag("Item Database").GetComponent<ItemDatabase>();//to input the script rference
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
		
		for(int i = 0 ; i < inventory.Count; ++i)//loop through all item in inventory to display them
		{
			GUI.Label(new Rect(10,i * 10,200,50),inventory[i].itemname);
		}
	}
}
