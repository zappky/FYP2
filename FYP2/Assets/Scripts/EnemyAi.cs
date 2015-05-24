using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this script aim to describle behaviour of ai

public class EnemyAI : MonoBehaviour {

	public enum EnemyState
	{
		IDLE,
		ROAM,
		ALERT,
		RESET
	}
	public EnemyState state = EnemyState.IDLE;

	//AI properties
	public float moveSpeed = 5.0f;
	public float alertMoveSpeed = 2.0f;			// move spd multiplier for when in alert state
	public float IDLE_DELAY = 2.0f;				// how long AI stays in idle before roaming 
	public float lineOfSight = 10.0f; 			// how far AI can see (adv.)
	public float attackRange = 10.0f; 			// AI's range of reach 
	public float toleranceLength = 10.0f;		// the length of which to determine whether AI has reach the waypoint
	//public GUISkin icon;						// icon on minimap

	public Transform target;					// stores location of target for AI to move to  
												// (e.g. noise player made/ player's location(if he's in view)/ 
												//  last seen location)

	// list of waypoints
	public List<Transform> waypointList = new List<Transform>();	 
	private int nextWaypt; 						// AI's next waypoint  

	private float delay;						// idle delay (in seconds)


	void Start () {
		delay = IDLE_DELAY;
		nextWaypt = 0;							// 0 - original pos. (whr AI spawns/is from) 

		// Spawn pos
		this.transform.position = waypointList[nextWaypt].position;			

		//waypointlist.Add (transform.position); 	//storing spawn position

		// AI render color = green
	}


	void Update () {
		AIUpdate ();
	}

	void AIUpdate()
	{
		switch (state) 
		{
//=================================================================================================================
//#IDLE

			case EnemyState.IDLE:
			default:
			{
				// slack, do nth
				delay -= Time.deltaTime;						
				
				if(delay <= 0)							 
				{
					delay = IDLE_DELAY;
					state = EnemyState.ROAM;			 
				}

				// STEALTH CHECK
				// if heard loud noise or spotted player 
				// state = alert
				// AI render color = red
				// target = noise/player/player last seen
			}
			break;

//#IDLE		
//=================================================================================================================
//#ROAM

			case EnemyState.ROAM:			// roam from 1 waypt to another
			{
				// MOVEMENT
				if(waypointList.Count > 1)	// if there are waypoint other than original point
				{
					// move to next waypt
					transform.position = Vector3.MoveTowards(transform.position, waypointList[nextWaypt].position, 
				                                         	 moveSpeed * Time.deltaTime);
					
					// if AI is alr on targeted waypt 
					if( (waypointList[nextWaypt].position-transform.position).sqrMagnitude < toleranceLength )
					{
						++nextWaypt;		// move to next waypt
						if(nextWaypt > waypointList.Count-1)
						{
							nextWaypt = 0;
						}
					}
				}

				// STEALTH CHECK
				// if heard loud noise or spotted player 
				// state = alert
				// AI render color = red
				// target = noise/player/player last seen
			}
			break;
			
//#ROAM		
//=================================================================================================================
//#ALERT
										
			case EnemyState.ALERT:		// got alerted, search for player (or if in sight, attack)
			{
				Vector3.MoveTowards(transform.position, target.position, moveSpeed*alertMoveSpeed * Time.deltaTime);
				
				// if alertness >= 20
				// if player within atk range
				// 		(if we're doing AI attack for this proj)
				// 		alertness = max  
				//		attack player
				// 		(else)
				//		alertIncr acc = 3
				// 		alertness += alertIncrement * alertIncr acc, gameover if alertness is at max

				// else (if player not within atk range)
				// 
				//		if player in sight
				//			alertIncr acc = 1.5
				//			alertness += alertIncrement * alertIncr acc
				//			target = player
				//
				//	 		if lost sight of player
				//				target = player last seen pos
				//				if at target (i.e. last seen pos)
				//					look ard/roam movement
				//		 			alertness -=  alertDecrement*0.5	// slower dec of alertness
				//
				//		else (distraction/noise was cause of alert)
				//			if at target (i.e. location of d/noise)
				//				look ard/roam movement
				//	 			alertness -=  alertDecrement			// faster dec of alertness
				
				// else (alert < 20)
				// state = reset
			}
			break;
			
//#ALERT		
//=================================================================================================================
//#RESET

			case EnemyState.RESET:		// relax, and go back to normal. AI goes to next waypt before idling fr a while
			{
				Vector3.MoveTowards(transform.position,waypointList[nextWaypt].position,moveSpeed * Time.deltaTime);
				
				if( (waypointList[nextWaypt].position-transform.position).sqrMagnitude < toleranceLength )
				{
						++nextWaypt;						// get rdy to move to next waypt
						if(nextWaypt > waypointList.Count-1)
						{
							nextWaypt = 0;
						}
						state = EnemyState.IDLE;			//reset the state
				}
			}
			break;
		}
	}
}
