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

	public float weightlimit = 10;
	public float currentweight = 0;

	// Use this for initialization
	void Start () {
	
		//insert the script reference
		database = ItemDatabase.Instance;
		playerinfo = this.GetComponent<PlayerInfo>();//should be relative the player object attached
		playercastslot = this.GetComponent<CastSlot>();

		for (int i = 0; i<(slotX*slotY); ++i) //populate the slot list
		{
			slots.Add(new Item());
			inventory.Add(new Item());
		}
		//init weight limit
		CalculateWeightLimit();

		//just some init of the inventory item
		AddItem ("battery");
		AddItem ("battery");

		AddItem ("gear");
		AddItem ("gear");

		//playercastslot.AddItem(new Item(inventory[0]));
		//AddItem (1);

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
		void SwapInventoryItem(int indexfrom,int indexto)
		{
			Item tempitem = inventory[indexfrom];
			inventory[indexfrom] = inventory[indexto];
			inventory[indexto] = tempitem;
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

		//brute force loop and return a index to the item based on the search id
		public int GetItemIndex(int id)
		{
			for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
			{
				if(inventory[i].id == id)
				{
					return i;
				}
			}
			return -1;
		}
		//brute force loop and return a index to the item based on the search name
		public int GetItemIndex(string itemname)
		{
			for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
			{
				if(inventory[i].itemname == itemname)
				{
					return i;
				}
			}
			return -1;
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
		public void AddItem(string itemname)//untested
		{	
			if (this.currentweight >= this.weightlimit)
			{
				return;
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
								
//								bool found = false;
//								for(int k = 0 ; k < inventory.Count; ++k)//loop through whole inventory //not efficient,but cannot think of better alogrithm
//								{
//									if(inventory[k].itemname == itemname)//if there a slot is empty
//									{
//										++inventory[k].amount;
//										found = true;
//										break;
//									}
//								}
//								if(!found)
//								{
//									inventory[i] = new Item(database.itemDatabase[j]);//add it in
//									break;
//								}
								if (trackerindex > -1)//if found
								{
									if(UpdateCurrentWeight(true,inventory[trackerindex].weight,1) == true)
									{
										++inventory[trackerindex].amount;
									}
									break;
								}else
								{
									if(UpdateCurrentWeight(true,database.itemDatabase[j].weight,1) == true)
									{										
										inventory[i] = new Item(database.itemDatabase[j]);//add it in
										playercastslot.AddItem(new Item(inventory[i]));
										break;
									}

								}
								
							}else
							{
								
								if(UpdateCurrentWeight(true,database.itemDatabase[j].weight,1) == true)
								{										
									inventory[i] = new Item(database.itemDatabase[j]);//add it in
									playercastslot.AddItem(new Item(inventory[i]));
								}
							}
							break;	
						}
					}
					break;			
				}
			}
		}
		public void AddItem(int id)
		{	
			if (this.currentweight >= this.weightlimit)
			{
				return;
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
									if(UpdateCurrentWeight(true,inventory[trackerindex].weight,1) == true)
									{
										++inventory[trackerindex].amount;
										break;
									}
								}else
								{
									if(UpdateCurrentWeight(true,database.itemDatabase[j].weight,1) == true)
									{										
										inventory[i] = new Item(database.itemDatabase[j]);//add it in
										playercastslot.AddItem(new Item(inventory[i]));
										break;
									}
								}
								
							}else
							{
								if(UpdateCurrentWeight(true,database.itemDatabase[j].weight,1) == true)
								{	
									inventory[i] = new Item(database.itemDatabase[j]);//add it in
									playercastslot.AddItem(new Item(inventory[i]));
								}
							}
							break;	
						}
					}
					break;			
				}
			}
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
						inventory[inventoryindex] = new Item();
						playercastslot.RemoveItem(inventory[inventoryindex]);
					}
				}
			}else
			{
				inventory[inventoryindex] = new Item();
				playercastslot.RemoveItem(inventory[inventoryindex]);
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
			if(database.UseItemEffect(item.id))//if successful
			{
				RemoveItem(item.id);
			}
		}
		public void UseItem(Item item,int itemindex)
		{
			if(database.UseItemEffect(item.id))//if successful
			{
				RemoveKnownItem(itemindex);
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
				//SimplePrintInventory();
				DrawInventory();
				if(showtooltip == true && draggingitem == false)
				{
					GUI.Box(new Rect(Event.current.mousePosition.x,Event.current.mousePosition.y,slotsize*5,slotsize*5),tooltip);
				}
				if(draggingitem == true)
				{
					GUI.DrawTexture(new Rect(Event.current.mousePosition.x,Event.current.mousePosition.y,slotsize,slotsize),itemdragged.icon);
				}
			}
			
		}
		
		string CreateToolTip(Item item)
		{
			return "Name: "+item.itemname+"\n Amount: "+ item.amount +"\n\n"+"Description: "+ item.description;	
		}
	void DrawInventory()
	{	
		int index = 0;
		Event currentevent = Event.current;
		
		if(currentevent.button == 1 && currentevent.type == EventType.mouseUp && draggingitem)//discarding item when leftclick and dragging item//temporary
		{				
			if(itemdragged.stackable == true ||  itemdragged.type == Item.ItemType.Consumable)
			{
				--itemdragged.amount;
				//Debug.Log("discarding");
				if(itemdragged.amount <= 0)
				{			
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
			
				if(slots[index].id >= 0)//if slot contain an valid item
				{
					GUI.DrawTexture(slotRect,slots[index].icon);
					
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
							inventory[index] = itemdragged;
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
							draggingitem = false;
							itemdragged = null;
						}
						
						if(currentevent.button == 0 && currentevent.type == EventType.mouseDown && !draggingitem)//if left click and drag,and not currently dragging item
						{
							mouseprevposition = currentevent.mousePosition ;		
							
						}else if(currentevent.button == 0 && currentevent.type == EventType.mouseDrag && !draggingitem)
						{ 
							slottempoffset = currentevent.mousePosition - mouseprevposition;
						}else if(currentevent.button == 0 && currentevent.type == EventType.mouseUp && !draggingitem)
						{
							slotfineoffset += slottempoffset;
							slottempoffset.Set(0.0f,0.0f);
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
