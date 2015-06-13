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
	//public GUISkin icon;						// icon on minimap
	public EnemyStats stats;
	public GameObject Player;
	public Transform target;					// stores location of target for AI to move to  
												// (e.g. noise player made/ player's location(if he's in view)/ 
												//  last seen location)

	// list of waypoints
	public  List<Transform> waypointList = new List<Transform>();	 
	private int nextWaypt; 						// AI's next waypoint 

	// list of decoys in world (e.g. alarm clock)
	public  List<GameObject> worldDecoyList = new List<GameObject>();	 

	private bool collideWithOther;				// to stop AI from moving any further due to obstacle blocking (e.g. chair in AI's path)
	private bool playerInSight;					// may nt be needed
	private float delay;						// idle delay (in seconds)

	private SphereCollider col;					// AI's hearing range 

	void Start () {
		collideWithOther = false;
		delay = stats.IDLE_DELAY;
		nextWaypt = 0;							// 0 - original pos. (whr AI spawns/is from) 

		// Spawn pos
		transform.position = waypointList[nextWaypt].position;			

		// AI render color = green
		gameObject.GetComponent<Renderer>().material.color = new Color(0, 1, 0);

		stats.attackRange += GetComponent<Transform>().localScale.x;
		col = gameObject.GetComponent<SphereCollider>(); 
	}

	void Update()
	{
		switch (state) 
		{
//=================================================================================================================
//#IDLE

			case EnemyState.IDLE:
			default:
			{
				// slack, do nth/ look ard
				delay -= Time.deltaTime;						
				
				if(delay <= 0)							 
				{
					delay = stats.IDLE_DELAY;
					state = EnemyState.ROAM;		

					//transform.LookAt(waypointList[nextWaypt].transform.position, Vector3.up);
				}

				// STEALTH CHECK
				// if heard loud noise or spotted player 
				// state = alert
				// AI render color = red
				// target = noise/player/player last seen
				if(worldDecoyList[0].GetComponent<CTimer>().alert)	// if alarm decoy in world rings
				{
					delay = stats.IDLE_DELAY*2;
					state = EnemyState.ALERT;
					target = worldDecoyList[0].transform;
					target.position = new Vector3(target.position.x,
				                     	 		  transform.position.y,		// so that AI wont fly/go underground
				                     	 		  target.position.z);

					// turn AI render color to red
					gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
				}
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
				                                         	 stats.moveSpeed * Time.deltaTime);
					
					// if AI is alr on targeted waypt 
					if( (waypointList[nextWaypt].position-transform.position).sqrMagnitude < stats.toleranceLength )
					{
						++nextWaypt;				// move to next waypt
						if(nextWaypt > waypointList.Count-1)
						{
							nextWaypt = 0;
						}
						
						state = EnemyState.IDLE;	// slack/look ard for a while
					}
				}

				// STEALTH CHECK
				// if heard loud noise or spotted player 
				// state = alert
				// AI render color = red
				// target = noise/player/player last seen
				if(worldDecoyList[0].GetComponent<CTimer>().alert)	// if alarm decoy in world rings
				{
					delay = stats.IDLE_DELAY*2;
					state = EnemyState.ALERT;
					target = worldDecoyList[0].transform;
					target.position = new Vector3(target.position.x,
				                     	 		  transform.position.y,		// so that AI wont fly/go underground
				                     	 		  target.position.z);

					// turn AI render color to red
					gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
				}
			}
			break;
			
//#ROAM		
//=================================================================================================================
//#ALERT
										
			case EnemyState.ALERT:		// got alerted, search for player (or if in sight, attack)
			{
				// if target is close enough
				if((target.position-transform.position).magnitude < stats.attackRange )
				{
					// do sth
					if(target.gameObject == worldDecoyList[0])
					{
						worldDecoyList[0].GetComponent<CTimer>().alert = false;		// AI switches off the alarm
						worldDecoyList[0].GetComponent<SoundEffect>().StopSound();	
					}	

					delay = stats.IDLE_DELAY;
					state = EnemyState.RESET;	

					// AI render color = green
					gameObject.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
				}
				else
				{
					//transform.LookAt(target.transform.position, Vector3.up);
					// move AI closer to target
					if(!collideWithOther)
					{
						float alertMoveSpeed;

						if(target.gameObject == Player)
						{
							alertMoveSpeed = stats.alertSightMoveSpeed;
						}
						else
						{
							alertMoveSpeed = stats.alertNoiseMoveSpeed;
						}
						
						transform.position = Vector3.MoveTowards(transform.position, target.position, 
					                                         	 stats.moveSpeed*alertMoveSpeed * Time.deltaTime);
					}
				}
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
				transform.position = Vector3.MoveTowards(transform.position, waypointList[nextWaypt].position,
			                                         	 stats.moveSpeed * Time.deltaTime);
				
				if( (waypointList[nextWaypt].position-transform.position).sqrMagnitude < stats.toleranceLength )
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

	void OnTriggerStay(Collider other)
	{
		// if player enters sphere col
		if(other.gameObject == Player)
		{
			playerInSight = false;		// reset

			// check if player is within fov of AI
//			Vector3 dir = other.transform.position - transform.position;
//			float angle = Vector3.Angle(dir, transform.forward);			
//
//			if(angle > fieldOfViewAngle * 0.5f)
//			{
//				RaycastHit hit;
//
//				if(Physics.Raycast(transform.position,
//				                   dir.normalized, out hit, col.radius))
//				{
//					// if raycast hits player (i.e. AI spotted player)
//					if(hit.collider.gameObject == Player)
//					{
//						playerInSight = true;
//						target = Player.transform;
//						//state = EnemyState.ALERT;
//
//						// turn AI render color to red
//						gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
//					}
//				}
					
			// simple check for AI view (sphere col = AI fov)
			if(other.gameObject == Player)	
			{
				playerInSight = true;
				target = Player.transform;
				target.position = new Vector3(Player.transform.position.x,
						                      transform.position.y,
						                      Player.transform.position.z);
				state = EnemyState.ALERT;

				// turn AI render color to red
				gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject == Player)
		{
			playerInSight = false;
			//target = Player.transform (last seen pos);
			target = null;
			state = EnemyState.RESET;
			
			// turn AI render color to red
			gameObject.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
		}
	}

	void OnCollisionEnter()
	{
		collideWithOther = true;	
		Debug.Log ("AI BANG TO STH");
	}
	void OnCollisionExit()
	{
		collideWithOther = false;
		Debug.Log ("AI NO LONGER BANGING");
	}
}
