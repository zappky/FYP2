using UnityEngine;
using System.Collections;

//this script is suppose to attached to all interactable game object to toggle effect,I.E picking up item
public class Interactable : MonoBehaviour {

	private Inventory playerInventory = null;
	private VendorDatabase vendordatabase = null;
	private QuestManager questManager = null;	
	public bool lighting = true;
	public my_QuestLog questReference = null;
	public ParticleRenderer particleRender = null;

	void Start () {
		playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
		vendordatabase = VendorDatabase.Instance;
		questManager = QuestManager.Instance;
		particleRender = this.gameObject.GetComponentInChildren<ParticleRenderer>();

		switch(this.name)
		{
			case "BoxofPaperclips":
			questReference = questManager.GetQuestLog("Collect Paper Clip to Fix Grapple");
				break;
			
			case "Yarn":
				// need collect max amt (till full inventory) of strings to fin quest 
				questReference = questManager.GetQuestLog("Get Some Strings");
				break;
			
			case "TearableBook":
				questReference = questManager.GetQuestLog("Tear Book Page for Paper Pieces");
				break;

			default:
				break;
		}

	}

	void Update () {
	}
	
	public void CompleteQuest()
	{
		if(questReference != null)
		{
			questReference.statues = true;
			SetLighting(false);
		}
		else
		{
			Debug.Log(this.name + "has null quest reference");
		}

	}
	public void SetLighting(bool lighting)
	{
		if(particleRender != null)
		{
			this.lighting = lighting;
			particleRender.enabled = lighting;
		}
		else
		{
			Debug.Log(this.name + "has null particleRender reference");
		}

	}
	public void ToggleLighting()
	{
		if(particleRender != null)
		{
			this.lighting = !this.lighting;
			particleRender.enabled = lighting;
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
