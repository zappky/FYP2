using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour {

	public GameObject[] enemies;
	public GameObject[] objectives;
	List<GameObject> radarObjectsE;  //enemies
	List<GameObject> borderObjectsE;
	List<GameObject> radarObjectsO;  //objectives
	List<GameObject> borderObjectsO;
	public GameObject enemyPrefab;
	public GameObject objectivePrefab;
	public float switchDist;
	public Transform helpTransform;

	List<EnemyAI> ai = new List<EnemyAI>();

	// Use this for initialization
	void Start () 
	{
		enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		for(int i = 0; i < enemies.Length; i++)
		{
			//Debug.Log(enemies[i].name);
			ai.Add(enemies[i].GetComponent<EnemyAI>());
		}
		createRadarObjects();
	}
	
	// Update is called once per frame
	void Update () 
	{
		for (int i = 0; i < radarObjectsO.Count; i++) 
		{
			if(Vector3.Distance(radarObjectsO[i].transform.position, transform.position) > switchDist)
			{
				//switch to see borderobjects
				helpTransform.LookAt(radarObjectsO[i].transform);
				borderObjectsO[i].transform.position = transform.position + switchDist * helpTransform.forward;
				/*if(radarObjects[i].tag == )
				{
					radarObjects[i].transform.position = enemies[i].transform.position
				}*/
				borderObjectsO[i].layer = LayerMask.NameToLayer("MiniMap");
				radarObjectsO[i].layer = LayerMask.NameToLayer("Invisible");
			}
			else
			{
				borderObjectsO[i].layer = LayerMask.NameToLayer("Invisible");
				radarObjectsO[i].layer = LayerMask.NameToLayer("MiniMap");
			}
		}

		for (int i = 0; i < radarObjectsE.Count; i++) 
		{
			if(Vector3.Distance(radarObjectsE[i].transform.position, transform.position) > switchDist)
			{
				//switch to see borderobjects
				helpTransform.LookAt(radarObjectsE[i].transform);
				borderObjectsE[i].transform.position = transform.position + switchDist * helpTransform.forward;

				radarObjectsE[i].transform.position = enemies[i].transform.position;

				borderObjectsE[i].layer = LayerMask.NameToLayer("MiniMap");
				radarObjectsE[i].layer = LayerMask.NameToLayer("Invisible");
			}
			else
			{
				borderObjectsE[i].layer = LayerMask.NameToLayer("Invisible");
				radarObjectsE[i].layer = LayerMask.NameToLayer("MiniMap");
			}
		}
	}

	void createRadarObjects()
	{
		radarObjectsE = new List<GameObject> ();
		borderObjectsE = new List<GameObject> ();

		radarObjectsO = new List<GameObject> ();
		borderObjectsO = new List<GameObject> ();

		if(objectives.Length > 0)
		{
			foreach(GameObject o in objectives)
			{
				GameObject k = Instantiate(objectivePrefab, o.transform.position, Quaternion.identity) as GameObject;
				radarObjectsO.Add(k);
				GameObject j = Instantiate(objectivePrefab, o.transform.position, Quaternion.identity) as GameObject;
				borderObjectsO.Add(j);
			}
		}

		if(ai.Count > 0)
		{
			for(int i = 0; i < ai.Count; i++)
			{
				GameObject k = Instantiate(enemyPrefab, ai[i].transform.position, Quaternion.identity) as GameObject;
				//k.GetComponent<MeshRenderer>().enabled = false;
				radarObjectsE.Add(k);
				GameObject j = Instantiate(enemyPrefab, ai[i].transform.position, Quaternion.identity) as GameObject;
				borderObjectsE.Add(j);
			}
		}
	}
}
