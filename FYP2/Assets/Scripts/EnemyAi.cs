using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script aim to describle behaviour of ai

public class EnemyAi : MonoBehaviour {

	public enum EnemyState
	{
		NONE,
		IDLE,
		PATROL,
		ALERT,
		RESET
	}
	public EnemyState state = EnemyState.NONE;
	//a frustrum
	public float movementspeed = 1.0f;
	public float eyesight = 1.0f; // uncomfirm yet
	public float attackrange = 10.0f; // the range of reach of ai
	public float tolerancelength = 10.0f;//the length of which to determine whether ai has reach the waypoint
	public GUISkin icon;//icon on minimap

	public List<Transform> waypointlist = new List<Transform>();//list of waypoint
	int targetwaypoint = 0;
	public Transform targetenemy;

	// Use this for initialization
	void Start () {
		waypointlist.Add (transform.position);//storing original position
	}
	
	// Update is called once per frame
	void Update () {
		AiUpdate ();
	}

	void AiUpdate()
	{
		switch (state) 
		{
			case EnemyState.NONE:
			case EnemyState.IDLE:
			default:
			{
				//do nothing
			}
			break;

			case EnemyState.PATROL://waypoint patrol
			{
				if(waypointlist.Count > 1)//if there are waypoint other than original point
				{
					Vector3.MoveTowards(transform.position,waypointlist[targetwaypoint].position,movementspeed *Time.deltaTime);
				if( (waypointlist[targetwaypoint].position-transform.position).sqrMagnitude < tolerancelength )
					{
						++targetwaypoint;
						if(targetwaypoint > waypointlist.Count)
						{
							targetwaypoint = 1;
						}
					}
				}
			}
			break;

			case EnemyState.ALERT://got alerted,search and attack enemy
			{
			Vector3.MoveTowards(transform.position,targetenemy.position,movementspeed * Time.deltaTime);

				if( (transform.position - targetenemy.position).sqrMagnitude <attackrange )
				{
					//attack animation and stuff
				}
			}
			break;

			case EnemyState.RESET://relax,and go back to normal
			{
				targetwaypoint = 0;
				Vector3.MoveTowards(transform.position,waypointlist[targetwaypoint].position,movementspeed * Time.deltaTime);
			if( (waypointlist[targetwaypoint].position-transform.position).sqrMagnitude < tolerancelength )
				{
					targetwaypoint = 1;//restore to the first waypoint
					state = EnemyState.IDLE;//reset the state
				}
			}
			break;

		}
	}
}
