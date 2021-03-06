﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class my_DialogBookmark
{
	public string bookmarkName = "";
	public int id = -1;
	public my_DialogNode bookmarkedDialogNode = null;

	public my_DialogBookmark()
	{

	}
	public my_DialogBookmark(int id, my_DialogNode nodeToBookmark)
	{
		this.id = id;
		this.bookmarkedDialogNode = nodeToBookmark;
	}
	public my_DialogBookmark(string bookmarkname, my_DialogNode nodeToBookmark)
	{
		this.bookmarkName = bookmarkname;
		this.bookmarkedDialogNode = nodeToBookmark;
	}
	public my_DialogBookmark(int id,string bookmarkname, my_DialogNode nodeToBookmark)
	{
		this.id = id;
		this.bookmarkName = bookmarkname;
		this.bookmarkedDialogNode = nodeToBookmark;
	}

}
[System.Serializable]
public class my_DialogOption
{
	public string text = "";//the text to be displayed
	public int returnStatus = 0;// zero means neutral, 1 is postive, -1 is negative respond
	
	public my_DialogNode nextDialog = null;//the next node
	public int nextDialogId = -1;//for xml loading purpose//preferably dont use it for any checking outside of initlization
	public my_DialogNode parentDialog = null;// the node which the option is attached to
	
	public my_DialogOption()
	{	
	}

	public my_DialogOption(my_DialogOption another)
	{	
		this.text = another.text;
		this.returnStatus = another.returnStatus;
		this.nextDialog = another.nextDialog;
		this.nextDialogId = another.nextDialogId;
		this.parentDialog =  another.parentDialog;
	}

	//for xml purpose
	public my_DialogOption(string text , int returnstatus,int nextdialogid)
	{	
		this.text = text;
		this.returnStatus = returnstatus;
		this.nextDialogId = nextdialogid;
	}
	
	public my_DialogOption(string text , int returnstatus)
	{	
		this.text = text;
		this.returnStatus = returnstatus;
	}


	public my_DialogOption(string text , int returnstatus,my_DialogNode nextDialog)
	{	
		this.text = text;
		this.returnStatus = returnstatus;
		this.nextDialog = nextDialog;
	}

	public my_DialogOption(string text , int returnstatus,my_DialogNode nextDialog,my_DialogNode parentDialog)
	{	
		this.text = text;
		this.returnStatus = returnstatus;
		this.nextDialog = nextDialog;
		this.parentDialog = parentDialog;
	}
}
[System.Serializable]
public class my_DialogNode
{
	//***Note*** first index of options list will be reserved for linking dialog node with zero or one option to another node //
	//so when creating new dialognode, please always provide at least one dialog option as linkage to next node,unless it is the end of dialog tree.
	public int nodeId = -1; // the index of the node to ease tracking of the progress
	public string actorName = "";// the npc which is saying the dialog
	public string text = ""; // the text of the dialog
	//public my_DialogNode nextDialog = null;//the next node
	//public my_DialogNode parentDialog = null;// the node which before this
	public List<my_DialogOption> options = new List<my_DialogOption>(); // a list of dialog option

	public my_DialogNode()
	{
	}

	public my_DialogNode(my_DialogNode another)
	{
		this.text = another.text;
		this.actorName = another.actorName;
		this.options = another.options;
		//this.parentDialog = another.parentDialog;
		//this.nextDialog = another.nextDialog;
		this.nodeId = another.nodeId;
	}
	
	//marking the end of the node
	public my_DialogNode(string text, string actorName)
	{
		this.text = text;
		this.actorName = actorName;
	}

	//marking single forward conversation node
	public my_DialogNode(string text, string actorName,my_DialogOption singleOption)
	{
		this.text = text;
		this.actorName = actorName;
		options.Add(singleOption);
	}

	//marking multi direction conversation node
	public my_DialogNode(string text, string actorName,List<my_DialogOption> Options)
	{
		this.text = text;
		this.actorName = actorName;

		foreach (my_DialogOption item in Options)
		{
			this.options.Add(item);
		}
	}

	public void AddDialogOption(my_DialogOption a_option)
	{
		a_option.parentDialog = this;
		this.options.Add(a_option);
	}
}


//this script is to be the data structure of npc dialogs
[System.Serializable]
public class DialogTree : IEnumerable<my_DialogNode>
{
	public int id = -1;
	public List<my_DialogNode> dialogs =  new List<my_DialogNode>();
	public List<my_DialogBookmark> dialogSections =  new List<my_DialogBookmark>();//bookmark of each unique section of dialogs
	public string gameLevelParent = ""; // identifier of where this dialog tree is suppose to be at.//follo the currentlevelname
	
	public IEnumerator<my_DialogNode> GetEnumerator()
	{
		return dialogs.GetEnumerator();
	}
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return dialogs.GetEnumerator();
	}
	
	public DialogTree()
	{
		//InitTestData();
	}

	public my_DialogNode this[int index]
	{
		get
		{
			return dialogs[index];
		}
	}
	public my_DialogNode GetDialogBookmarkedWithIndex(int index)
	{
		return dialogSections[index].bookmarkedDialogNode;
	}
	public my_DialogNode GetDialogBookmarked(string name)
	{
		foreach (my_DialogBookmark a_bookmark in dialogSections)
		{
			if(a_bookmark.bookmarkName == name)
			{
				return a_bookmark.bookmarkedDialogNode;
			}
		}
		Debug.Log("ERROR: GetDialogBookmarked cannot find with: " + name);
		return null;
	}
	public my_DialogNode GetDialogBookmarked(int id)
	{
		if(id <0)
		{
			Debug.Log("ERROR: GetDialogBookmarked id search is negative value");
			return null;
		}

		if(id < dialogSections.Count)
		{
			if(dialogSections[id].id == id)
			{
				return dialogSections[id].bookmarkedDialogNode;
			}
			//if still cannot be found,brute force search
			for(int i = 0 ; i <id ; ++i )
			{
				if(dialogSections[i].id == id)
				{
					return dialogSections[i].bookmarkedDialogNode;
				}
			}
			for(int i = id+1 ; i <dialogSections.Count ; ++i )
			{
				if(dialogSections[i].id == id)
				{
					return dialogSections[i].bookmarkedDialogNode;
				}
			}
		}else
		{
			Debug.Log("WARNING: GetDialogBookmarked id search is over list size,brute force search will be performed");
			for(int i = 0 ; i <dialogSections.Count ; ++i )
			{
				if(dialogSections[i].id == id)
				{
					return dialogSections[i].bookmarkedDialogNode;
				}
			}
		}


		Debug.Log("ERROR: GetDialogBookmarked cannot find with: " + id);
		return null;
	}
	public my_DialogNode GetDialog(int nodeid)
	{
		if(nodeid < 0)
		{
			Debug.Log("ERROR: GetDialog with id is negative value : " + nodeid);
			return null;
		}
		if(nodeid < dialogs.Count)
		{
			if(dialogs[nodeid].nodeId == nodeid)
			{
				return dialogs[nodeid];
			}
			//if still cannot be found,brute force search
			for(int i = 0 ; i < nodeid ; ++i)
			{
				if(dialogs[i].nodeId == nodeid)
				{
					return dialogs[i];
				}
			}
			for(int i = nodeid+1 ; i < dialogs.Count ; ++i)
			{
				if(dialogs[i].nodeId == nodeid)
				{
					return dialogs[i];
				}
			}
		}else
		{
			Debug.Log("WARNING: GetDialog with id is over list size,brute force search will be performed : " + nodeid);
			for(int i = 0 ; i < dialogs.Count ; ++i)
			{
				if(dialogs[i].nodeId == nodeid)
				{
					return dialogs[i];
				}
			}
		}



		return null;
	}
	public my_DialogNode GetDialogWithIndex(int index)
	{
		if(index >= 0 && index < dialogs.Count)
		{
			return dialogs[index];
		}
		Debug.Log("ERROR: Get dialog node array out of range");
		return null;
	}

	public bool isEmpty()
	{
		if(dialogs.Count == 0)
		{
			return true;
		}

		return false;
	}
	public my_DialogNode RootNode 
	{
		get
		{
			if (isEmpty() == false)
			{
				return dialogs[0];
			}
			return null;
		}
	}
	public void AddDialogBookmark(my_DialogNode a_dialog)
	{
		dialogSections.Add(new my_DialogBookmark(dialogSections.Count,dialogSections.Count.ToString(),a_dialog));
	}
	public void AddDialog(my_DialogNode a_dialog)
	{
		if(a_dialog.nodeId < 0)
		{
			a_dialog.nodeId = dialogs.Count;
		}
		dialogs.Add(a_dialog);
	}

}
