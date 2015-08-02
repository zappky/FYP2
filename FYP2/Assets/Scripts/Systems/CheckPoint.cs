using UnityEngine;
using System.Collections;

//this script is to describle what is a checkpoint, a game object should attach this script to be indicated as a checkpoint instead of being tag as checkpoint
[System.Serializable]
public class CheckPoint : MonoBehaviour {

	public int id = -1;
	public int indexInList = -1;
	public int orderPlacement = -1;
	public LevelManager levelManager = null;

	bool lighting = false;
	my_QuestLog questReference = null;
	ParticleSystem particles = null;

	public enum CheckPoint_Type
	{
		CHECKPOINT_NONE,
		CHECKPOINT_DIALOG_TRIGGER,
		CHECKPOINT_SAVE_TRIGGER
	}

	public CheckPoint_Type checkpointType = CheckPoint_Type.CHECKPOINT_NONE;
	// Use this for initialization
	void Start () {
		levelManager = LevelManager.Instance;
		
		particles = gameObject.GetComponentInChildren<ParticleSystem>();

		if(particles != null)
		{
			SetLighting(false);
			// Level 1
			if(levelManager.CurrentLevelName == "Level1")
				questReference = QuestManager.Instance.questLogListDatabase.GetQuestLog("Reach the Door");
		}
	}

	void Update()
	{
		//print (QuestManager.Instance.GetCurrentQuest().questname);

		if(particles != null)
		{
			if(!lighting && questReference == QuestManager.Instance.GetCurrentQuest())
				SetLighting(true);
		}
	}

	public CheckPoint()
	{
	}

	public CheckPoint (CheckPoint another)
	{
		this.id = another.id;
		this.indexInList = another.indexInList;
		this.orderPlacement = another.orderPlacement;
	}
	
	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			// dont save if player in air or falls from a great ht
			if(!other.gameObject.GetComponent<CharacterController>().isGrounded
			|| other.gameObject.GetComponent<FallResponse>().sfxPlayed)	
				return;

			if(indexInList > levelManager.currentCheckPointIndex  )//check if the current collided checkpoint is further than the last checkpoint
			{
				levelManager.currentCheckPointIndex = indexInList;

				switch (this.checkpointType)
				{
					case CheckPoint_Type.CHECKPOINT_DIALOG_TRIGGER:
						Debug.Log("Triggering dialog trigger: " +this.name );
						levelManager.playerdialoginferface.StartNewDialogSessionUsingBookmark(levelManager.CurrentLevelName,this.name);
						levelManager.SavePlayerInfo();
						break;
						
					case CheckPoint_Type.CHECKPOINT_SAVE_TRIGGER:
						levelManager.SavePlayerInfo();
						break;
						
					case CheckPoint_Type.CHECKPOINT_NONE://dont care
						break;
						
					default:
						Debug.Log("ERROR: Unhandled checkpoint type trigger : " + this.checkpointType.ToString());
						break;
				}
			}

		}
	}

	public void SetLighting(bool lightUp)
	{
		if(particles == null)
			return;

		this.lighting = lightUp;
		particles.enableEmission = lighting;
	}
}
