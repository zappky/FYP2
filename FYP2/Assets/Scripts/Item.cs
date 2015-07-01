using UnityEngine;
using System.Collections;
// this script is to describle all item attribute
[System.Serializable]//this command make the inspector allow me to check and assign the property to each element in the item database// i am new to this too
public class Item    {
	
	public string itemname = "Undefined";//Due to giving "name" will clash with in-built unity .name , a different name is given for the varable
	public int id = -1;
	public string description = "Nil";
	public float weight = -1;//in kg
	public bool stackable = false;
	public int amount = 1;
	public int itemindexinlist = -1;//to track the index of the item in the list, for optimization, due to it having to manually managed, it wont always works
	
	public enum ItemType
	{
		Undefined,
		Useable,
		Consumable,
		Equipable,
		CraftMaterial,
		Quest
	}
	public ItemType type = ItemType.Undefined;
	public Texture2D icon ;
	public bool displayDisableIcon = false;//to flag whether to draw the disabledicon instead of normal one
	public Texture2D disabledicon ;

	public string ItemName
	{
		get
		{
			return this.itemname;
		}
		set
		{
			this.itemname = value;
			SelfReloadIcon();
		}
	}

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
		this.displayDisableIcon = another.displayDisableIcon;

		//this.icon = another.icon;
		SelfReloadIcon();
	}

	public Item(int id, string name)// shortenconstructor
	{
		this.id = id;
		this.itemname = name;
		this.type = ItemType.Undefined;
		this.description = "Nil";
		this.weight = 1;
		this.amount = 1;
		this.stackable = true;

		SelfReloadIcon();
	}
	public Item(int id, string name,string itemtype,string description,int weight,int amount,bool stackable)//constructor
	{
		this.id = id;
		this.itemname = name;

		SetItemType(itemtype);

		this.description = description;
		this.weight = weight;
		this.amount = amount;
		this.stackable = stackable;
		
		SelfReloadIcon();
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

		SelfReloadIcon();
	}
	public void SelfReloadIcon()
	{
		this.icon = Resources.Load<Texture2D>("Item Icon/"+ this.itemname);
		//if(this.type == ItemType.Consumable  ||  this.type == ItemType.Useable)
		//{
			this.disabledicon = Resources.Load<Texture2D>("Item Icon/"+ this.itemname + "_disabled");
		//}
	}
	public string GetItemType()
	{
		switch(this.type)
		{
		case ItemType.Consumable:
			return "consumable";

		case ItemType.CraftMaterial:
			return "craftmaterial";

		case ItemType.Equipable:
			return "equipable";

		case ItemType.Quest:
			return "quest";


		case ItemType.Useable:
			return "useable";

			
		case ItemType.Undefined:
		default:
			return "undefined";


		}
	}
	public void SetItemType(string itemtype)
	{
		switch(itemtype)
		{
		case "craftmaterial":
		case "Craftmaterial":
		case "craftMaterial":
		case "CraftMaterial":
			this.type = ItemType.CraftMaterial;
			break;
			
		case "equipable":
		case "Equipable":
			this.type = ItemType.Equipable;
			break;
			
		case "useable":
		case "Useable":
			this.type = ItemType.Useable;
			break;
			
		case "quest":
		case "Quest":
			this.type = ItemType.Quest;
			break;
			
		case "consumable":
		case "Consumable":
			this.type = ItemType.Consumable;
			break;
			
		case "Undefined":
		case "undefined":
		default:
			this.type = ItemType.Undefined;
			break;
		}
	}
	public float CalculateCombinedWeight()
	{
		return this.amount * this.weight;
	}
	
}
