using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (EnemyStats))]
[RequireComponent (typeof (EnemySight))]
[RequireComponent (typeof (EnemyAlert))]
public class EnemyAI : MonoBehaviour {

	public enum EnemyState
	{
		IDLE,
		ROAM,
		ALERT,
		RESET
	}
	public EnemyState state = EnemyState.IDLE;

	public List<Transform> waypointList = new List<Transform>();		// list of waypoints 
	public List<GameObject> worldDecoyList = new List<GameObject>();	// list of decoys in world (e.g. alarm clock)
			
	bool delaySet;						// true if delay is set (used mainly for alert delays)
	bool collideWithOther;				// to stop AI from moving due to obstacle blocking, whn chasing player (e.g. chair in AI's path)
	int nextWaypt; 						// AI's next waypoint 	
	float delay;						// idle delay (in seconds)

	Transform target;
	Vector3 targetPos;					// stores location of target for AI to move to  
										// (e.g. src of noise player made/ decoy's src/ player in sight)	 
	EnemyStats stats;
	EnemySight sight;
	Renderer debugRenderer;				// to access AI renderer for color chg to show alert state (debug only)
	SphereCollider col;					// AI's sight & hearing range 
	GameObject player;


	void Start () {
		stats = gameObject.GetComponent<EnemyStats>();
		sight = gameObject.GetComponent<EnemySight>();
		col = gameObject.GetComponent<SphereCollider>(); 
		player = GameObject.FindGameObjectWithTag("Player");
		debugRenderer = gameObject.GetComponent<Renderer>();

		stats.attackRange += GetComponent<Collider>().bounds.size.x*0.5f;

		delaySet = false;
		collideWithOther = false;
		delay = stats.IDLE_DELAY;
		nextWaypt = 0;					// 0 - original pos. (whr AI spawns/is from) 
		
		// Spawn pos
		transform.position = waypointList[nextWaypt].position;	

		// AI render color = green (!alerted)
		debugRenderer.material.color = Color.green;
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
					if(!delaySet)									// delay abit, so AI wont immediately rush there
					{
						delaySet = true;
						delay = stats.IDLE_DELAY;					

						// turn AI render color to red
						gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
					}
					else
					{
						delay -= Time.deltaTime;						
						
						if(delay <= 0)							 
						{
							delaySet = false;
							state = EnemyState.ALERT;
							target = worldDecoyList[0].transform;
							targetPos = new Vector3(target.position.x,
							                        transform.position.y,		// so that AI wont fly/go underground
							                        target.position.z);
						}
					}
				}
				else if(sight.playerInSight)			//temp, use alertness value once alert calc is done
					state = EnemyState.ALERT;
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
					// face next waypt
					Quaternion targetRotation = Quaternion.LookRotation(waypointList[nextWaypt].position-transform.position);
					transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y,
					                                                            targetRotation.eulerAngles.y,
					                                                            stats.rotationSpeed * Time.deltaTime); 
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
					if(!delaySet)									// delay abit, so AI wont immediately rush there
					{
						delaySet = true;
						delay = stats.IDLE_DELAY;					

						// turn AI render color to red
						gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
					}
					else
					{
						delay -= Time.deltaTime;						
						
						if(delay <= 0)							 
						{
							delaySet = false;
							state = EnemyState.ALERT;
							target = worldDecoyList[0].transform;
							targetPos = new Vector3(target.position.x,
							                        transform.position.y,		// so that AI wont fly/go underground
							                        target.position.z);
						}
					}
				}
				else if(sight.playerInSight)			//temp, use alertness value once alert calc is done
					state = EnemyState.ALERT;
			}
			break;
			
//#ROAM		
//=================================================================================================================
//#ALERT
										
			case EnemyState.ALERT:		// got alerted, search for player (or if in sight, attack)
			{
				// check if gt anyth in AI's way
				if(collideWithOther)
				{
					if(!delaySet)
					{
						delaySet = true;
						// temp, AI should 'act' like he's waiting for player (or sth else) before reseting
						delay = stats.ALERT_DELAY;	

					}
					else
					{
						delay -= Time.deltaTime;						
						
						if(delay <= 0)							 
						{
							delaySet = false;
							state = EnemyState.RESET;
						}
					}
				}
				else
				{
					float alertMoveSpeed, distDiff;
					
					if(target == null && sight.playerInSight)
						target = player.transform;

					if(target.gameObject == player)
					{
						targetPos = sight.targetPos;
						alertMoveSpeed = stats.alertSightMoveSpeed;
						distDiff = stats.attackRange+player.GetComponent<CharacterController>().bounds.size.x*0.5f;
					}
					else
					{
						alertMoveSpeed = stats.alertNoiseMoveSpeed;
						distDiff = stats.attackRange+target.gameObject.GetComponent<Collider>().bounds.size.x*0.5f;
					}
				
					// if target is close enough
					if((transform.position-targetPos).magnitude < distDiff)
					{
						// do sth
						if(target.gameObject == worldDecoyList[0])
						{
							worldDecoyList[0].GetComponent<CTimer>().alert = false;		// AI switches off the alarm
							worldDecoyList[0].GetComponent<SoundEffect>().StopSound();	

							delay -= Time.deltaTime;						
							
							if(!delaySet)
							{
								delaySet = true;
								delay = stats.ALERT_DELAY;	
							}
							else
							{
								delay -= Time.deltaTime;						
								
								if(delay <= 0)							 
								{
									delaySet = false;
									state = EnemyState.RESET;
								}
							}
						}		
						// atk player 

						// if lose sight of player
						else if(!sight.playerInSight)		
						{
							if(!delaySet)
							{
								delaySet = true;
								// temp, AI should 'act' like he's looking ard, before reseting
								delay = stats.ALERT_DELAY;	
							}
							else
							{
								delay -= Time.deltaTime;						
								
								if(delay <= 0)							 
								{
									delaySet = false;
									state = EnemyState.RESET;
									Debug.Log("RESET");
								}
							}	
						}
					}
					else 	
					{
						// face target
						Quaternion targetRotation = Quaternion.LookRotation(targetPos-transform.position);
						transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y,
						                                                            targetRotation.eulerAngles.y,
						                                                            stats.rotationSpeed * Time.deltaTime);
					
						// move AI closer to target
						transform.position = Vector3.MoveTowards(transform.position, targetPos, 
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
				collideWithOther = false;
				// face next waypt
				Quaternion targetRotation = Quaternion.LookRotation(waypointList[nextWaypt].position-transform.position);
				transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y,
				                                                            targetRotation.eulerAngles.y,
				                                                            stats.rotationSpeed * Time.deltaTime); 
				transform.position = Vector3.MoveTowards(transform.position, waypointList[nextWaypt].position,
			                                         	 stats.moveSpeed * Time.deltaTime);
				
				if( (waypointList[nextWaypt].position-transform.position).sqrMagnitude < stats.toleranceLength )
				{
					++nextWaypt;						// get rdy to move to next waypt
					if(nextWaypt > waypointList.Count-1)
					{
						nextWaypt = 0;
					}
					delay = stats.IDLE_DELAY;
					state = EnemyState.IDLE;			//reset the state
				}

				debugRenderer.material.color = Color.green;
			}
			break;
		}
	}


	void OnCollisionEnter(Collision other)
	{
		if(!sight.playerInSight)			// since AI cant see player
			return;
		if (other.gameObject.tag != "Floor")
		{
			// static obj is in AI's way (e.g. chair, bed)
			if(other.gameObject.GetComponent<Rigidbody>() == null)
			{
				collideWithOther = true;
				Debug.Log("COL W STATIC OBJ: " + other.gameObject.name);
			}
			else
			{
				// static game obj is in AI's way (e.g. grappable wall)
				if(other.gameObject.GetComponent<Rigidbody>().isKinematic)
				{
					collideWithOther = true;
					Debug.Log("COL W STATIC GAME OBJ: " + other.gameObject.name);
				}
			}
		}
	}
	void OnCollisionExit()
	{
		collideWithOther = false;
	}
}
