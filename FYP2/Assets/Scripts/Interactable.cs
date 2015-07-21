using UnityEngine;
using System.Collections;

//this script is suppose to attached to all interactable game object to toggle effect,I.E picking up item
public class Interactable : MonoBehaviour {

	private Inventory playerInventory = null;
	private VendorDatabase vendordatabase = null;
	private QuestManager questManager = null;	
	public bool lighting = true;
	public my_QuestLog questReference = null;
	private Behaviour theHalo = null;
	// Use this for initialization
	void Start () {
		playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
		vendordatabase = VendorDatabase.Instance;
		questManager = QuestManager.Instance;

		switch(this.name)
		{
			case "testquest 1":
				questReference = questManager.questLogListDatabase.GetQuestLog("Collect Paper Clips");
			break;
			case "testquest 2":
				questReference = questManager.questLogListDatabase.GetQuestLog("xmltestquest2");
			break;
			case "testquest 3":
				questReference = questManager.questLogListDatabase.GetQuestLog("xmltestquest3");
			break;
			case "testquest 4":
				questReference = questManager.questLogListDatabase.GetQuestLog("xmltestquest4");
			break;
			case "testquest 5":
			questReference = questManager.questLogListDatabase.GetQuestLog("xmltestquest5");
			break;

			case "BoxofPaperclips":
				theHalo = ((Behaviour)GetComponent("Halo"));//not all object with interact script will use halo 
				questReference = questManager.questLogListDatabase.GetQuestLog("Collect Paper Clip");
				break;
			default:
				break;
		}

	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void CompleteQuest()
	{
		if(questReference != null )
		{
			if(questReference != questManager.GetCurrentQuest())
			{
				Debug.Log("WARNING: Attempting to clear quest that is not current quest");
				return;
			}
			Debug.Log("clearing quest " + questReference.questname);
			questManager.ClearQuest(questReference);
			SetLighting(false);
		}else
		{
			Debug.Log(this.name + "has null quest reference");
		}

	}
	public void SetLighting(bool lighting)
	{
		if(theHalo != null)
		{
			this.lighting = lighting;
			theHalo.enabled = lighting;
		}else
		{
			Debug.Log(this.name + "has null halo reference");
		}

	}
	public void ToggleLighting()
	{
		if(theHalo != null)
		{
			this.lighting = !this.lighting;
			theHalo.enabled = lighting;
		}else
		{
			Debug.Log(this.name + "has null halo reference");
		}
		this.lighting = !this.lighting;
	}
	//for picking up effect
	public void SpawnItemWithName()//add item using name
	{
		foreach (my_VendorEntry item in vendordatabase.vendorList)
		{
			if(item.vendorName == this.name)//find the corresponding vendor name
			{
				//produce and add the item to inventory
				foreach (Item_Proxy itemcontent in item.outputItemList)
				{
					for(int i = 0 ; i <itemcontent.amount; ++i)
					{
						playerInventory.AddItem(itemcontent.itemname);
					}
				}
				break;
			}
		}
	}

	//for picking up effect
	public void SpawnItemWithId()//add item using id
	{
		foreach (my_VendorEntry item in vendordatabase.vendorList)
		{
			if(item.vendorName == this.name)//find the corresponding vendor name
			{
				//produce and add the item to inventory
				foreach (Item_Proxy itemcontent in item.outputItemList)
				{
					for(int i = 0 ; i <itemcontent.amount; ++i)
					{
						playerInventory.AddItem(itemcontent.itemid);
					}
				}
				break;
			}
		}
	}
	public void SpawnItem()
	{
		SpawnItem(true);
	}
	//for picking up effect
	public void SpawnItem(bool preferNameAsIdentifier)//preferred default call, prority is given to add item using id first.
	{
		print ("spawning item");
		foreach (my_VendorEntry item in vendordatabase.vendorList)
		{
			if(item.vendorName == this.name)//find the corresponding vendor name
			{
				//produce and add the item to inventory
				foreach (Item_Proxy itemcontent in item.outputItemList)
				{
					for(int i = 0 ; i <itemcontent.amount; ++i)
					{
						if(preferNameAsIdentifier == true)
						{
							if(playerInventory.AddItem(itemcontent.itemname) == false)
							{
								if(playerInventory.AddItem(itemcontent.itemid) == false)
								{
									SpawnItemBlind();

								}
							}
						}else
						{
							if(playerInventory.AddItem(itemcontent.itemid) == false)
							{
								if(playerInventory.AddItem(itemcontent.itemname) == false)
								{
									SpawnItemBlind();

								}
							}
						}

					}
				}
				break;
			}
		}
	}
	//for picking up effect
	public void SpawnItemBlind()//add item to player inventory based on gameobject name// not dymanic enough
	{
		playerInventory.AddItem(this.name);
	}
}
