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
	public static bool initedBefore = false;
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
		Initialize(false);// dont allow reinitalize of this class by default
	}
	
	public void Initialize(bool re_init)
	{
		if(initedBefore == false || re_init == true)
		{
			LoadVendorsData();
			initedBefore = true;
		}
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
		if(id < 0)
		{
			Debug.Log("ERROR: GetVendor with id is negative value");
			return null;
		}
		if(id < vendorList.Count)
		{
			//early prediction
			if(vendorList[id].id == id)
			{
				return vendorList[id];
			}
			//if still cannot be found,brute force search
			for(int i = 0 ; i < id; ++i)
			{
				if(vendorList[i].id == id)
				{
					return vendorList[i];
				}
			}
			for(int i = id+1 ; i < vendorList.Count; ++i)
			{
				if(vendorList[i].id == id)
				{
					return vendorList[i];
				}
			}
		}else
		{
			Debug.Log("WARNING: GetVendor with id is over list size,brute force will be performed");
			for(int i = 0 ; i < vendorList.Count; ++i)
			{
				if(vendorList[i].id == id)
				{
					return vendorList[i];
				}
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
