using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class my_VendorEntry
{
	public int id = -1;
	public string vendorName = "";
	public List<Item_Proxy> outputItemList = new List<Item_Proxy>();

	public my_VendorEntry()
	{
	}
	public my_VendorEntry(int vendorid,string vendorname)
	{
		this.id = vendorid;
		this.vendorName = vendorname;
	}
	public my_VendorEntry(int vendorid)
	{
		this.id = vendorid;
	}
	public my_VendorEntry(string vendorname)
	{
		this.vendorName = vendorname;
	}
	public my_VendorEntry(my_VendorEntry another)
	{
		this.vendorName = another.vendorName;
		this.id = another.id;

		this.outputItemList.Clear();

		foreach( Item_Proxy item in another.outputItemList)
		{
			this.outputItemList.Add(item);
		}
	}
	public void AddOutputItem(int id, string name,int amount)
	{
		this.outputItemList.Add(new Item_Proxy(id,name,amount));
	}
}

public class VendorDatabase : MonoBehaviour {

	public static VendorDatabase instance = null;
	public List<my_VendorEntry> vendorList = new List<my_VendorEntry>();

	public static VendorDatabase Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("Vendor Database").AddComponent<VendorDatabase>();
				DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}

	public void Initialize()
	{	
		LoadVendorsData();
	}

	public void LoadVendorsData()
	{
		this.vendorList = FileManager.Instance.LoadVendorsData();
	}

	public void OnApplicationQuit()//this will be auto called like start and update function
	{
		print ("vendor database get quited");
		FileManager.Instance.SaveVendorDatabase();
		DestroyInstance();
	}
	public void DestroyInstance()
	{
		instance = null;
	}

	public my_VendorEntry GetVendor(int id)
	{
		foreach  (my_VendorEntry item in vendorList)
		{
			if(item.id == id)
			{
				return item;
			}
		}
		return null;
	}
	public my_VendorEntry GetVendor(string vendorname)
	{
		foreach  (my_VendorEntry item in vendorList)
		{
			if(item.vendorName == vendorname)
			{
				return item;
			}
		}
		return null;
	}
}
