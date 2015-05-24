using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script describle player inventory system

public class Inventory : MonoBehaviour {
	public int slotX = 5;//slot column
	public int slotY = 2;//slot rows
	public float slotXpadding = 1;//as per slotsize 
	public float slotYpadding = 1;//as per slotsize 
	public Vector2 slotfineoffset; //for representing aftermath of all mousedrag
	public Vector2 slottempoffset; //for mouse drag calulcation
	public float slotsXstartposition = 0;
	public float slotsYstartposition = 0;
	public float slotsize = 20;
	public List<Item> inventory = new List<Item>();//to hold the actual item
	public List<Item> slots = new List<Item>();//used as proxy for inventory to display,and drag and dro
	public ItemDatabase database;
	public bool display = false;
	public bool showtooltip = false;
	public string tooltip = "";
	public GUISkin skin;
	//public bool testhit = false;
	public int fromindex = -1;
	public bool draggingitem = false;
	public Item itemdragged;
	public Vector2 mouseprevposition;
	//public int testdoubleclickcount = 0;
	
	// Use this for initialization
	void Start () {
	
		database = GameObject.FindGameObjectWithTag("Item Database").GetComponent<ItemDatabase>();//to input the script rference

		for (int i = 0; i<(slotX*slotY); ++i) //populate the slot list
		{
			slots.Add(new Item());
			inventory.Add(new Item());
		}

		AddItem (0);
		AddItem (1);
		AddItem (2);
		//AddItem (0);
		//AddItem (0);
		//		for(int i = 0 ; i <10; ++i)
		//		{
		//			inventory.Add(database.itemDatabase[0]);//add testing object
		//		}
		print(CheckContainsItem(0));
	}
	// Update is called once per frame
	// Update is called once per frame
	void Update () {
			if(Input.GetButtonDown("Inventory"))
			{
				ToggleDisplay();
			}
			if(Input.GetKeyDown("s"))
			{
				SaveInventory();
			}
			if(Input.GetKeyDown("l"))
			{
				LoadInventory();
			}
			
		}
		void SwapInventoryItem(int indexfrom,int indexto)
		{
			Item tempitem = inventory[indexfrom];
			inventory[indexfrom] = inventory[indexto];
			inventory[indexto] = tempitem;
		}
		public void AddItem(int id)
		{	
			for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
			{
				if(inventory[i].id < 0)//if there a slot is empty
				{
					for(int j = 0 ; j <database.itemDatabase.Count;++j)//loop through the item database
					{
						if(database.itemDatabase[j].id == id)//look for the matching item id 
						{
							if(database.itemDatabase[j].stackable == true)
							{
								//							Debug.Log("item is stackable");
								//							if(CheckContainsItem(id) == true)
								//							{
								//								Debug.Log("already have the item");
								//								++inventory[i].amount;
								//							}else
								//							{
								//								Debug.Log("dont have the item");
								//								inventory[i] = database.itemDatabase[j];//add it in
								//							}
								bool found = false;
								for(int k = 0 ; k < inventory.Count; ++k)//loop through whole inventory //not efficient,but cannot think of better alogrithm
								{
									if(inventory[k].id == id)//if there a slot is empty
									{
										++inventory[k].amount;
										found = true;
										break;
									}
								}
								if(!found)
								{
									inventory[i] = database.itemDatabase[j];//add it in
									break;
								}
								
							}else
							{
								Debug.Log("just add the item");
								inventory[i] = database.itemDatabase[j];//add it in
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
				--inventory[inventoryindex].amount;
				
				if(inventory[inventoryindex].amount <= 0)
				{
					inventory[inventoryindex] = new Item();
				}
			}else
			{
				inventory[inventoryindex] = new Item();
			}
		}
		void RemoveItem(int id)
		{
			for(int i = 0 ; i < inventory.Count; ++i)//loop through whole inventory
			{
				if(inventory[i].id == id)
				{
					RemoveKnownItem(id);
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
		void UseItem(Item item)
		{
			database.UseItemEffect(item.id);
			RemoveItem(item.id);
		}
		
		public void ToggleDisplay()
		{
			display = !display;
		}
		void OnGUI()
		{
			GUI.skin = skin;
			if(display == true)
			{
				//SimplePrintInventory();
				DrawInventory();
				if(showtooltip == true && draggingitem == false)
				{
					GUI.Box(new Rect(Event.current.mousePosition.x,Event.current.mousePosition.y,200,200),tooltip);
				}
				if(draggingitem == true)
				{
					GUI.DrawTexture(new Rect(Event.current.mousePosition.x,Event.current.mousePosition.y,50,50),itemdragged.icon);
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
		
		for (int y = 0; y < slotY; ++y) 
		{
			for (int x = 0; x < slotX; ++x) 
			{
				Rect slotRect = new Rect(x*slotsize*slotXpadding + slotsXstartposition + slotfineoffset.x +slottempoffset.x, y*slotsize*slotYpadding + slotsYstartposition + slotfineoffset.y+slottempoffset.y,slotsize,slotsize);
				//Rect slotRect = new Rect(x*60,y*60,50,50);
				GUI.Box(slotRect, y.ToString(),skin.GetStyle("slot"));
				
				slots[index] = inventory[index];//sync up
			
				if(currentevent.button == 1 && currentevent.type == EventType.mouseUp && draggingitem)//discarding item when leftclick and dragging item//temporary
				{				
					if(itemdragged.stackable == true ||  itemdragged.type == Item.ItemType.Consumable)
					{
						--itemdragged.amount;
						
						if(itemdragged.amount <= 0)
						{
							
							draggingitem = false;
							itemdragged = null;
						}
					}
					
				}

				if(slots[index].id >= 0)//if slot contain an valid item
				{
					GUI.DrawTexture(slotRect,slots[index].icon);
					
					if(slotRect.Contains(currentevent.mousePosition))//check if mouse hover over the box
					{
						
						//testhit = true;
						showtooltip = true;
						print("Index" + index);
						tooltip = CreateToolTip(slots[index]);
						
						
						if(currentevent.button == 0 &&currentevent.type == EventType.mouseDown && currentevent.clickCount == 2)//double click
						{
							//Debug.Log("double click");
							//++testdoubleclickcount;
							UseItem(slots[index]);
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
//				if(tooltip == "")
//				{
//					//print ("turning off tooltip");
//					showtooltip = false;
//				}
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
}
