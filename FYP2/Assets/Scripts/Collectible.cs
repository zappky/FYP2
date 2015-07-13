using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {

	public LevelManager levelManager = null;

	// Use this for initialization
	void Start () {
		levelManager = LevelManager.Instance;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider other)
	{
		switch(other.gameObject.tag)
		{
		case "Player":
			if(levelManager.achievementmanager.AddCollectible(levelManager.CurrentLevelName) == true)
			{
				Destroy(this.gameObject);//destroy self	
			}else
			{
				Debug.Log("ERROR: Cannot find collectible list to record,so gameobject wouldnt be destroy");
			}
			break;

		default:
			break;
		}
	}

}
