using UnityEngine;
using System.Collections;

//this script is suppose to attached to all interactable game object to toggle effect,I.E picking up item
public class Interactable : MonoBehaviour {

	private Inventory playerInventory;
	private VendorDatabase vendordatabase;

	// Use this for initialization
	void Start () {
		playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
		vendordatabase = VendorDatabase.Instance;
	}
	
	// Update is called once per frame
	void Update () {
	
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
