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
	
	public Item_Proxy(int itemid, int amount)
	{
		this.amount = amount;
		this.itemid = itemid;
	}
}

[System.Serializable]
public class CraftingRecipe//i do this purely because i want to view the item in the inspector as well =P
{//container class
	public string recipe_name = "";
	public List<Item_Proxy> ingrediant = new List<Item_Proxy>();
	public List<Item_Proxy> output = new List<Item_Proxy>();
	
	public CraftingRecipe(string recipe_name,List<Item_Proxy> ingrediant,List<Item_Proxy> output)
	{
		//multi in multi out
		this.recipe_name = recipe_name;
		this.ingrediant = ingrediant;
		this.output = output;
	}	
	public CraftingRecipe(string recipe_name,List<Item_Proxy> ingrediant,Item_Proxy output)
	{
		//multi in single out
		this.recipe_name = recipe_name;
		this.ingrediant = ingrediant;
		this.output.Add(output);
	}
	
	
	public CraftingRecipe(string recipe_name,List<Item> ingrediant,List<Item> output)//bundle the amount of each individual ingrediant need inside item.amount,same got for output
	{
		//multi in multi out
		this.recipe_name = recipe_name;
		
		for(int i = 0 ; i < ingrediant.Count; ++i)
		{
			this.ingrediant.Add(ConvertItemToStorageType(ingrediant[i]));
		}
		
		for(int i = 0 ; i < output.Count; ++i)
		{
			this.output.Add(ConvertItemToStorageType(output[i]));
		}
	}
	public CraftingRecipe(string recipe_name,List<Item> ingrediant,Item output)//bundle the amount of each individual ingrediant need inside item.amount,same got for output
	{
		//multi in single out
		this.recipe_name = recipe_name;
		
		for(int i = 0 ; i < ingrediant.Count; ++i)
		{
			this.ingrediant.Add(ConvertItemToStorageType(ingrediant[i]));
		}
		
		//for(int i = 0 ; i < output.Count; ++i)
		//{
			this.output.Add(ConvertItemToStorageType(output));
		//}
	}

		
	public CraftingRecipe(string recipe_name,List<int> ingrediant_id, List<int> ingrediant_amount,List<int> output_id, List<int> output_amount)//primitive data type into storage data type
	{
		//multi in multi out
		this.recipe_name = recipe_name;
		
		for(int i = 0 ; i < ingrediant_id.Count; ++i)
		{
			this.ingrediant.Add(MakeItemData(ingrediant_id[i],ingrediant_amount[i]));
		}
		
		for(int i = 0 ; i < output_id.Count; ++i)
		{
			this.output.Add(MakeItemData(output_id[i],output_amount[i]));
		}
	}	
	public CraftingRecipe(string recipe_name,List<int> ingrediant_id, List<int> ingrediant_amount,int output_id, int output_amount)//primitive data type into storage data type
	{
		//multi in single out
		this.recipe_name = recipe_name;
		
		for(int i = 0 ; i < ingrediant_id.Count; ++i)
		{
			this.ingrediant.Add(MakeItemData(ingrediant_id[i],ingrediant_amount[i]));
		}
		
		//for(int i = 0 ; i < output_id.Count; ++i)
		//{
			this.output.Add(MakeItemData(output_id,output_amount));
		//}
	}
	
	public Item_Proxy MakeItemData(int itemid, int amount)
	{
		return new Item_Proxy(itemid,amount); 
	}
	public Item_Proxy ConvertItemToStorageType(Item item,int amount_required)
	{
		return new Item_Proxy(item.id,amount_required);
	}
	public Item_Proxy ConvertItemToStorageType(Item item)
	{
		return new Item_Proxy(item.id,item.amount);
	}
	public Item_Proxy ConvertItemToStorageTypeFlatten(Item item)
	{
		return new Item_Proxy(item.id,1);
	}
	public void ClearIngrediant()
	{
		//this.recipe_name = "";
		this.ingrediant.Clear();
		//this.output.Clear();
	}
	public void ClearOutput()
	{
		//this.recipe_name = "";
		//this.ingrediant.Clear();
		this.output.Clear();
	}
	public void ClearAll()
	{
		this.recipe_name = "";
		this.ingrediant.Clear();
		this.output.Clear();
	}
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
	
	
	public CraftingRecipe MakeCraftingRecipe(string recipieName,List<int> itemIdList,List<int>itemAmount,List<int> outputIdList,List<int>outputAmountList)
	{
		return new CraftingRecipe(recipieName,itemIdList,itemAmount,outputIdList,outputAmountList);
	}
	public CraftingRecipe MakeCraftingRecipe(string recipieName,List<int> itemIdList,List<int>itemAmount,int outputId,int outputAmount)
	{
		return new CraftingRecipe(recipieName,itemIdList,itemAmount,outputId,outputAmount);
	}
	
	public CraftingRecipe MakeCraftingRecipe(string recipieName,List<Item> itemList,List<Item> outputList)
	{
		return new CraftingRecipe(recipieName,itemList,outputList);
	}
	public CraftingRecipe MakeCraftingRecipe(string recipieName,List<Item> itemList,Item output)
	{
		return new CraftingRecipe(recipieName,itemList,output);
	}
	
	public CraftingRecipe MakeCraftingRecipe(string recipieName,List<Item_Proxy> itemList,List<Item_Proxy> outputList)
	{
		return new CraftingRecipe(recipieName,itemList,outputList);
	}
	public CraftingRecipe MakeCraftingRecipe(string recipieName,List<Item_Proxy> itemList,Item_Proxy output)
	{
		return new CraftingRecipe(recipieName,itemList,output);
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
		List<Item> temp1 = new List<Item>();
		temp1.Add(GetItem(0));
		temp1.Add(GetItem(1));	
		
		List<Item> temp2 = new List<Item>();
		temp2.Add(GetItem(2));
		
		craftDatabase.Add(MakeCraftingRecipe("test recipe",temp1,temp2));	
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
