using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
public class my_DialogOption
{
	public string text = "";//the text to be displayed
	public int returnStatus = 0;// zero means neutral, 1 is postive, -1 is negative respond
	
	public my_DialogNode nextDialog = null;//the next node
	public int nextDialogId = -1;//for xml loading purpose
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

	public List<my_DialogNode> dialogs =  new List<my_DialogNode>();
	
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

	public my_DialogNode FindDialog(int nodeid)
	{
		if( nodeid >= 0 && nodeid < dialogs.Count && dialogs[nodeid].nodeId == nodeid)//early test
		{
			return dialogs[nodeid];
		}else
		{
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
	public my_DialogNode GetDialog(int index)
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
	public void AddDialog(my_DialogNode a_dialog)
	{
		if(a_dialog.nodeId < 0)
		{
			a_dialog.nodeId = dialogs.Count;
		}
		dialogs.Add(a_dialog);
	}

}
