using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

	public List<EnemyAI> EnemiesList = new List<EnemyAI>();
	GameObject player;

	void Start () {
		// add all enemies to manager
		if(transform.childCount > 0)
		{
			foreach(Transform child in this.transform)
			{
				EnemiesList.Add(child.GetComponent<EnemyAI>());
			}
		}

		player = GameObject.FindGameObjectWithTag("Player");
	}

	void Update () {

	}

	// fn. to add alert to all AIs 
	// (checking whether to add alertness - according to dist - is done in EnemyAlert fn. itself)
	public void AddAlertGlobally(Vector3 srcPos) {
		if(EnemiesList.Count > 0)
		{
			foreach(EnemyAI enemy in EnemiesList)
			{
				enemy.GetComponent<EnemyAlert>().AlertIncr(srcPos);
			}
		}
	}

	// fn. to get closest AI to player 
	public float GetClosestAlertness() {
		float distFromPlayer = -1;
		EnemyAI enemyCloseToPlayer = null;
		
		if(EnemiesList.Count > 0)
		{
			foreach(EnemyAI enemy in EnemiesList)
			{
				if(enemyCloseToPlayer == null)
				{
					enemyCloseToPlayer = enemy;
					distFromPlayer = (player.transform.position - enemyCloseToPlayer.transform.position).magnitude;
				}
				else
				{
					// if next enemy in enemiesList is closer thn the one before
					if((player.transform.position - enemy.transform.position).magnitude < distFromPlayer)
					{
						enemyCloseToPlayer = enemy;
						distFromPlayer = (player.transform.position - enemyCloseToPlayer.transform.position).magnitude;
					}
					// if both same dist away from player
					else if((player.transform.position - enemy.transform.position).magnitude == distFromPlayer)
					{
						// if current enemy in this list has > alertness, switch to him
						if(enemy.GetComponent<EnemyAlert>().alert > enemyCloseToPlayer.GetComponent<EnemyAlert>().alert)
						{
							enemyCloseToPlayer = enemy;
							distFromPlayer = (player.transform.position - enemyCloseToPlayer.transform.position).magnitude;
						}
					}
				}
			}
			return enemyCloseToPlayer.GetComponent<EnemyAlert>().alert;
		}
		else
			return 0.0f;
	}
}
