using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script will manage and hold all unique item data in the world in here
//this script will interact closely with player's inventory .

[System.Serializable]
public class Item_Proxy//just a container for holding key data about crafting combo
{
	public int itemid = -1;
	public string itemname = "";
	public int amount = 1;
	
	public Item_Proxy()
	{
		this.amount = 1;
		this.itemid = -1;
		this.itemname = "";
	}
	
	public Item_Proxy(int itemid, int amount)
	{
		this.amount = amount;
		this.itemid = itemid;
	}

	public Item_Proxy(string itemname, int amount)
	{
		this.amount = amount;
		this.itemname = itemname;
	}

	public Item_Proxy(int itemid,string itemname, int amount)
	{
		this.amount = amount;
		this.itemname = itemname;
		this.itemid = itemid;
	}
	
	public Item_Proxy(Item_Proxy another)
	{
		this.amount = another.amount;
		this.itemid = another.itemid;
		this.itemname = another.itemname;
	}

	public string StringSelf()
	{
		return "Item Id/Name : " + itemid.ToString()+"/"+itemname + " Item amount: "+ amount.ToString() ;
	}
}

[System.Serializable]
public class CraftingRecipe//i do this purely because i want to view the item in the inspector as well =P
{//container class
	public string recipe_name = "";
	public int id = -1;
	public string description = "nill";
	public List<Item_Proxy> ingrediant = new List<Item_Proxy>();
	public List<Item_Proxy> output = new List<Item_Proxy>();
	public Texture2D recipeIcon ;
	public string recipeIconName = "";
	private ItemDatabase database ;

	private Item ConsultItemIdentify(int itemid)
	{
		database  = ItemDatabase.Instance;
		return database.GetItem(itemid);
	}
	private string ConsultItemName(int itemid)
	{
		database  = ItemDatabase.Instance;
		return database.GetItem(itemid).itemname;
	}

	public string RecipeIconName
	{
		get
		{
			return this.recipeIconName;
		}
		set
		{
			this.recipeIconName = value;
			LoadRecipeIcon();
		}
	}

	public CraftingRecipe()
	{
	}

	public void ReloadRecipeIcon()
	{
		this.recipeIcon = Resources.Load<Texture2D>("Item Icon/"+ recipeIconName);
	}
	public void LoadRecipeIcon()
	{
		if(output.Count >= 1)
		{
			if(recipeIconName == "" ||  recipeIconName == "nill" || recipeIconName == "none" || recipeIconName == " ")
			{
				recipeIconName = ConsultItemName(output[0].itemid);
			}

		}
		this.recipeIcon = Resources.Load<Texture2D>("Item Icon/"+ recipeIconName);
	}
	public void LoadRecipeIcon(string iconname)
	{
		recipeIconName = iconname;
		this.recipeIcon = Resources.Load<Texture2D>("Item Icon/"+ recipeIconName);
	}
	public CraftingRecipe(CraftingRecipe another)
	{

		this.id = another.id;
		this.recipeIconName = another.recipeIconName;
		this.recipe_name = another.recipe_name;
		this.description = another.description;

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

		LoadRecipeIcon();
	}
	
	public CraftingRecipe(int id ,string recipe_name,List<Item_Proxy> ingrediant,List<Item_Proxy> output,string description,string recipeIconName)
	{

		//multi in multi out
		this.id = id;
		this.recipeIconName = recipeIconName;
		this.recipe_name = recipe_name;
		this.description = description;

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
		LoadRecipeIcon();
	}	
	public CraftingRecipe(int id ,string recipe_name,List<Item_Proxy> ingrediant,Item_Proxy output,string description,string recipeIconName)
	{

		//multi in single out
		this.id = id;
		this.recipe_name = recipe_name;
		this.description = description;
		this.recipeIconName = recipeIconName;
		for (int i = 0 ; i < ingrediant.Count ; ++i)
		{
			this.ingrediant.Add(new Item_Proxy (ingrediant[i]));
		}
		//this.ingrediant = ingrediant;
		this.output.Add(output);

		LoadRecipeIcon();
	}
	
	
	public CraftingRecipe(int id ,string recipe_name,List<Item> ingrediant,List<Item> output,string description,string recipeIconName)//bundle the amount of each individual ingrediant need inside item.amount,same got for output
	{

		//multi in multi out
		this.id = id;
		this.recipe_name = recipe_name;
		this.description = description;
		this.recipeIconName = recipeIconName;
		
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

		LoadRecipeIcon();
	}
	public CraftingRecipe(int id ,string recipe_name,List<Item> ingrediant,Item output,string description,string recipeIconName)//bundle the amount of each individual ingrediant need inside item.amount,same got for output
	{

		//multi in single out
		this.recipe_name = recipe_name;
		this.id = id;
		this.description = description;
		this.recipeIconName = recipeIconName;
		for(int i = 0 ; i < ingrediant.Count; ++i)
		{
			//this.ingrediant.Add(ConvertItemToStorageType(ingrediant[i]));
			AddIngrediant(ingrediant[i]);
		}
		

		//this.output.Add(ConvertItemToStorageType(output));
		AddOutputItem(output);

		LoadRecipeIcon();
	}

		
	public CraftingRecipe(int id ,string recipe_name,List<int> ingrediant_id, List<int> ingrediant_amount,List<int> output_id, List<int> output_amount,string description,string recipeIconName)//primitive data type into storage data type
	{

		//multi in multi out
		this.recipe_name = recipe_name;
		this.id = id;
		this.description = description;
		this.recipeIconName = recipeIconName;
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
		LoadRecipeIcon();
	}	
	public CraftingRecipe(int id ,string recipe_name,List<int> ingrediant_id, List<int> ingrediant_amount,int output_id, int output_amount,string description,string recipeIconName)//primitive data type into storage data type
	{

		//multi in single out
		this.recipe_name = recipe_name;
		this.id = id;
		this.description = description;
		this.recipeIconName = recipeIconName;
		for(int i = 0 ; i < ingrediant_id.Count; ++i)
		{
			//this.ingrediant.Add(MakeItemData(ingrediant_id[i],ingrediant_amount[i]));
			AddIngrediant(ingrediant_id[i],ingrediant_amount[i]);
		}
		

		//this.output.Add(MakeItemData(output_id,output_amount));
		AddOutputItem(output_id,output_amount);

		LoadRecipeIcon();
	}


	public void AddIngrediant(Item a_item)
	{
		this.ingrediant.Add(MakeItemData(a_item.id,a_item.ItemName,a_item.amount));
	}
	public void AddIngrediant(int itemid, int amount)
	{
		this.ingrediant.Add(MakeItemData(itemid,amount));
	}
	public void AddIngrediant(int itemid,string itemname, int amount)
	{
		this.ingrediant.Add(MakeItemData(itemid,itemname,amount));
	}
	
	public void AddOutputItem(Item a_item)
	{
		this.output.Add(MakeItemData(a_item.id,a_item.ItemName,a_item.amount));
	}
	public void AddOutputItem(int itemid, int amount)
	{
		this.output.Add(MakeItemData(itemid,amount));
	}
	public void AddOutputItem(int itemid,string itemname, int amount)
	{
		this.output.Add(MakeItemData(itemid,itemname,amount));
	}


	private Item_Proxy MakeItemData(int itemid,string itemname, int amount)
	{
		return new Item_Proxy(itemid,itemname,amount); 
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
		this.ingrediant.Clear();
	}
	public void ClearOutput()
	{
		this.output.Clear();
	}
	public void ClearAll()
	{
		this.recipe_name = "";
		this.ingrediant.Clear();
		this.output.Clear();
	}
	public string StringIngrediants()
	{
		string appendstring = "";

		for(int i = 0 ; i< ingrediant.Count ; ++i)
		{
			appendstring += ConsultItemName(ingrediant[i].itemid) + "  x" +ingrediant[i].amount.ToString()+"\n";
		}

		return appendstring;
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
	public static bool initedBefore = false;
	public List<Item> itemDatabase = new List<Item>();//containing all unique game item,crafted item and materals
	public List<CraftingRecipe> craftDatabase = new List<CraftingRecipe>();//containing all game item crafting combination

	public static ItemDatabase Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("Item Database").AddComponent<ItemDatabase>();
				DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}
	public void Initialize()
	{
		Initialize(false);//don allow reinitalize of this class by default
	}
	public void Initialize(bool re_init)
	{
		if(initedBefore == false || re_init == true)
		{
			LoadItemsData();
			LoadCraftsData();
			initedBefore = true;
		}
	}

	public void OnApplicationQuit()//this will be auto called like start and update function
	{
		print ("item database get quited");
		FileManager.Instance.SaveItemDatabase();
		FileManager.Instance.SaveCraftDatabase();

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
	
	public CraftingRecipe MakeCraftingRecipe(int id,string recipieName,List<int> itemIdList,List<int>itemAmount,List<int> outputIdList,List<int>outputAmountList,string description,string recipeIconName)
	{
		return new CraftingRecipe(id,recipieName,itemIdList,itemAmount,outputIdList,outputAmountList, description, recipeIconName);
	}
	public CraftingRecipe MakeCraftingRecipe(int id,string recipieName,List<int> itemIdList,List<int>itemAmount,int outputId,int outputAmount,string description,string recipeIconName)
	{
		return new CraftingRecipe(id,recipieName,itemIdList,itemAmount,outputId,outputAmount, description, recipeIconName);
	}
	
	public CraftingRecipe MakeCraftingRecipe(int id,string recipieName,List<Item> itemList,List<Item> outputList,string description,string recipeIconName)
	{
		return new CraftingRecipe(id,recipieName,itemList,outputList, description, recipeIconName);
	}
	public CraftingRecipe MakeCraftingRecipe(int id,string recipieName,List<Item> itemList,Item output,string description,string recipeIconName)
	{
		return new CraftingRecipe(id,recipieName,itemList,output, description, recipeIconName);
	}
	
	public CraftingRecipe MakeCraftingRecipe(int id,string recipieName,List<Item_Proxy> itemList,List<Item_Proxy> outputList,string description,string recipeIconName)
	{
		return new CraftingRecipe(id,recipieName,itemList,outputList, description, recipeIconName);
	}
	public CraftingRecipe MakeCraftingRecipe(int id,string recipieName,List<Item_Proxy> itemList,Item_Proxy output,string description,string recipeIconName)
	{
		return new CraftingRecipe(id,recipieName,itemList,output, description, recipeIconName);
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
		if(id < 0 )
		{
			Debug.Log("ERROR: GetItem with id is negative value");
			return null;
		}

		if(id <itemDatabase.Count)//early prediction
		{
			if(id == itemDatabase[id].id)
			{
				return itemDatabase[id];
			}
			//if still cannot be found,brute force search
			for(int i = 0 ; i < id; ++i)
			{
				if(id == itemDatabase[i].id)
				{
					return itemDatabase[i];
				}
			}
			for(int i = id+1 ; i < itemDatabase.Count; ++i)
			{
				if(id == itemDatabase[i].id)
				{
					return itemDatabase[i];
				}
			}
		}else
		{
			Debug.Log("WARNING: GetItem with id is over list size,brute force will be performed");
			for(int i = 0 ; i < itemDatabase.Count; ++i)
			{
				if(id == itemDatabase[i].id)
				{
					return itemDatabase[i];
				}
			}
		}



		return null;
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
		return null;
	}
	public Item CreateItem(int id)//duplicate a new item same as the database one
	{

		if(id < 0 )
		{
			Debug.Log("ERROR: CreateItem with id is negative value");
			return null;
		}
		
		if(id <itemDatabase.Count)//early prediction
		{
			if(id == itemDatabase[id].id)
			{
				return new Item(itemDatabase[id]);
			}
			//if still cannot be found, brute force search
			for(int i = 0 ; i < id; ++i)
			{
				if(id == itemDatabase[i].id)
				{
					return new Item(itemDatabase[i]);
				}
			}
			for(int i = id+1 ; i < itemDatabase.Count; ++i)
			{
				if(id == itemDatabase[i].id)
				{
					return new Item(itemDatabase[i]);
				}
			}
		}else
		{
			Debug.Log("WARNING: CreateItem with id is over list size,brute force will be performed");
			for(int i = 0 ; i < itemDatabase.Count; ++i)
			{
				if(id == itemDatabase[i].id)
				{
					return new Item(itemDatabase[i]);
				}
			}
		}
		

		return null;
	}
	public bool CheckValidItemId(int id)
	{
		if(id < 0 )
		{
			Debug.Log("ERROR: CheckValidItemId with id is negative value");
			return false;
		}
		
		if(id <itemDatabase.Count)//early prediction
		{
			if(id == itemDatabase[id].id)
			{
				return true;
			}
			//if still cannot be found,brute force search
			for(int i = 0 ; i < id; ++i)
			{
				if(id == itemDatabase[i].id)
				{
					return true;
				}
			}
			for(int i = id+1 ; i < itemDatabase.Count; ++i)
			{
				if(id == itemDatabase[i].id)
				{
					return true;
				}
			}
		}else
		{
			Debug.Log("WARNING: CheckValidItemId with id is over list size,brute force will be performed");
			for(int i = 0 ; i < itemDatabase.Count; ++i)
			{
				if(id == itemDatabase[i].id)
				{
					return true;
				}
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

	public List<Item> CraftItem(List<Item>ingrediant,int recipeId)//untested
	{
		if(ingrediant.Count == 0)
		{
			return null;
		}

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
		if(ingrediant.Count == 0)
		{
			return null;
		}

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
		if(recipe == null || ingrediant.Count == 0)
		{
			return null;
		}
		if(recipe.output.Count == 0 || recipe.ingrediant.Count == 0)
		{
			return null;
		}

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
		if(recipe == null)
		{
			return false;
		}
		if(recipe.output.Count == 0 || recipe.ingrediant.Count == 0)
		{
			return false;
		}

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
		return null;
	}
	public CraftingRecipe GetCraftRecipe(int id)//beware that it is getting as reference
	{
		if(id <0)
		{
			Debug.Log("ERROR: GetCraftRecipe with id is negative value");
			return null;
		}
		if(id < craftDatabase.Count)//early prediction
		{
			if(id == craftDatabase[id].id)
			{
				return craftDatabase[id];
			}
			//if still cannot be found, brute force search
			for(int i = 0 ; i < id; ++i)
			{
				if(id == craftDatabase[i].id)
				{
					return craftDatabase[i];
				}
			}
			for(int i = id+1 ; i < craftDatabase.Count; ++i)
			{
				if(id == craftDatabase[i].id)
				{
					return craftDatabase[i];
				}
			}
		}else
		{
			Debug.Log("WARNING: GetCraftRecipe with id is over list size,brute force will be performed");
			for(int i = 0 ; i < craftDatabase.Count; ++i)
			{
				if(id == craftDatabase[i].id)
				{
					return craftDatabase[i];
				}
			}
		}


		return null;
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
		if(id <0)
		{
			Debug.Log("ERROR: CreateCraftRecipe with id is negative value");
			return null;
		}
		if(id < craftDatabase.Count)//early prediction
		{
			if(id == craftDatabase[id].id)
			{
				return new CraftingRecipe(craftDatabase[id]);
			}
			//if still cannot be found, brute force search
			for(int i = 0 ; i < id; ++i)
			{
				if(id == craftDatabase[i].id)
				{
					return new CraftingRecipe(craftDatabase[i]);
				}
			}
			
			for(int i = id+1 ; i < craftDatabase.Count; ++i)
			{
				if(id == craftDatabase[i].id)
				{
					return new CraftingRecipe(craftDatabase[i]);
				}
			}
		}else
		{
			Debug.Log("ERROR: CreateCraftRecipe with id over list size,brute force will be performed");
			for(int i = 0 ; i < craftDatabase.Count; ++i)
			{
				if(id == craftDatabase[i].id)
				{
					return new CraftingRecipe(craftDatabase[i]);
				}
			}
		}


		return new CraftingRecipe();
	}
	
	public bool CheckValidCraftRecipe(int id)
	{
		if(id <0)
		{
			Debug.Log("ERROR: CheckValidCraftRecipe with id is negative value");
			return false;
		}
		if(id < craftDatabase.Count)//early prediction
		{
			if(id == craftDatabase[id].id)
			{
				return true;
			}
			//if still cannot be found,brute force search
			for(int i = 0 ; i < id; ++i)
			{
				if(id == craftDatabase[i].id)
				{
					return true;
				}
			}
			for(int i = id+1 ; i < craftDatabase.Count; ++i)
			{
				if(id == craftDatabase[i].id)
				{
					return true;
				}
			}
		}else
		{
			Debug.Log("WARNING: CheckValidCraftRecipe with id is over list size,brute force will be performed");
			for(int i = 0 ; i < craftDatabase.Count; ++i)
			{
				if(id == craftDatabase[i].id)
				{
					return true;
				}
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
