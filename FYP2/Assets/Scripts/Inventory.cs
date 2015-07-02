using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script describle player inventory system

public class Inventory : MonoBehaviour {

	//interface variable section
	public int slotX = 5;//slot column
	public int slotY = 2;//slot rows
	public float slotXpadding = 1;//as per slotsize 
	public float slotYpadding = 1;//as per slotsize 
	public float slotsXstartposition = 0;
	public float slotsYstartposition = 0;
	public float slotsize = 20;
	public Vector2 slotfineoffset; //for representing aftermath of all mousedrag
	public Vector2 slottempoffset; //for mouse drag calulcation

	public bool display = false;
	public bool showtooltip = false;
	public string tooltip = "";
	public GUISkin skin;

	//internal storage management section
	public int fromindex = -1;
	public bool draggingitem = false;
	public Item itemdragged;
	public Vector2 mouseprevposition;

	//script reference section
	public ItemDatabase database = null;
	public PlayerInfo playerinfo = null;
	public CastSlot playercastslot = null;

	//inventory information section
	public List<Item> inventory = new List<Item>();//to hold the actual item
	public List<Item> slots = new List<Item>();//used as proxy for inventory to display,and drag and drop
	public List<Item> craftslots = new List<Item>();//used as crafting recipe representation

	private Event currentevent;

	public float weightlimit = 10;
	public float currentweight = 0;
	public int currentPage = 0;
	private Rect backgroundRect;//original background rect info
	private Rect updatedBackgroundRect;//background rect info with the updated position from being dragged
	private Rect updatedTabRect;//single temp tab rect info with the updated position from being dragged
	private List<string> pageNameList = new List<string>();//palleral arrary
	private List<string> tabNameList = new List<string>();//palleral arrary
	private List<Rect> tabRectList = new List<Rect>();//palleral arrary // original tab rect info
	private const int tabAmount = 2 ;
	private Rect toolTipRect = new Rect();
	private Rect dragItemIconRect = new Rect();


	// Use this for initialization
	void Start () {
	
		//insert the script reference
		database = ItemDatabase.Instance;
		playerinfo = this.GetComponent<PlayerInfo>();//should be relative the player object attached
		playercastslot = this.GetComponent<CastSlot>();

		float tabWidth = Screen.width*0.05f;
		float tabHeight = tabWidth*0.5f;
		
		for (int i = 0 ; i < tabAmount; ++i)
		{
			tabRectList.Add(new Rect( Screen.width *0.15f + (i)*tabWidth ,Screen.height*0.15f,tabWidth,tabHeight));

		}

		tabNameList.Add("I");
		tabNameList.Add("C");

		pageNameList.Add("Inventory");
		pageNameList.Add("Crafting");

		backgroundRect = new Rect(tabRectList[0].xMin,tabRectList[tabAmount-1].yMax,Screen.width *0.65f,Screen.height *0.65f);
		float finePadding = slotsize *0.25f;
		slotsXstartposition = backgroundRect.xMin + finePadding * 2.5f;
		slotsYstartposition = backgroundRect.yMin + finePadding * 2.5f;
		slotYpadding = slotXpadding = 1.25f;


		slotX = (int) ( (backgroundRect.width - finePadding) / (slotsize*slotXpadding ));
		slotY = (int) ( (backgroundRect.height - finePadding) / (slotsize*slotYpadding ));

		for (int i = 0; i<(slotX*slotY); ++i) //populate the slot list
		{
			slots.Add(new Item());
			craftslots.Add(new Item());
			inventory.Add(new Item());
		}
		//init weight limit
		CalculateWeightLimit();

		//just some init of the inventory item
		//AddItem (1,3);
		//AddItem (2,3);

		Item tempitem = new Item();

		for(int i = 0 ; i< database.craftDatabase.Count; ++i)
		{
			tempitem = new Item();
			if( database.craftDatabase[i].id >= 1)
			{
				tempitem.id = database.craftDatabase[i].id;//using item as representation of crafting recipe information
				tempitem.description = CreateCraftToolTip(database.craftDatabase[i]);
				tempitem.icon = database.craftDatabase[i].recipeIcon;

				for  (int i2 = 0 ; i2< craftslots.Count; ++ i2)
				{
					if(craftslots[i2].id < 0)
					{
						craftslots[i2] = tempitem;
						break;
					}
				}
			}

		}
		toolTipRect.width = slotsize*5;
		toolTipRect.height = toolTipRect.width;
		dragItemIconRect.width = slotsize;
		dragItemIconRect.height = dragItemIconRect.width;
	}
	// Update is called once per frame
	void Update () {
			if(Input.GetButtonDown("Inventory"))
			{
				ToggleDisplay();
			}
			if(display == true)
			{
				if(Input.GetKeyDown("s"))
				{
					SaveInventory();
				}
				if(Input.GetKeyDown("l"))
				{
					LoadInventory();
				}
				if(Input.GetKeyDown("t"))
				{
					AddItem(database.CraftItem(inventory,database.GetCraftRecipe(1)));
				}
			}
			
		}


	//brute force loop and return a reference to the item based on the search id
	public Item GetItem(int id)
	{
		for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
		{
			if(inventory[i].id == id)
			{
				return inventory[i];
			}
		}
		return null;
	}
	//brute force loop and return a reference to the item based on the search name
	public Item GetItem(string itemname)
	{
		for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
		{
			if(inventory[i].itemname == itemname)
			{
				return inventory[i];
			}
		}
		return null;
	}

	public void AddItemKnown(Item a_item,int index)//blind adding of item
	{
		if(index >= 0 && index < inventory.Count)
		{
			inventory[index] = a_item;
		}else
		{
			print ("ERROR: Blind inventory add item - index is acccesing out of range inventory list");
		}

	}

	public void AddItem(List<Item> Items)
	{
		//Debug.Log("debug item count " + Items.Count );

		for(int i = 0 ; i < Items.Count; ++i)
		{
			AddItem(Items[i]);
		}
	}
	public void AddItem(Item item)//here didnt check if the item is valid or weight predicted is valid
	{	
		for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
		{
			if(inventory[i].id == item.id && inventory[i].stackable == true)//if found matching item
			{
				//Debug.Log("increment existing stackable item" + item.amount);
				
				//Debug.Log("checking inventory amount" + inventory[i].amount);
				inventory[i].amount += item.amount;
				item = null;
				
				break;
			}
		}
		if(item != null) //if item is still there
		{
			for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
			{
				if(inventory[i].id < 0)//if there a slot is empty
				{
					inventory[i] = item;
					playercastslot.AddItem(new Item(inventory[i]));
					//Debug.Log("adding new item");
					break;
				}
			}
		}
	}
	public bool AddItem(string itemname)//untested
	{
		return AddItem(itemname,1);
	}
	public bool AddItem(string itemname,int amount)//untested
	{	
		if (this.currentweight >= this.weightlimit)
		{
			return false;
		}

		int trackerindex = -1;

		for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
		{
			if(inventory[i].itemname == itemname)
			{
				trackerindex = i ;
			}
			if(inventory[i].id < 0)//if there a slot is empty
			{
				for(int j = 0 ; j <database.itemDatabase.Count;++j)//loop through the item database
				{
					if(database.itemDatabase[j].itemname == itemname)//look for the matching item id 
					{
						if(database.itemDatabase[j].stackable == true)
						{

							if (trackerindex > -1)//if found
							{
								//stackable and is old item
								if(UpdateCurrentWeight(true,inventory[trackerindex].weight,inventory[trackerindex].amount+amount) == true)
								{
									inventory[trackerindex].amount += amount;
									return true;
								}
								//break;
							}else
							{
								//stackable and is new item
								if(UpdateCurrentWeight(true,database.itemDatabase[j].weight,1+amount) == true)
								{										
									inventory[i] = new Item(database.itemDatabase[j]);//add it in
									inventory[i].amount = amount;
									playercastslot.AddItem(new Item(inventory[i]));

									inventory[i].itemindexinlist = i;

									return true;
									//break;
								}

							}
							
						}else
						{
							if(UpdateCurrentWeight(true,database.itemDatabase[j].weight,inventory[i].amount + amount) == true)//ensure enough weight spacing left
							{
								for(int addcount = 0 ; addcount < amount; ++addcount)
								{
									for(int icontinue = i ; icontinue < inventory.Count; ++icontinue)//loop through whole inventory agn
									{
										if(inventory[icontinue].id < 0)//if there a slot is empty
										{
											inventory[icontinue] = new Item(database.itemDatabase[j]);//add it in
											playercastslot.AddItem(new Item(inventory[icontinue]));
											
											inventory[icontinue].itemindexinlist = icontinue;

											break;//break out once found an empty slot and inserted
										}							
									}
								}
								return true;
							}
						}
						break;	
					}
				}
				break;			
			}
		}

		return false;
	}
	public bool AddItem(int id)//untested
	{
		return AddItem(id,1);
	}
	public bool AddItem(int id,int amount)//untested
	{	
		if (this.currentweight >= this.weightlimit)
		{
			return false;
		}

		int trackerindex = -1;

		for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
		{
			if(inventory[i].id == id)
			{
				trackerindex = i ;
			}
			if(inventory[i].id < 0)//if there a slot is empty
			{
				
				for(int j = 0 ; j <database.itemDatabase.Count;++j)//loop through the item database
				{
					if(database.itemDatabase[j].id == id)//look for the matching item id 
					{
						if(database.itemDatabase[j].stackable == true)
						{
							if (trackerindex > -1)//if found
							{
								//stackable and is old item
								if(UpdateCurrentWeight(true,inventory[trackerindex].weight,inventory[trackerindex].amount+amount) == true)
								{
									inventory[trackerindex].amount += amount;
									return true;
									//break;
								}
							}else
							{
								//stackable and is new item
								if(UpdateCurrentWeight(true,database.itemDatabase[j].weight,1+amount) == true)
								{										
									inventory[i] = new Item(database.itemDatabase[j]);//add it in
									inventory[i].amount = amount;
									playercastslot.AddItem(new Item(inventory[i]));

									inventory[i].itemindexinlist = i;

									return true;
									//break;
								}
							}
							
						}else
						{
							if(UpdateCurrentWeight(true,database.itemDatabase[j].weight,inventory[i].amount + amount) == true)//ensure enough weight spacing left
							{
								for(int addcount = 0 ; addcount < amount; ++addcount)
								{
									for(int icontinue = i ; icontinue < inventory.Count; ++icontinue)//loop through whole inventory agn
									{
										if(inventory[icontinue].id < 0)//if there a slot is empty
										{
											inventory[icontinue] = new Item(database.itemDatabase[j]);//add it in
											playercastslot.AddItem(new Item(inventory[icontinue]));
											
											inventory[icontinue].itemindexinlist = icontinue;
											
											break;//break out once found an empty slot and inserted
										}	
									}
								}
								return true;
							}
						}
						break;	
					}
				}
				break;			
			}
		}
		return false;
	}
	void RemoveKnownItem(int inventoryindex)
	{
		if(inventory[inventoryindex].stackable == true ||  inventory[inventoryindex].type == Item.ItemType.Consumable)
		{
			if (UpdateCurrentWeight(false,inventory[inventoryindex].weight,1) == true) //if the weight test pass
			{
				--inventory[inventoryindex].amount;
				if(inventory[inventoryindex].amount <= 0)
				{
					//playercastslot.RemoveItem(inventory[inventoryindex]);
					playercastslot.GetItem(inventory[inventoryindex]).displayDisableIcon = true;
					inventory[inventoryindex] = new Item();

				}
			}
		}else
		{
			//playercastslot.RemoveItem(inventory[inventoryindex]);
			playercastslot.GetItem(inventory[inventoryindex]).displayDisableIcon = true;
			inventory[inventoryindex] = new Item();

		}
	}
	void RemoveItem(string itemname)
	{
		for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
		{
			if(inventory[i].itemname == itemname)
			{
				RemoveKnownItem(i);
				break;
			}
		}
	}
	void RemoveItem(int id)
	{
		for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
		{
			if(inventory[i].id == id)
			{
				RemoveKnownItem(i);
				break;
			}
		}
	}
	bool CheckContainsItem(Item a_item)
	{

		if( a_item.itemindexinlist >= 0 && a_item.itemindexinlist < inventory.Count)
		{
			if( inventory[a_item.itemindexinlist].id == a_item.id || inventory[a_item.itemindexinlist].itemname == a_item.itemname)//early prediction
			{
				//Debug.Log("early item access prediction acitvated");
				return true;
			}
		}
		//else
//		{
//			Debug.Log("normal item access acitvated");
//		}


		for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
		{
			if(inventory[i].id == a_item.id || inventory[i].itemname == a_item.itemname)
			{
				return true;
			}
		}
		return false;
	}

	bool CheckContainsItem(string itemname)
	{

		for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
		{
			if(inventory[i].itemname == itemname)
			{
				return true;
			}
		}
		return false;
	}
	bool CheckContainsItem(int id)
	{
		for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
		{
			if(inventory[i].id == id)
			{
				return true;
			}
		}
		return false;
	}
	public void UseItem(Item item)
	{
		if(CheckContainsItem(item) == false)
		{
			Debug.Log("Inventory doesnt contain item: " + item.itemname + "use item aborted");
			return;
		}

		switch(item.id)
		{
			default:
			case 0:
			{
				Debug.Log("nill effect");
			}break;
				
			case 3:
			{
			//Debug.Log("alarm1 effect");
			GameObject obj = Instantiate(Resources.Load("DecoyAlarm"),Camera.main.transform.position,new Quaternion(0,0,0,0)) as GameObject;
			CTimer escript = obj.GetComponent<CTimer>();
			escript.OnLookInteract();
			//print("Display: "  + escript.display);
			//print("Operate: "  + escript.operate);
			RemoveItem(item.id);
				//alarmitem.OnLookInteract();	
			}break;
		}

	}

	public void UseItem(Item item,int itemindex)
	{
		if(CheckContainsItem(item) == false)
		{
			Debug.Log("Inventory doesnt contain item: " + item.itemname + "use item aborted");
			return;
		}
		switch(item.id)
		{
			default:
			case 0:
			{
				Debug.Log("nill effect");
			}break;
				
			case 3:
			{
				//Debug.Log("alarm2 effect");
				GameObject obj = Instantiate(Resources.Load("DecoyAlarm"),Camera.main.transform.position,new Quaternion(0,0,0,0)) as GameObject;
				CTimer escript = obj.GetComponent<CTimer>() as CTimer;
				escript.OnLookInteract();
				//print("Display: "  + escript.display);
				//print("Operate: "  + escript.operate);
				RemoveKnownItem(itemindex);
				//alarmitem.OnLookInteract();	
			}break;
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
			GUI.skin = skin;

			currentevent = Event.current;

			if(updatedBackgroundRect.Contains(currentevent.mousePosition))
			{
				if( draggingitem == false || itemdragged == null)
				{
					if(currentevent.button == 0 && currentevent.type == EventType.mouseDown)//if left click and drag,and not currently dragging item
					{
						mouseprevposition = currentevent.mousePosition ;		
						
					}else if(currentevent.button == 0 && currentevent.type == EventType.mouseDrag)
					{ 
						slottempoffset = currentevent.mousePosition - mouseprevposition;
						
					}else if(currentevent.button == 0 && currentevent.type == EventType.mouseUp)
					{
						slotfineoffset += slottempoffset;
						
						slottempoffset.Set(0.0f,0.0f);
						
					}
				}

			}

			updatedBackgroundRect = new Rect(backgroundRect.x + slotfineoffset.x +slottempoffset.x,backgroundRect.y + slotfineoffset.y +slottempoffset.y, backgroundRect.width,backgroundRect.height);
			//GUI.Box(new Rect(backgroundRect.x + slotfineoffset.x +slottempoffset.x,backgroundRect.y + slotfineoffset.y +slottempoffset.y, backgroundRect.width,backgroundRect.height),pageNameList[currentPage].ToString());//draw background
			//GUI.Box(backgroundRect,pageNameList[currentPage].ToString());//draw background
			GUI.Box(updatedBackgroundRect,pageNameList[currentPage].ToString());//draw background

			for(int i = 0 ; i <tabAmount ; ++i)
			{
				//GUI.Box(new Rect(tabRectList[i].x + slotfineoffset.x +slottempoffset.x,tabRectList[i].y + slotfineoffset.y +slottempoffset.y,tabRectList[i].width,tabRectList[i].height  ),tabNameList[i].ToString());//draw the tabs
				updatedTabRect = new Rect(tabRectList[i].x + slotfineoffset.x +slottempoffset.x,tabRectList[i].y + slotfineoffset.y +slottempoffset.y,tabRectList[i].width,tabRectList[i].height) ;
				GUI.Box(updatedTabRect,tabNameList[i].ToString());//draw the tabs

				if(currentevent.button == 0 && currentevent.type == EventType.mouseDown && !draggingitem)//if right click down on a tab
				{
					if(updatedTabRect.Contains(currentevent.mousePosition) == true)
					{
						if(draggingitem == false)
						{
							currentPage = i;
						}
						
					}
				}
			}
			
			switch (currentPage)
			{

				//inventory page
				case 0:
					DrawInventory();
					dragItemIconRect.position = currentevent.mousePosition;

					if(draggingitem == true)//display dragged item icon on mouseposition
					{
						GUI.DrawTexture(dragItemIconRect,itemdragged.icon);
					}

					break;
				//crafting page	
				case 1:
					DrawCrafting();
					break;

				default:
					print ("ERROR: Unhandled inventory page detected");
					break;
			}

			if(showtooltip == true && draggingitem == false)
			{
				toolTipRect.position = currentevent.mousePosition;
				GUI.Box(toolTipRect,tooltip);
			}

		}
		
	}
	
	string CreateToolTip(Item item)
	{
		return "Name: "+item.itemname+"\n Amount: "+ item.amount +"\n\n"+"Description: "+ item.description;	
	}
	string CreateCraftToolTip(CraftingRecipe recipe)
	{
		return "Recipe Name: "+recipe.recipe_name+"\n Description: "+ recipe.description +"\n\n"+"Ingrediants: "+ recipe.StringIngrediants();	
	}
	void DrawCrafting()
	{
		int index = 0;
		for (int y = 0; y < slotY; ++y) 
		{
			for (int x = 0; x < slotX; ++x) 
			{
				Rect slotRect = new Rect(x*slotsize*slotXpadding + slotsXstartposition + slotfineoffset.x +slottempoffset.x, y*slotsize*slotYpadding + slotsYstartposition + slotfineoffset.y+slottempoffset.y,slotsize,slotsize);
				GUI.Box(slotRect, "",skin.GetStyle("slot"));

				if(updatedBackgroundRect.Contains(currentevent.mousePosition) == false)//if outside of the background box
				{
					showtooltip = false;
				}
				
				if(craftslots[index].id >= 0)//if slot contain an valid item
				{
					if(craftslots[index].icon != null)
					{
						GUI.DrawTexture(slotRect,craftslots[index].icon);
					}

					
					if(slotRect.Contains(currentevent.mousePosition))//check if mouse hover over the box
					{
						showtooltip = true;
						tooltip = craftslots[index].description;
						if(currentevent.button == 0 &&currentevent.type == EventType.mouseDown && currentevent.clickCount == 2)//double click
						{
							AddItem(database.CraftItem(inventory,database.GetCraftRecipe(craftslots[index].id)));
						}
					}
				}else//if current slot contain invalid item
				{
					if(slotRect.Contains(currentevent.mousePosition))//check if mouse hover over the box
					{
						showtooltip = false;
					}
					
				}
				++index;
			}
		}
	}
	void DrawInventory()
	{	
		int index = 0;
		currentevent = Event.current;
		
		if(currentevent.button == 1 && currentevent.type == EventType.mouseUp && draggingitem)//discarding item when leftclick and dragging item//temporary
		{				
			if(itemdragged.stackable == true ||  itemdragged.type == Item.ItemType.Consumable)
			{
				//RemoveItem(itemdragged.itemname);

				--itemdragged.amount;
				//Debug.Log("discarding");
				if(itemdragged.amount <= 0)
				{		
					//playercastslot.RemoveItem(itemdragged);
					playercastslot.GetItem(itemdragged).displayDisableIcon = true;
					draggingitem = false;
					itemdragged = null;

				}
			}
			
		}
		
		for (int y = 0; y < slotY; ++y) 
		{
			for (int x = 0; x < slotX; ++x) 
			{
				Rect slotRect = new Rect(x*slotsize*slotXpadding + slotsXstartposition + slotfineoffset.x +slottempoffset.x, y*slotsize*slotYpadding + slotsYstartposition + slotfineoffset.y+slottempoffset.y,slotsize,slotsize);
				//Rect slotRect = new Rect(x*60,y*60,50,50);
				//GUI.Box(slotRect, index.ToString(),skin.GetStyle("slot"));
				GUI.Box(slotRect, "",skin.GetStyle("slot"));
				slots[index] = inventory[index];//sync up

				if(updatedBackgroundRect.Contains(currentevent.mousePosition) == false)//if outside of the background box
				{
					showtooltip = false;
				}

				if(slots[index].id >= 0)//if slot contain an valid item
				{
					if(slots[index].icon != null)
					{
						GUI.DrawTexture(slotRect,slots[index].icon);
					}
					if(slotRect.Contains(currentevent.mousePosition))//check if mouse hover over the box
					{
						
						//testhit = true;
						showtooltip = true;
						//print("Index" + index);
						tooltip = CreateToolTip(slots[index]);
						
						
						if(currentevent.button == 0 &&currentevent.type == EventType.mouseDown && currentevent.clickCount == 2)//double click
						{
							//Debug.Log("double click");
							//++testdoubleclickcount;
							UseItem(slots[index],index);
						}
						
						if(currentevent.button == 0 && currentevent.type == EventType.mouseDrag && !draggingitem)//if left click and drag,and not currently dragging item
						{
							draggingitem = true;
							fromindex = index;
							itemdragged = slots[index];//copy the item
							inventory[index] = new Item(); // delete the thing.


						}
						if(currentevent.type == EventType.mouseUp && draggingitem)//dragging an item and let go of mouse
						{
							inventory[fromindex] = inventory[index];
							inventory[fromindex].itemindexinlist = fromindex;
							inventory[index] = itemdragged;
							inventory[index].itemindexinlist = index;
							draggingitem = false;
							itemdragged = null;
						}
						
					}
				}else//if current slot contain invalid item
				{
				
						
					if(slotRect.Contains(currentevent.mousePosition))//check if mouse hover over the box
					{
						//tooltip = "";
						showtooltip = false;

						if(currentevent.type == EventType.mouseUp && draggingitem)
						{
							inventory[index] = itemdragged;
							inventory[index].itemindexinlist = index;
							draggingitem = false;
							itemdragged = null;
						}

					}
					
				}
				++index;
				
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
	
	void LoadInventory()
	{
		for(int i = 0 ; i<inventory.Count;++i)
		{
			inventory[i] = PlayerPrefs.GetInt("Inventory " + i, -1) >= 0 ? database.itemDatabase[PlayerPrefs.GetInt("Inventory " + i)]: new Item();
		}
	}
	
	void SaveInventory()
	{
		for(int i = 0 ; i<inventory.Count;++i)
		{
			PlayerPrefs.SetInt("Inventory " + i,inventory[i].id);
		}
	}
	bool UpdateCurrentWeight(bool mode , float item_weight, int item_amount)
	{
		float combinedweight = item_weight * item_amount;

		if (mode == true)//add mode
		{
			if (this.currentweight + combinedweight <= this.weightlimit)
			{
				this.currentweight += combinedweight;
				return true;
			}
			
		}else
		{
			this.currentweight -= combinedweight;
			return true;
		}
		
		return false;
	}

	void UpdateCurrentWeight()//a hard update
	{
		currentweight = CalculateCurrentWeight();
	}
	
	float CalculateCurrentWeight()
	{
		float total = 0.0f;

		for(int i = 0 ; i < inventory.Count; ++i)//loop through all item in inventory
		{
			total += inventory[i].CalculateCombinedWeight();
		}
		return total;
	}
	void CalculateWeightLimit()
	{
		//backpack weight limit is usually 20%~30% of the person weight
		this.weightlimit = playerinfo.weight * 0.2f;
	}
}
