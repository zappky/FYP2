﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//this script is to hold all of the dialog script of all npc. a central place of dialog script enquiry
public class DialogDatabase : MonoBehaviour {

	public List<DialogTree> dialogDatabase = new List<DialogTree>();//the index of the list of dialog tree is used as levels
	public static DialogDatabase instance = null;
	public static bool initedBefore = false;

	public static DialogDatabase Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("Dialog Database").AddComponent<DialogDatabase>();
				DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}
	public DialogTree this[int index]
	{
		get
		{
			return dialogDatabase[index];
		}
	}
	public void Initialize()
	{
		Initialize(false);//don allow reinitalize of this class by default
	}

	public void Initialize(bool re_init)
	{
		if(initedBefore == false || re_init == true)
		{
			LoadDialogTreeDatas();
			initedBefore = true;
		}
	}

	public void OnApplicationQuit()
	{
		SaveDialogTreeDatas();
		DestroyInstance();
	}

	public void DestroyInstance()
	{
		SaveDialogTreeDatas();
		instance = null;
	}
	public void SaveDialogTreeDatas()
	{
		FileManager.Instance.SaveDialogDatabase();
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
					if(a_dialogoption.nextDialogId < 0)
					{
						a_dialogoption.nextDialog = null;
					}

					my_DialogNode thenextdialognode = templist[i].GetDialog(a_dialogoption.nextDialogId);
					//Debug.Log("Suspect: " + a_dialogoption.nextDialogId);
					a_dialogoption.nextDialog = thenextdialognode;
				}
				
			}
			
			dialogDatabase.Add(templist[i]);
		}
	}
	public my_DialogNode GetBookmarkDialogNode(string gameLevelParentName , string bookmark_node_name)
	{
		return GetDialogTree(gameLevelParentName).GetDialogBookmarked(bookmark_node_name);
	}
	
	public my_DialogNode GetBookmarkDialogNode(int dialogTreeId , string bookmark_node_name)
	{
		return GetDialogTree(dialogTreeId).GetDialogBookmarked(bookmark_node_name);
	}

	public my_DialogNode GetBookmarkDialogNode(string gameLevelParentName , int bookmark_node_id)
	{
		return GetDialogTree(gameLevelParentName).GetDialogBookmarked(bookmark_node_id);
	}
	
	public my_DialogNode GetBookmarkDialogNode(int dialogTreeId , int bookmark_node_id)
	{
		return GetDialogTree(dialogTreeId).GetDialogBookmarked(bookmark_node_id);
	}

	public my_DialogNode GetDialogNode(string gameLevelParentName , int node_id)
	{
		return GetDialogNode(GetDialogTree(gameLevelParentName),node_id);
	}

	public my_DialogNode GetDialogNode(int dialogTreeId , int node_id)
	{
		return GetDialogNode(GetDialogTree(dialogTreeId),node_id);
	}

	public my_DialogNode GetDialogNode(DialogTree dialogTree , int node_id)
	{
		if(node_id <0)
		{
			Debug.Log("ERROR: GetDialogNode id is negative search ");
			return null;
		}

		if(node_id < dialogTree.dialogs.Count)
		{
			if(node_id == dialogTree.dialogs[node_id].nodeId)
			{
				return dialogTree.dialogs[node_id];
			}
			//if still cannot be found,brute force search
			for(int i = 0 ; i < node_id; ++i)
			{
				if(dialogTree.dialogs[i].nodeId == node_id)
				{
					return dialogTree.dialogs[i];
				}
			}
			for(int i = node_id+1 ; i < dialogTree.dialogs.Count; ++i)
			{
				if(dialogTree.dialogs[i].nodeId == node_id)
				{
					return dialogTree.dialogs[i];
				}
			}

		}else
		{
			Debug.Log("WARNING: GetDialogNode id is over list size search,brute force search will be performed");
			for(int i = 0; i < dialogTree.dialogs.Count; ++i)
			{
				if(dialogTree.dialogs[i].nodeId == node_id)
				{
					return dialogTree.dialogs[i];
				}
			}
		}


		return null;
	}

	public DialogTree GetDialogTree(string gameLevelName)
	{
		for(int i = 0 ; i < dialogDatabase.Count; ++i)
		{
			if(dialogDatabase[i].gameLevelParent == gameLevelName)
			{
				return dialogDatabase[i];
			}
		}
		Debug.Log("ERROR: Get Dialog tree -  cannot find tree with the name: " + gameLevelName);
		return null;
	}

	public DialogTree GetDialogTree(int id)
	{	
		if(id <0)
		{
			Debug.Log("ERROR: GetDialogTree id is negative search ");
			return null;
		}
		
		if(id < dialogDatabase.Count)
		{
			if(dialogDatabase[id].id == id)
			{
				return dialogDatabase[id];
			}
			//if still cannot be found

			for(int i = 0 ; i <id;++i)
			{
				if(dialogDatabase[i].id == id)
				{
					return dialogDatabase[i];
				}
			}
			for(int i = id+1 ; i <id;++i)
			{
				if(dialogDatabase[i].id == id)
				{
					return dialogDatabase[i];
				}
			}
		}else
		{
			Debug.Log("WARNING: GetDialogTree id is over list size search,brute force search will be performed");
			foreach(DialogTree a_tree in dialogDatabase)
			{
				if(a_tree.id == id)
				{
					return a_tree;
				}
			}
		}
		Debug.Log("ERROR: GetDialogTree cannot find dialogtree with: " + id);
		return null	;	
	}

}
