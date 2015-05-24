using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script will manage and hold all unique item data in the world in here
//this script will interact closely with player's inventory .

[System.Serializable]
public class Item_Proxy//just a container for holding key data about crafting combo
{
	public int amount = 1;
	public int itemid = -1;
	
	public Item_Proxy(int amount, int itemid)
	{
		this.amount = amount;
		this.itemid = itemid;
	}
}

[System.Serializable]
public class CraftingRecipe//i do this purely because i want to view the item in the inspector as well =P
{//container class
	public string recipe_name = "";
	public List<Item_Proxy> recipe = new List<Item_Proxy>();
	
	public CraftingRecipe(List<Item> recipe,string recipe_name)//bundle the required amount inside item.amount
	{
		this.recipe_name = recipe_name;
		
		List<Item_Proxy> temp = new List<Item_Proxy>();
		
		for(int i = 0 ; i <recipe.Count;++i)
		{
			temp.Add( MakeItemData(recipe[i].amount,recipe[i].id) );
		}
		this.recipe = temp;
	}
	public CraftingRecipe(List<int> itemIdList,List<int>itemAmount,string recipe_name)
	{
		this.recipe_name = recipe_name;
		
		List<Item_Proxy> temp = new List<Item_Proxy>();
		
		for(int i = 0 ; i <itemIdList.Count;++i)
		{
			temp.Add( MakeItemData(itemAmount[i],itemIdList[i]) );
		}
		
		this.recipe = temp;
	}
	public CraftingRecipe(List<Item_Proxy> recipe,string recipe_name)
	{
		this.recipe_name = recipe_name;
		this.recipe = recipe;
	}
	
	public CraftingRecipe(List<Item> recipe)//bundle the required amount inside item.amount
	{
		List<Item_Proxy> temp = new List<Item_Proxy>();
		
		for(int i = 0 ; i <recipe.Count;++i)
		{
			temp.Add( MakeItemData(recipe[i].amount,recipe[i].id) );
		}
		this.recipe = temp;
	}
	public CraftingRecipe(List<int> itemIdList,List<int>itemAmount)
	{
		List<Item_Proxy> temp = new List<Item_Proxy>();
		
		for(int i = 0 ; i <itemIdList.Count;++i)
		{
			temp.Add( MakeItemData(itemAmount[i],itemIdList[i]) );
		}
			
		this.recipe = temp;
	}
	public CraftingRecipe(List<Item_Proxy> recipe)
	{
		this.recipe = recipe;
	}
	public Item_Proxy MakeItemData(int amount, int itemid)
	{
		return new Item_Proxy(amount,itemid); 
	}
	public Item_Proxy ConvertItemToStorageType(Item item,int amount_required)
	{
		return new Item_Proxy(item.id,amount_required);
	}
//	public Item_Proxy ConvertItemToStorageType(Item item)
//	{
//		return new Item_Proxy(item.id,1);
//	}
//	public Item_Proxy ConvertItemToStorageType(int itemId,int itemAmount)
//	{
//		return new Item_Proxy(itemId,itemAmount);
//	}

}

public class ItemDatabase : MonoBehaviour {
	public List<Item> itemDatabase = new List<Item>();//containing all unique game item,crafted item and materals
	public List<CraftingRecipe> craftDatabase = new List<CraftingRecipe>();//containing all game item crafting combination
	public CTimer alarmitem;
	
	void Start()
	{
		alarmitem = GameObject.FindGameObjectWithTag("Alarm").GetComponent<CTimer>();//to input the script rference
		
		//PopulateGameObjectData();
		//PopulateCraftingData();
		
		PopulateTestObject();//remove when project go full launch
		PopulateTestCraftingData();
	}
	public CraftingRecipe MakeCraftingRecipe(List<int> itemIdList,List<int>itemAmount)
	{
		return new CraftingRecipe(itemIdList,itemAmount);
	}
	public CraftingRecipe MakeCraftingRecipe(List<Item> itemList)//bundle the required amount inside item.amount
	{
		return new CraftingRecipe(itemList);
	}
	public CraftingRecipe MakeCraftingRecipe(List<int> itemIdList,List<int>itemAmount,string recipe_name)
	{
		return new CraftingRecipe(itemIdList,itemAmount,recipe_name);
	}
	public CraftingRecipe MakeCraftingRecipe(List<Item> itemList,string recipe_name)//bundle the required amount inside item.amount
	{
		return new CraftingRecipe(itemList,recipe_name);
	}
//	public List<Item> MakePair(Item item1, Item item2)
//	{
//		List<Item> templist = new List<Item>();
//		templist.Add(item1);
//		templist.Add(item2);
//		return templist;
//	}
//	public List<Item> MakePair(int itemid1, int itemid2)
//	{
//		List<Item> templist = new List<Item>();
//		templist.Add(GetItem(itemid1));
//		templist.Add(GetItem(itemid2));
//		return templist;
//	}
	public void PopulateGameObjectData()//real game object data goes here
	{
	
	}
	public void PopulateCraftingData()//real game object crafting combination data goes here
	{
		
	}
	public void PopulateTestCraftingData()
	{
		//List<Item> temp = new List<Item>();
		//temp.Add(GetItem(0));
		//temp.Add(GetItem(1));	
		
		List<int> temp1 = new List<int>();
		List<int> temp2 = new List<int>();
		temp1.Add(0);
		temp1.Add(1);
		temp2.Add(2);
		temp2.Add(3);
		
		craftDatabase.Add(MakeCraftingRecipe(temp1,temp2,"test recipe"));	
	}
	public void PopulateTestObject()
	{
		itemDatabase.Add(new Item(0,"testobject",Item.ItemType.Undefined,"just a test object 0",1,1,true));
		itemDatabase.Add(new Item(1,"testobject",Item.ItemType.Consumable,"just a test alarm item 1",1,1,true));
		itemDatabase.Add(new Item(2,"testobject",Item.ItemType.Consumable,"just a test item 2",1,1,false));	
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
