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
	EnemySight sight;					// this includes enemy hearing 
	EnemyAlert alert;
	Renderer debugRenderer;				// to access AI renderer for color chg to show alert state (debug only)
	SphereCollider col;					// AI's sight & hearing range 
	GameObject player;


	void Start () {
		stats = gameObject.GetComponent<EnemyStats>();
		sight = gameObject.GetComponent<EnemySight>();
		alert = gameObject.GetComponent<EnemyAlert>();
		col = gameObject.GetComponent<SphereCollider>(); 
		player = GameObject.FindGameObjectWithTag("Player");
		debugRenderer = gameObject.GetComponent<Renderer>();

		stats.attackRange += GetComponent<CapsuleCollider>().bounds.size.x*0.5f;

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
		CheckAlertness();

		FSM();
	}

	void CheckAlertness()
	{
		if(state != EnemyState.ALERT)
		{
			if(alert.isAlarmed)
				state = EnemyState.ALERT;
			else if(alert.isAlarmedByDecoy)
			{
				if(!delaySet)									// delay abit, so AI wont immediately rush there
				{
					delaySet = true;
					delay = stats.IDLE_DELAY;					
				}
				else
				{
					delay -= Time.deltaTime;						
					
					if(delay <= 0)							 
					{
						delaySet = false;
						state = EnemyState.ALERT;

						targetPos = new Vector3(alert.targetPos.x,
						                        transform.position.y,		// so that AI wont fly/go underground
						                        alert.targetPos.z);
					}
				}
			}
		}
	}

	void FSM()
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
				}

				// old mtd
//				if(worldDecoyList[0].GetComponent<CTimer>().alert)	// if alarm decoy in world rings
//				{
//					if(!delaySet)									// delay abit, so AI wont immediately rush there
//					{
//						delaySet = true;
//						delay = stats.IDLE_DELAY;					
//					}
//					else
//					{
//						delay -= Time.deltaTime;						
//						
//						if(delay <= 0)							 
//						{
//							delaySet = false;
//							alert.isAlarmedByDecoy = true;
//							target = worldDecoyList[0].transform;
//							targetPos = new Vector3(target.position.x,
//							                        transform.position.y,		// so that AI wont fly/go underground
//							                        target.position.z);
//						}
//					}
//				}
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
				
				// old mtd
//				if(worldDecoyList[0].GetComponent<CTimer>().alert)	// if alarm decoy in world rings
//				{
//					if(!delaySet)									// delay abit, so AI wont immediately rush there
//					{
//						delaySet = true;
//						delay = stats.IDLE_DELAY;					
//
//						// turn AI render color to red
//						gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
//					}
//					else
//					{
//						delay -= Time.deltaTime;						
//						
//						if(delay <= 0)							 
//						{
//							delaySet = false;
//							state = EnemyState.ALERT;
//							target = worldDecoyList[0].transform;
//							targetPos = new Vector3(target.position.x,
//							                        transform.position.y,		// so that AI wont fly/go underground
//							                        target.position.z);
//						}
//					}
//				}
			}
			break;
			
//#ROAM		
//=================================================================================================================
//#ALERT
										
			case EnemyState.ALERT:		// got alerted, search for player (or if in sight, attack)
			{
				// check if gt anyth in AI's way (most probably when chasing player/decoy made by player
				if(collideWithOther)
				{
					if(alert.isAlarmedByDecoy)
					{
						if(!delaySet)
						{
							delaySet = true;
							// temp, AI should 'act' like he's looking ard
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
				}
				else
				{
					float alertMoveSpeed;
					
					// old mtd
//					if(target == null && alert.playerInSight)
//						target = player.transform;

//					if(target.gameObject == player)
//					{
//						targetPos = alert.targetPos;
//						alertMoveSpeed = stats.alertSightMoveSpeed;
//						distDiff = stats.attackRange+player.GetComponent<CharacterController>().bounds.size.x*0.5f;
//					}
//					else
//					{
//						alertMoveSpeed = stats.alertNoiseMoveSpeed;
//						distDiff = stats.attackRange+target.gameObject.GetComponent<Collider>().bounds.size.x*0.5f;
//					}

					if(alert.playerInSight)
						alertMoveSpeed = stats.alertSightMoveSpeed;
					else
						alertMoveSpeed = stats.alertNoiseMoveSpeed;
						
					// if target is close enough
					if((transform.position-targetPos).magnitude < stats.attackRange)
					{
						// do sth
						// old mtd
						//if(target.gameObject == worldDecoyList[0])
						if(alert.isAlarmedByDecoy)
						{
							// old mtd..
//							worldDecoyList[0].GetComponent<CTimer>().alert = false;		// AI switches off the alarm
//							worldDecoyList[0].GetComponent<SoundEffect>().StopSound();	

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
									// find a way to reset world decoy
									delaySet = false;
									state = EnemyState.RESET;
								}
							}
						}		
						else if(!alert.playerInSight)	// i.e. AI was chasing last seen/heard pos
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
						// add alertness rapidly (max alert = gameover)
						else if(alert.playerInSight)
						{
							alert.playerInRange = true;
						}
					}
					else 	
					{
						// face target
						Quaternion targetRotation = Quaternion.LookRotation(alert.targetPos-transform.position);
						transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y,
						                                                            targetRotation.eulerAngles.y,
						                                                            stats.rotationSpeed * Time.deltaTime);
					
						// move AI closer to target
						transform.position = Vector3.MoveTowards(transform.position, alert.targetPos, 
						                                         stats.moveSpeed*alertMoveSpeed * Time.deltaTime);
					}
				}
			}
			break;
			
//#ALERT		
//=================================================================================================================
//#RESET

			case EnemyState.RESET:		// relax, and go back to normal. AI goes to next waypt before idling fr a while
			{
				alert.isAlarmedByDecoy = false;
				collideWithOther = false;
				
				// go back to waypt prev heading
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
			}
			break;
		}
	}


	void OnCollisionEnter(Collision other)
	{
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
