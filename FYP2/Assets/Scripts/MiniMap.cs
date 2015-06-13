using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour {

	public GameObject[] enemies;
	List<GameObject> radarObjects;
	public GameObject enemyPrefab;
	List<GameObject> borderObjects;
	public float switchDist;
	public Transform helpTransform;

	// Use this for initialization
	void Start () 
	{
		createRadarObjects();
	}
	
	// Update is called once per frame
	void Update () 
	{
		for (int i = 0; i < radarObjects.Count; i++) 
		{
			if(Vector3.Distance(radarObjects[i].transform.position, transform.position) > switchDist)
			{
				//switch to see borderobjects
				helpTransform.LookAt(radarObjects[i].transform);
				borderObjects[i].transform.position = transform.position + switchDist * helpTransform.forward;
				borderObjects[i].layer = LayerMask.NameToLayer("MiniMap");
				radarObjects[i].layer = LayerMask.NameToLayer("Invisible");
			}
			else
			{
				borderObjects[i].layer = LayerMask.NameToLayer("Invisible");
				radarObjects[i].layer = LayerMask.NameToLayer("MiniMap");
			}
		}
	}

	void createRadarObjects()
	{
		radarObjects = new List<GameObject> ();
		borderObjects = new List<GameObject> ();
		foreach(GameObject o in enemies)
		{
			GameObject k = Instantiate(enemyPrefab, o.transform.position, Quaternion.identity) as GameObject;
			radarObjects.Add(k);
			GameObject j = Instantiate(enemyPrefab, o.transform.position, Quaternion.identity) as GameObject;
			borderObjects.Add(j);
		}
	}
}
