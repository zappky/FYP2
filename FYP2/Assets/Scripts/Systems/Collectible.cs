using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {

	public LevelManager levelManager = null;
	public my_CollectibleList currentCollectibleLevel = null;

	// Use this for initialization
	void Start () {
		levelManager = LevelManager.Instance;
		currentCollectibleLevel = levelManager.achievementmanager.GetCollectibleLevel(levelManager.CurrentLevelName);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider other)
	{
		switch(other.gameObject.tag)
		{
		case "Player":
			if(currentCollectibleLevel.DuplicateCollectibleCheck(this.name) == false)
			{
				levelManager.achievementmanager.AddCollectible(levelManager.CurrentLevelName,new my_Collectible(this.name));
				Destroy(this.gameObject);//destroy self	
			}else
			{
				print("duplicate detected: " +this.name );
			}

			break;

		default:
			break;
		}
	}

}
