using UnityEngine;
using System.Collections;

//this script is suppose to attached to all interactable game object to toggle effect,I.E picking up item
public class Interactable : MonoBehaviour {

	private Inventory playerInventory = null;
	private VendorDatabase vendordatabase = null;
	private QuestManager questManager = null;	
	bool lighting = true;
	public my_QuestLog questReference = null;
	ParticleSystem particles;

	void Start () {
		playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
		vendordatabase = VendorDatabase.Instance;
		questManager = QuestManager.Instance;
		particles = gameObject.GetComponentInChildren<ParticleSystem>();

		switch(this.name)
		{
			case "BoxofPaperclips":
				questReference = questManager.questLogListDatabase.GetQuestLog("Collect Paper Clip to Fix Grapple");
				break;
				
			case "Yarn":
				questReference = questManager.questLogListDatabase.GetQuestLog("Get Some Strings");
				break;
				
			case "TearableBook":
				questReference = questManager.questLogListDatabase.GetQuestLog("Tear Book Page for Paper Pieces");
				break;
				
			case "CakeBox":
				questReference = questManager.questLogListDatabase.GetQuestLog("Grab the... Cake?");
				break;

			default:
				break;
		}

		if(questReference != questManager.GetCurrentQuest())	
			SetLighting(false);
	}

	void Update () {
		if(!lighting && questReference == questManager.GetCurrentQuest())	
			SetLighting(true);
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

			switch(this.name)
			{
				case "BoxofPaperclips":
					DialogInterface.Instance.StartNewDialogSessionUsingBookmark(LevelManager.Instance.CurrentLevelName, 5); 
					break;

				case "Yarn":
					DialogInterface.Instance.StartNewDialogSessionUsingBookmark(LevelManager.Instance.CurrentLevelName, 10); 
					break;

				case "TearableBook":
					break;
				
				case "CakeBox":
					DialogInterface.Instance.StartNewDialogSessionUsingBookmark(LevelManager.Instance.CurrentLevelName, 1); 
				break;
			}
			SetLighting(false);
		}
		else
		{
			Debug.Log(this.name + "has null quest reference");
		}

	}
	public void SetLighting(bool lighting)
	{
		if(particles != null)
		{
			this.lighting = lighting;
			particles.enableEmission = lighting;
		}
		else
		{
			Debug.Log(this.name + "has null particleRender reference");

			if(this.name == "Yarn")
			{
				transform.parent.GetComponentInChildren<ParticleSystem>().enableEmission = false;
			}
		}

	}
	public void ToggleLighting()
	{
		if(particles != null)
		{
			this.lighting = !this.lighting;
			particles.enableEmission = lighting;
		}
		else
		{
			Debug.Log(this.name + "has null particleRender reference");
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
