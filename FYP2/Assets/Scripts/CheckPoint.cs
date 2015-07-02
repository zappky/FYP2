using UnityEngine;
using System.Collections;

//this script is to describle what is a checkpoint, a game object should attach this script to be indicated as a checkpoint instead of being tag as checkpoint
public class CheckPoint : MonoBehaviour {

	public string checkPointName = "";
	public int id = -1;
	public bool playerEnteredCheckPoint = false;
	public bool playerStayedCheckPoint = false;
	public bool playerExitedCheckPoint = false;

	public enum CheckPoint_Type
	{
		CHECKPOINT_NONE,
		CHECKPOINT_DIALOG_TRIGGER,
		CHECKPOINT_SAVE_TRIGGER,
		CHECKPOINT_TOTAL
	}

	public CheckPoint_Type checkpointType = CheckPoint_Type.CHECKPOINT_NONE;
	// Use this for initialization
	void Start () {
		this.checkPointName = this.name;
	}

	void Update()
	{
	}

	public CheckPoint()
	{

	}

	public CheckPoint (CheckPoint another)
	{
		this.checkPointName = another.checkPointName;
		this.id = another.id;
	}

	public void RepositionPlayerAt(GameObject playerobj)
	{
		playerobj.transform.position = this.transform.position;
	}

	void OnCollisionEnter(Collision other)
	{
		print ("checking coliison enter");
		print ("collision game object name " +  other.gameObject.name);

		if(other.gameObject.tag == "Player")
		{
			playerEnteredCheckPoint = true;
			playerStayedCheckPoint = false;
			playerExitedCheckPoint = false;
		}
	}
	void OnCollisionStay (Collision other)
	{
		print ("checking coliison stay ");
		print ("collision game object name " +  other.gameObject.name);
		if(other.gameObject.tag == "Player")
		{
			playerEnteredCheckPoint = false;
			playerStayedCheckPoint = true;
			playerExitedCheckPoint = false;
		}
	}
	void OnCollisionExit (Collision other)
	{
		print ("checking coliison exit");
		print ("collision game object name " +  other.gameObject.name);
		if(other.gameObject.tag == "Player")
		{
			playerEnteredCheckPoint = false;
			playerStayedCheckPoint = false;
			playerExitedCheckPoint = true;
		}
	}
}
