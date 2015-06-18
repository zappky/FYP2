using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script will manage and hold all unique item data in the world in here
//this script will interact closely with player's inventory .

[System.Serializable]
public class Item_Proxy//just a container for holding key data about crafting combo
{
	public int itemid = -1;
	public int amount = 1;
	
	public Item_Proxy()
	{
		this.amount = 1;
		this.itemid = -1;
	}
	
	public Item_Proxy(int itemid, int amount)
	{
		this.amount = amount;
		this.itemid = itemid;
	}
	
	public Item_Proxy(Item_Proxy another)
	{
		this.amount = another.amount;
		this.itemid = another.itemid;
	}
	public string StringSelf()
	{
		return "Item Id : " + itemid.ToString() + " Item amount: "+ amount.ToString();
	}
}

[System.Serializable]
public class CraftingRecipe//i do this purely because i want to view the item in the inspector as well =P
{//container class
	public string recipe_name = "";
	public int id = -1;
	public List<Item_Proxy> ingrediant = new List<Item_Proxy>();
	public List<Item_Proxy> output = new List<Item_Proxy>();

	
	public CraftingRecipe()
	{
		
	}
	
	public CraftingRecipe(CraftingRecipe another)
	{
		this.id = another.id;
		this.recipe_name = another.recipe_name;

		for (int i = 0 ; i < another.ingrediant.Count ; ++i)
		{
			this.ingrediant.Add(new Item_Proxy (another.ingrediant[i]));
		}
		for (int i = 0 ; i < another.output.Count ; ++i)
		{
			this.output.Add(new Item_Proxy (another.output[i])) ;
		}
		//this.ingrediant = another.ingrediant;
		//this.output = another.output;
	}
	
	public CraftingRecipe(int id ,string recipe_name,List<Item_Proxy> ingrediant,List<Item_Proxy> output)
	{
		//multi in multi out
		this.id = id;
		this.recipe_name = recipe_name;

		for (int i = 0 ; i < ingrediant.Count ; ++i)
		{
			this.ingrediant.Add(new Item_Proxy (ingrediant[i]));
		}
		for (int i = 0 ; i < output.Count ; ++i)
		{
			this.output.Add(new Item_Proxy (output[i])) ;
		}

		//this.ingrediant = ingrediant;
		//this.output = output;
	}	
	public CraftingRecipe(int id ,string recipe_name,List<Item_Proxy> ingrediant,Item_Proxy output)
	{
		//multi in single out
		this.id = id;
		this.recipe_name = recipe_name;
		for (int i = 0 ; i < ingrediant.Count ; ++i)
		{
			this.ingrediant.Add(new Item_Proxy (ingrediant[i]));
		}
		//this.ingrediant = ingrediant;
		this.output.Add(output);
	}
	
	
	public CraftingRecipe(int id ,string recipe_name,List<Item> ingrediant,List<Item> output)//bundle the amount of each individual ingrediant need inside item.amount,same got for output
	{
		//multi in multi out
		this.id = id;
		this.recipe_name = recipe_name;
		
		for(int i = 0 ; i < ingrediant.Count; ++i)
		{
			//this.ingrediant.Add(ConvertItemToStorageType(ingrediant[i]));
			AddIngrediant(ingrediant[i]);
		}
		
		for(int i = 0 ; i < output.Count; ++i)
		{
			//this.output.Add(ConvertItemToStorageType(output[i]));
			AddOutputItem(output[i]);
		}
	}
	public CraftingRecipe(int id ,string recipe_name,List<Item> ingrediant,Item output)//bundle the amount of each individual ingrediant need inside item.amount,same got for output
	{
		//multi in single out
		this.recipe_name = recipe_name;
		this.id = id;
		for(int i = 0 ; i < ingrediant.Count; ++i)
		{
			//this.ingrediant.Add(ConvertItemToStorageType(ingrediant[i]));
			AddIngrediant(ingrediant[i]);
		}
		

		//this.output.Add(ConvertItemToStorageType(output));
		AddOutputItem(output);
	}

		
	public CraftingRecipe(int id ,string recipe_name,List<int> ingrediant_id, List<int> ingrediant_amount,List<int> output_id, List<int> output_amount)//primitive data type into storage data type
	{
		//multi in multi out
		this.recipe_name = recipe_name;
		this.id = id;
		for(int i = 0 ; i < ingrediant_id.Count; ++i)
		{
			//this.ingrediant.Add(MakeItemData(ingrediant_id[i],ingrediant_amount[i]));
			AddIngrediant(ingrediant_id[i],ingrediant_amount[i]);
		}
		
		for(int i = 0 ; i < output_id.Count; ++i)
		{
			//this.output.Add(MakeItemData(output_id[i],output_amount[i]));
			AddOutputItem(output_id[i],output_amount[i]);
		}
	}	
	public CraftingRecipe(int id ,string recipe_name,List<int> ingrediant_id, List<int> ingrediant_amount,int output_id, int output_amount)//primitive data type into storage data type
	{
		//multi in single out
		this.recipe_name = recipe_name;
		this.id = id;
		for(int i = 0 ; i < ingrediant_id.Count; ++i)
		{
			//this.ingrediant.Add(MakeItemData(ingrediant_id[i],ingrediant_amount[i]));
			AddIngrediant(ingrediant_id[i],ingrediant_amount[i]);
		}
		

		//this.output.Add(MakeItemData(output_id,output_amount));
		AddOutputItem(output_id,output_amount);
	}

	public void AddIngrediant(Item a_item)
	{
		this.ingrediant.Add(MakeItemData(a_item.id,a_item.amount));
	}
	public void AddOutputItem(Item a_item)
	{
		this.output.Add(MakeItemData(a_item.id,a_item.amount));
	}

	public void AddIngrediant(int itemid, int amount)
	{
		this.ingrediant.Add(MakeItemData(itemid,amount));
	}
	public void AddOutputItem(int itemid, int amount)
	{
		this.output.Add(MakeItemData(itemid,amount));
	}

	private Item_Proxy MakeItemData(int itemid, int amount)
	{
		return new Item_Proxy(itemid,amount); 
	}
	private Item_Proxy ConvertItemToStorageType(Item item,int amount_required)
	{
		return new Item_Proxy(item.id,amount_required);
	}
	private Item_Proxy ConvertItemToStorageType(Item item)
	{
		return new Item_Proxy(item.id,item.amount);
	}
	private Item_Proxy ConvertItemToStorageTypeFlatten(Item item)
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

	public string StringSelf()
	{
		string appendstring = "";
		string appendstringt = "";
		for(int i = 0 ; i< ingrediant.Count ; ++i)
		{
			appendstring += ingrediant[i].StringSelf();
		}
		for(int i = 0 ; i< output.Count ; ++i)
		{
			appendstringt += output[i].StringSelf();
		}
		return "Recipe id : " + id.ToString() + " Recipe name: " + recipe_name + "\n Ingrediants: " + appendstring + "\n outputs: " + appendstringt ;
	}
}

public class ItemDatabase : MonoBehaviour {
	private static ItemDatabase instance = null;
	public List<Item> itemDatabase = new List<Item>();//containing all unique game item,crafted item and materals
	public List<CraftingRecipe> craftDatabase = new List<CraftingRecipe>();//containing all game item crafting combination
	public CTimer alarmitem = null;

	public static ItemDatabase Instance
	{
		get
		{
			if(instance == null)
			{

				//not needed
//				GameObject[] detectionlist = GameObject.FindGameObjectsWithTag("Item Database");
//				//print("duplicate detected: " + detectionlist.Length);
//				if (detectionlist.Length > 1)
//				{
//					foreach (GameObject item in detectionlist)
//					{
//						Destroy(item);
//					}
//				}

				instance = new GameObject("Item Database").AddComponent<ItemDatabase>();
			}
			return instance;
		}
	}
	public void Initialize()
	{	
		alarmitem = GameObject.FindGameObjectWithTag("Alarm").GetComponent<CTimer>();//to input the script rference
			//alarmitem = this.GetComponent<CTimer>();
			//PopulateGameObjectData();
			//PopulateCraftingData();
				
		LoadItemsData();
		LoadCraftsData();
		//PopulateTestObject();//remove when project go full launch
		//PopulateTestCraftingData();
	}
	public void OnApplicationQuit()//this will be auto called like start and update function
	{
		FileManager.Instance.SaveDatabase();
		DestroyInstance();
	}
	public void DestroyInstance()
	{
		//print ("database instance destroyed");
		instance = null;
	}
	public void LoadCraftsData()
	{
		this.craftDatabase = FileManager.Instance.LoadCraftsData();
	}
	public void LoadItemsData()
	{
		this.itemDatabase = FileManager.Instance.LoadItemsData();
	}
	
	public CraftingRecipe MakeCraftingRecipe(int id,string recipieName,List<int> itemIdList,List<int>itemAmount,List<int> outputIdList,List<int>outputAmountList)
	{
		return new CraftingRecipe(id,recipieName,itemIdList,itemAmount,outputIdList,outputAmountList);
	}
	public CraftingRecipe MakeCraftingRecipe(int id,string recipieName,List<int> itemIdList,List<int>itemAmount,int outputId,int outputAmount)
	{
		return new CraftingRecipe(id,recipieName,itemIdList,itemAmount,outputId,outputAmount);
	}
	
	public CraftingRecipe MakeCraftingRecipe(int id,string recipieName,List<Item> itemList,List<Item> outputList)
	{
		return new CraftingRecipe(id,recipieName,itemList,outputList);
	}
	public CraftingRecipe MakeCraftingRecipe(int id,string recipieName,List<Item> itemList,Item output)
	{
		return new CraftingRecipe(id,recipieName,itemList,output);
	}
	
	public CraftingRecipe MakeCraftingRecipe(int id,string recipieName,List<Item_Proxy> itemList,List<Item_Proxy> outputList)
	{
		return new CraftingRecipe(id,recipieName,itemList,outputList);
	}
	public CraftingRecipe MakeCraftingRecipe(int id,string recipieName,List<Item_Proxy> itemList,Item_Proxy output)
	{
		return new CraftingRecipe(id,recipieName,itemList,output);
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
		temp1.Add(CreateItem("battery"));
		temp1.Add(CreateItem("gear"));	
		
		List<Item> temp2 = new List<Item>();
		temp2.Add(CreateItem("clock"));
		temp2[0].amount = 1;
		craftDatabase.Add(MakeCraftingRecipe(0,"test Alarm recipe",temp1,temp2));	
	}
	public void PopulateTestObject()
	{
		itemDatabase.Add(new Item(1,"battery",Item.ItemType.CraftMaterial,"just a test object 0",1,1,true));	
		itemDatabase.Add(new Item(2,"gear",Item.ItemType.CraftMaterial,"just a test item 1",1,1,true));	
		itemDatabase.Add(new Item(3,"clock",Item.ItemType.Useable,"just a test alarm item 2",1,1,false));
	}
	public Item GetItemWithIndex(int index)
	{
		return itemDatabase[index];
	}
	public Item GetItem(string name)//beware that this is getting a reference, whatever you do to the returned item,gonna affect the database
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
	public Item GetItem(int id)//beware that this is getting a reference, whatever you do to the returned item,gonna affect the database
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
	public Item CreateItemWithIndex(int index)
	{
		return new Item(itemDatabase[index]);
	}
	public Item CreateItem(string name)//duplicate a new item same as the database one
	{
		for(int i = 0 ; i < itemDatabase.Count; ++i)
		{
			if(name == itemDatabase[i].itemname)
			{
				return new Item(itemDatabase[i]);
			}
		}
		return new Item();
	}
	public Item CreateItem(int id)//duplicate a new item same as the database one
	{
		for(int i = 0 ; i < itemDatabase.Count; ++i)
		{
			if(id == itemDatabase[i].id)
			{
				return new Item(itemDatabase[i]);
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
	public bool UseItemEffect(int id)//activate other script effect here
	{
		bool result = false;
		
		switch(id)
		{
			default:
			case 0:
			{
				Debug.Log("nill effect");
			}break;
				
			case 3:
			{
				Debug.Log("alarm effect");
				alarmitem.OnLookInteract();
				result = true;		
			}break;
		}
		
		return result;
	}
	public List<Item> CraftItem(List<Item>ingrediant,int recipeId)//untested
	{
		int matches = 0;
		bool skip = false;
		List<Item_Proxy> match = new List<Item_Proxy>(); 
		List<int> matchedId = new List<int>();//a list to mark which recipe ingrediant has already been recored
		List<Item> output = new List<Item>();//meant to be empty just for return
		CraftingRecipe recipe = GetCraftRecipe(recipeId);
		
		for(int i = 0 ; i < ingrediant.Count; ++i)
		{
			for(int k = 0 ; k < recipe.ingrediant.Count; ++k)
			{
				skip = false; //reset flag
				
				for(int j = 0 ; j < matchedId.Count; ++j)
				{
					if ( k == matchedId[j])//skip the already checked ingrediant
					{
						skip = true;
						break;
					}
				}
				if (skip == false)
				{
					if(ingrediant[i].id == recipe.ingrediant[k].itemid && ingrediant[i].amount >= recipe.ingrediant[k].amount)
					{
						++matches;
						match.Add(new Item_Proxy(i,recipe.ingrediant[k].amount));//keep record of which ingrediant index and how many of its amount to deduct.
						matchedId.Add(k);
						break;
					}
				}
			}
		}
		if(matches == recipe.ingrediant.Count)
		{
			//deduct the ingrediant amount
			for(int j = 0 ; j<match.Count;++j)
			{
				//itemid here is used as index to quickly access the ingrediant to be deducted.
				ingrediant[match[j].itemid].amount -= match[j].amount;
				if(ingrediant[match[j].itemid].amount <= 0)
				{
					ingrediant[match[j].itemid] = new Item();
				}
			}		
			return CreateOutputItem(recipe);//return the output list
		}
		return output;//return empty output list
	}
	public List<Item> CraftItem(List<Item>ingrediant,string recipeName)//untested
	{
		int matches = 0;
		bool skip = false;
		List<Item_Proxy> match = new List<Item_Proxy>(); 
		List<int> matchedId = new List<int>();//a list to mark which recipe ingrediant has already been recored
		List<Item> output = new List<Item>();//meant to be empty just for return
		CraftingRecipe recipe = GetCraftRecipe(recipeName);
		
		for(int i = 0 ; i < ingrediant.Count; ++i)
		{
			for(int k = 0 ; k < recipe.ingrediant.Count; ++k)
			{
				skip = false; //reset flag
				
				for(int j = 0 ; j < matchedId.Count; ++j)
				{
					if ( k == matchedId[j])//skip the already checked ingrediant
					{
						skip = true;
						break;
					}
				}
				if (skip == false)
				{
					if(ingrediant[i].id == recipe.ingrediant[k].itemid && ingrediant[i].amount >= recipe.ingrediant[k].amount)
					{
						++matches;
						match.Add(new Item_Proxy(i,recipe.ingrediant[k].amount));//keep record of which ingrediant index and how many of its amount to deduct.
						matchedId.Add(k);
						break;
					}
				}
			}
		}
		if(matches == recipe.ingrediant.Count)
		{
			//deduct the ingrediant amount
			for(int j = 0 ; j<match.Count;++j)
			{
				//itemid here is used as index to quickly access the ingrediant to be deducted.
				ingrediant[match[j].itemid].amount -= match[j].amount;
				if(ingrediant[match[j].itemid].amount <= 0)
				{
					ingrediant[match[j].itemid] = new Item();
				}
			}		
			return CreateOutputItem(recipe);//return the output list
		}
		return output;//return empty output list
	}
	
	public List<Item> CraftItem(List<Item>ingrediant,CraftingRecipe recipe)//take in ingrediant and recipe and produce out the crafted items in a list
	{
		int matches = 0;
		bool skip = false;
		List<Item_Proxy> match = new List<Item_Proxy>(); 
		List<int> matchedId = new List<int>();//a list to mark which recipe ingrediant has already been recored
		List<Item> output = new List<Item>();//meant to be empty just for return
		
		for(int i = 0 ; i < ingrediant.Count; ++i)
		{
			for(int k = 0 ; k < recipe.ingrediant.Count; ++k)
			{
				skip = false; //reset flag

				for(int j = 0 ; j < matchedId.Count; ++j)
				{
					if ( k == matchedId[j])//skip the already checked ingrediant
					{
						skip = true;
						break;
					}
				}
				if (skip == false)
				{
					if(ingrediant[i].id == recipe.ingrediant[k].itemid && ingrediant[i].amount >= recipe.ingrediant[k].amount)
					{
						++matches;
						match.Add(new Item_Proxy(i,recipe.ingrediant[k].amount));//keep record of which ingrediant index and how many of its amount to deduct.
						matchedId.Add(k);
						break;
					}
				}
			}
		}
		if(matches == recipe.ingrediant.Count)
		{
			//deduct the ingrediant amount
			for(int j = 0 ; j<match.Count;++j)
			{
				//itemid here is used as index to quickly access the ingrediant to be deducted.
				ingrediant[match[j].itemid].amount -= match[j].amount;
				if(ingrediant[match[j].itemid].amount <= 0)
				{
					ingrediant[match[j].itemid] = new Item();
				}
			}		
			return CreateOutputItem(recipe);//return the output list
		}
		return output;//return empty output list
	}
	
	public List<Item> CreateOutputItem(CraftingRecipe recipe)//just manufacture the crafted item in a list
	{
		List<Item> output = new List<Item>();
		Item tempitem = new Item();
		
		for(int i = 0 ; i < recipe.output.Count;++i)
		{
			tempitem = CreateItem (recipe.output[i].itemid);
			tempitem.amount = recipe.output[i].amount;
			output.Add(tempitem);
		}
		
		return output;
	}
	
	public bool CheckCraftable(List<Item>ingrediant,CraftingRecipe recipe)
	{
		int matches = 0;
		
		for(int i = 0 ; i < ingrediant.Count; ++i)
		{
			for(int k = 0 ; k < recipe.ingrediant.Count; ++k)
			{
				if(ingrediant[i].id == recipe.ingrediant[k].itemid && ingrediant[i].amount >= recipe.ingrediant[k].amount)
				{
					++matches;
					break;
				}
			}
		}
		if(matches == recipe.ingrediant.Count)
		{
			return true;
		}
		return false;
	}
	public CraftingRecipe GetCraftRecipe(string name)//beware that it is getting as reference
	{
		for(int i = 0 ; i < craftDatabase.Count; ++i)
		{
			if(name == craftDatabase[i].recipe_name)
			{
				return craftDatabase[i];
			}
		}
		return new CraftingRecipe();
	}
	public CraftingRecipe GetCraftRecipe(int id)//beware that it is getting as reference
	{
		for(int i = 0 ; i < craftDatabase.Count; ++i)
		{
			if(id == craftDatabase[i].id)
			{
				return craftDatabase[i];
			}
		}
		return new CraftingRecipe();
	}
	
	public CraftingRecipe CreateCraftRecipe(string name)//creating new crafting recipe based on the database one
	{
		for(int i = 0 ; i < craftDatabase.Count; ++i)
		{
			if(name == craftDatabase[i].recipe_name)
			{
				return new CraftingRecipe(craftDatabase[i]);
			}
		}
		return new CraftingRecipe();
	}
	public CraftingRecipe CreateCraftRecipe(int id)///creating new crafting recipe based on the database one
	{
		for(int i = 0 ; i < craftDatabase.Count; ++i)
		{
			if(id == craftDatabase[i].id)
			{
				return new CraftingRecipe(craftDatabase[i]);
			}
		}
		return new CraftingRecipe();
	}
	
	public bool CheckValidCraftRecipe(int id)
	{
		for(int i = 0 ; i < craftDatabase.Count; ++i)
		{
			if(id == craftDatabase[i].id)
			{
				return true;
			}
		}
		return false;
	}
	public bool CheckValidCraftRecipe(string name)
	{
		for(int i = 0 ; i < craftDatabase.Count; ++i)
		{
			if(name == craftDatabase[i].recipe_name)
			{
				return true;
			}
		}
		
		return false;
	}

}
