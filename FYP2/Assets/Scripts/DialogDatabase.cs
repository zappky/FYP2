using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//this script is to hold all of the dialog script of all npc. a central place of dialog script enquiry
public class DialogDatabase : MonoBehaviour {

	public List<DialogTree> dialogDatabase = new List<DialogTree>();//the index of the list of dialog tree is used as levels
	public static DialogDatabase instance = null;

	public static DialogDatabase Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("Dialog Database").AddComponent<DialogDatabase>();
			}
			return instance;
		}
	}

	public void Initialize()
	{
		LoadDialogTreeDatas();
	}

	public void OnApplicationQuit()
	{
		DestroyInstance();
	}

	public void DestroyInstance()
	{
		instance = null;
	}

	public void LoadDialogTreeDatas()
	{
		List<DialogTree>templist = FileManager.Instance.LoadDialogTreeData();//extract out the readied list of dialog tree from file manager
		for (int i = 0 ; i <templist.Count ; ++i)//do some preparation work before inserting into the real database
		{
			
			//link up each of the dialog option's next dialog node pointer in each dialog node.
			foreach (my_DialogNode a_dialognode in templist[i].dialogs)
			{
				foreach (my_DialogOption a_dialogoption in a_dialognode.options)
				{
					my_DialogNode thenextdialognode = templist[i].FindDialog(a_dialogoption.nextDialogId);
					a_dialogoption.nextDialog = thenextdialognode;
				}
				
			}
			
			dialogDatabase.Add(templist[i]);
		}
	}

	public DialogTree GetDialogTreeWithIndex(int index)
	{	
		if(index >= 0 && index < dialogDatabase.Count)
		{
			return dialogDatabase[index];
		}	
		Debug.Log("ERROR: Get Dialog tree index,array out of range");
		return null	;	
	}
	public DialogTree GetDialogTreeWithLevel(int level)
	{
		level--;

		if(level >= 0 && level < dialogDatabase.Count)
		{
			return dialogDatabase[level];
		}

		return null;

	}
}
