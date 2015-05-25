using UnityEngine;
using System.Collections;

// this script is to describle all item attribute
[System.Serializable]//this command make the inspector allow me to check and assign the property to each element in the item database// i am new to this too
public class Item   {
	
	public string itemname = "Undefined";//Due to giving "name" will clash with in-built unity .name , a different name is given for the varable
	public int id = -1;
	public string description = "Nil";
	public int weight = -1;
	public bool stackable = false;
	public int amount = 1;
	
	public enum ItemType
	{
		Undefined,
		Consumable,
		Equipable,
		Quest
	}
	public ItemType type = ItemType.Undefined;
	public Texture2D icon ;

	public Item()//constructor
	{
		itemname = "Undefined";//Due to giving "name" will clash with in-built unity .name , a different name is given for the varable
		id = -1;
		description = "Nil";
		weight = -1;
		stackable = false;
		amount = 1;
		type = ItemType.Undefined;

		//this.icon = Resources.Load<Texture2D>("Item Icon/"+ this.itemname);
	}
	public Item( Item another)//copy constructor
	{
		this.id = another.id;
		this.itemname = another.itemname;
		this.type = another.type;
		this.description = another.description;
		this.weight = another.weight;	
		this.amount = another.amount;
		this.stackable = another.stackable;

		this.icon = another.icon;
	}
	public Item(int id, string name,ItemType type,string description,int weight,int amount,bool stackable)//constructor
	{
		this.id = id;
		this.itemname = name;
		this.type = type;
		this.description = description;
		this.weight = weight;
	
		this.amount = amount;
		this.stackable = stackable;
		this.icon = Resources.Load<Texture2D>("Item Icon/"+ this.itemname);
	}
	
}
