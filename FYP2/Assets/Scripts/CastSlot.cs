using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script is to draw and make system of quickcast slot work
//concerns: this script can be combined into inventory script ,however due to concern of the code becoming messy,
//i decided to seperate them instead

[System.Serializable]
public class CastSlotList //workaround so that the list can be seralizabled
{
	public List<Item> slots = new List<Item>();

	public CastSlotList(int size)
	{
		for (int i = 0 ; i <size; ++i)
		{
			slots.Add(new Item());
		}
	}
}

[System.Serializable]
public class CastSlot : MonoBehaviour {

	//private List<Item> slots = new List<Item>(); //list of slot
	public List< CastSlotList > slotsLayerList = new List< CastSlotList >(); // list of  list of slot
	//public List< Item[] > testlist = new List< Item[]>[10];
	public bool display = true;
	public GUISkin skin = null;

	//internal storage management section
	public int fromindex = -1;
	public bool draggingitem = false;
	public Item itemdragged = null;
	public int currentSlotLayer = 0;

	//interface variable section
	public int maxSlotsColumn = 5;//amount of column
	public int maxSlotsLayer = 2;//amount of row
	public float slotsXpadding = 1.0f;//as per slotsize 
	public float slotSize = 50.0f;
	public float slotsXstartposition = 0.0f;
	public float slotsYstartposition = 0.0f;

	private Rect slotRect ;
	private Rect labelRect ;
	private GUIStyle labelStyle= null;
	//script reference
	public Inventory playerinventory = null;
	private Event currentevent = null;
	private Rect dragItemIconRect = new Rect();
	

//	// Use this for initialization
	void Start () {
		UpdateSlotStartingPosition();

		playerinventory = this.GetComponent<Inventory>();

		if (playerinventory == null)//if cannot find
		{
			//default to player inventory
			playerinventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
		}

		//maxSlotsLayer = CalculateMaximisedSlotLayerCount();

		for (int y = 0 ; y <maxSlotsLayer; ++y)
		{
			slotsLayerList.Add(new CastSlotList(maxSlotsColumn));
		}

		labelStyle = new GUIStyle(skin.GetStyle("slot"));
		
		labelStyle.fontSize = (int)slotSize;
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelRect = new Rect(slotsXstartposition - slotSize, slotsYstartposition ,slotSize,slotSize);
		dragItemIconRect.width = slotSize;
		dragItemIconRect.height = slotSize;
	}
	
	// Update is called once per frame
	void Update () {


		if(Input.GetKeyDown("tab"))
		{
			IncrementClampSlotLayer(1,true);
		}
		//Debug.Log("recieved key: " + Input.inputString);

	}

	void OnGUI()
	{		
		currentevent = Event.current;
		dragItemIconRect.position = currentevent.mousePosition;

		//blinding key 1 to 9 ,to the slot 1 to 9 and trigger the item effect
		if (currentevent.type == EventType.KeyDown) {
			if (currentevent.keyCode >= KeyCode.Alpha1
			    && currentevent.keyCode <= KeyCode.Alpha9) 
			{
				string output = currentevent.keyCode.ToString();
				int extractednum = int.Parse(""+output[output.Length-1]);
				if (extractednum > maxSlotsColumn)
				{
					extractednum = -1;
				}
				if (extractednum-1 >=0)
				{
					playerinventory.UseItem(slotsLayerList[currentSlotLayer].slots[extractednum-1]);
				}
			}
		}

		if (display == true)
		{
			GUI.skin = skin;

			DrawCastSlot();

			if(draggingitem == true)
			{
				if(itemdragged.displayDisableIcon == true)
				{
					GUI.DrawTexture(dragItemIconRect,itemdragged.disabledicon);
				}else{
					GUI.DrawTexture(dragItemIconRect,itemdragged.icon);
				}

			}
		}
	}

	void DrawCastSlot()
	{
		int index = 0;

		//labelRect = new Rect(slotsXstartposition - slotSize, slotsYstartposition ,slotSize,slotSize);
		GUI.Box(labelRect,(currentSlotLayer+1).ToString(),labelStyle);

		for (int x = 0; x < maxSlotsColumn; ++x) 
		{
			//Rect slotRect = new Rect(x*slotSize*slotsXpadding + slotsXstartposition ,slotsYstartposition ,slotSize,slotSize);
			slotRect = new Rect(x*slotSize*slotsXpadding + slotsXstartposition , slotsYstartposition ,slotSize,slotSize);
			GUI.Box(slotRect,(x+1).ToString(),skin.GetStyle("slot"));


			if(slotsLayerList[currentSlotLayer].slots[x].id >= 0)//if slot contain an valid item
			{

				if(slotsLayerList[currentSlotLayer].slots[x].displayDisableIcon == true)
				{
					if(slotsLayerList[currentSlotLayer].slots[x].disabledicon != null)
					{
						GUI.DrawTexture(slotRect,slotsLayerList[currentSlotLayer].slots[x].disabledicon);
					}
				}else
				{
					if(slotsLayerList[currentSlotLayer].slots[x].icon != null)
					{
						GUI.DrawTexture(slotRect,slotsLayerList[currentSlotLayer].slots[x].icon);
					}
				
				}


				if(slotRect.Contains(currentevent.mousePosition))//check if mouse hover over the box
				{
					if(currentevent.button == 0 &&currentevent.type == EventType.mouseDown && currentevent.clickCount == 2)//double click
					{
						//Debug.Log("double click");
						//++testdoubleclickcount;
						playerinventory.UseItem(slotsLayerList[currentSlotLayer].slots[x]);
					}
					if(currentevent.button == 0 && currentevent.type == EventType.mouseDrag && !draggingitem)//if left click and drag,and not currently dragging item
					{
						draggingitem = true;
						fromindex = index;
						itemdragged = slotsLayerList[currentSlotLayer].slots[x];//copy the item
						slotsLayerList[currentSlotLayer].slots[x] = new Item(); // delete the thing.
					}
					if(currentevent.type == EventType.mouseUp && draggingitem)//dragging an item and let go of mouse
					{
						slotsLayerList[currentSlotLayer].slots[fromindex] = slotsLayerList[currentSlotLayer].slots[x];
						slotsLayerList[currentSlotLayer].slots[x] = itemdragged;
						draggingitem = false;
						itemdragged = null;
					}
				}
			}else
			{
				if(slotRect.Contains(currentevent.mousePosition))//check if mouse hover over the box
				{
					if(currentevent.type == EventType.mouseUp && draggingitem)
					{
						slotsLayerList[currentSlotLayer].slots[x] = itemdragged;
						draggingitem = false;
						itemdragged = null;
					}
				}
			}

			++index;
		}

	}
	public Item GetItem(string item_name)
	{
		for (int y = 0 ; y<maxSlotsLayer; ++y )
		{
			for (int x = 0 ; x<maxSlotsColumn; ++x )
			{
				if(slotsLayerList[y].slots[x].ItemName == item_name)
				{
					return slotsLayerList[y].slots[x];
				}
			}
		}
		return null;
	}
	public Item GetItem(int item_id)
	{
		for (int y = 0 ; y<maxSlotsLayer; ++y )
		{
			for (int x = 0 ; x<maxSlotsColumn; ++x )
			{
				if(slotsLayerList[y].slots[x].id == item_id)
				{
					return slotsLayerList[y].slots[x];
				}
			}
		}
		return null;
	}
	public Item GetItem(Item a_item)
	{
		for (int y = 0 ; y<maxSlotsLayer; ++y )
		{
			for (int x = 0 ; x<maxSlotsColumn; ++x )
			{
				if(slotsLayerList[y].slots[x].id == a_item.id || slotsLayerList[y].slots[x].itemname == a_item.itemname)
				{
					return slotsLayerList[y].slots[x];
				}
			}
		}
		return null;
	}
	public bool CheckItemAlreadyAdded(int item_id)
	{
		for (int y = 0 ; y<maxSlotsLayer; ++y )
		{
			for (int x = 0 ; x<maxSlotsColumn; ++x )
			{
				if(slotsLayerList[y].slots[x].id == item_id)
				{
					return true;
				}
			}
		}

		return false;
	}

	public void AddItemKnown(Item a_item,int layerindex,int columnindex)//blind adding of item
	{
		slotsLayerList[layerindex].slots[columnindex] = a_item;
	}

	public void AddItem(Item a_item)
	{
		if( a_item.type != Item.ItemType.Consumable && a_item.type != Item.ItemType.Useable)
		{
			//Debug.Log("forbbiden add detected");
			return;
		}
		//Debug.Log("layer " + slotsLayerList.Count);
		//Debug.Log("column " + slotsLayerList[0].slots.Count);
		bool done = false;
		for (int y = 0 ; y<maxSlotsLayer; ++y )
		{
			if (done == true)
			{
				break;
			}
			for (int x = 0 ; x<maxSlotsColumn; ++x )
			{

				if(slotsLayerList[y].slots[x].id == a_item.id || slotsLayerList[y].slots[x].itemname == a_item.itemname  )
				{
					slotsLayerList[y].slots[x].displayDisableIcon = false;
					done = true;
					break;
				}

				if(slotsLayerList[y].slots[x].id < 0)//if slot contain an invalid item meaning empty slot
				{
					//a_item.displayDisableIcon = false;
					slotsLayerList[y].slots[x] = a_item;
					done = true;
					break;
				
				}
			}
		}
	}
	public void RemoveItem(int item_id)
	{
		//Debug.Log("removing item from quickcast");

		for (int y = 0 ; y<maxSlotsLayer;++y )
		{
			for (int x = 0 ; x<maxSlotsColumn;++x )
			{
				if(slotsLayerList[y].slots[x].id == item_id)
				{
					slotsLayerList[y].slots[x] = new Item();
					//Debug.Log("FOUND item to be removed from quickcast");
					break;
				}
			}
		}
	}
	
	public void RemoveItem(Item a_item)
	{
		//Debug.Log("removing item from quickcast");
		
		for (int y = 0 ; y<maxSlotsLayer;++y )
		{
			for (int x = 0 ; x<maxSlotsColumn;++x )
			{
				if(slotsLayerList[y].slots[x].id == a_item.id || slotsLayerList[y].slots[x].itemname == a_item.itemname )
				{
					slotsLayerList[y].slots[x] = new Item();
					//Debug.Log("FOUND item to be removed from quickcast");
					break;
				}
			}
		}
	}

	void IncrementClampSlotLayer(int step,bool warp)
	{
		if( step > 0)//positive step
		{
			if ( (currentSlotLayer +step) < maxSlotsLayer)
			{
				currentSlotLayer += step;
			}else
			{
				if (warp == true)
				{
					currentSlotLayer = 0;
				}else
				{
					currentSlotLayer = maxSlotsLayer-1;
				}

			}				
		}else if (step < 0)
		{
			if ( (currentSlotLayer +step) >= 0)
			{
				currentSlotLayer += step;
			}else
			{
				if (warp == true)
				{
					currentSlotLayer = maxSlotsLayer-1;
				}else
				{
					currentSlotLayer = 0;
				}
			}
		}
	}

	void UpdateSlotStartingPosition()//call this if the slotsize is changed realtime
	{
		slotsXstartposition = Screen.width  * 0.50f - (slotSize * maxSlotsColumn)*0.5f;
		slotsYstartposition = Screen.height * 0.95f - slotSize;
	}
	int CalculateMaximisedSlotColumnCount()
	{
		return playerinventory.inventory.Count / maxSlotsLayer;
	}
	int CalculateMaximisedSlotLayerCount()
	{
		return playerinventory.inventory.Count / maxSlotsColumn;
	}

	public void ToggleDisplay()
	{
		display = !display;
	}
}
