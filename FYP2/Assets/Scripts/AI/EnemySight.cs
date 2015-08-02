using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour 
{
	public float eyeHeight = 67.5f;
	public float fieldOfViewAngle = 110f; 	// how far AI can see 
	//public bool playerInSight;
	public LayerMask rayLayers;				// layers raycast would touch

	//public Vector3 targetPos;		// pos to move towards to (player/last seen pos)

	Vector3 lastSeenPos;			// player last seen pos
	SphereCollider col;
	CapsuleCollider bodyCol;
	GameObject player;
	EnemyAlert alert;				// AI's alertness calculations
	EnemyStats stats;	

	void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
		bodyCol = GetComponent<CapsuleCollider>();
		col = GetComponent<SphereCollider>();
		alert = GetComponent<EnemyAlert>();
		stats = GetComponent<EnemyStats>();
	}


	void Update() {
//		// if player too close to AI, add alert
//		print((AIpos-player.transform.position).magnitude +" vs "+ stats.attackRange+bodyCol.bounds.extents.x);
//
//		if((AIpos-player.transform.position).magnitude < stats.attackRange+bodyCol.bounds.extents.x)
//		{
//			alert.AlertIncr(player.transform.position);
//		}
	}


	void OnTriggerStay(Collider other)
	{
		// if player enters sphere col
		if(other.gameObject == player)
		{
			// old method
			//playerInSight = false;		// reset
			alert.playerInSight = false;		// reset
			alert.playerInRange = false;		// reset

			Vector3	AIpos = transform.position+transform.up*eyeHeight;
			Vector3 dir = other.transform.position+other.transform.up - AIpos;
			float angle = Vector3.Angle(new Vector3(dir.x, 0, dir.z), transform.forward);

			Ray ray = new Ray(AIpos, dir.normalized);	
			// check if player is within AI's fov  
			if(angle < fieldOfViewAngle * 0.5f)
			{ 
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, Mathf.Infinity, rayLayers))
				{
					// if ray hits player (i.e. AI spotted player)
					if(hit.collider.gameObject == player)
					{
						// add alertness
						alert.playerInSight = true;

						// old method
//						playerInSight = true;
//						targetPos = new Vector3(player.transform.position.x,
//						                        transform.position.y,			// since AI is giant, use his y pos
//						                        player.transform.position.z);
//						lastSeenPos = targetPos;

						Debug.DrawRay(ray.origin, ray.direction*transform.localScale.x*col.radius, Color.green, 1);
					}
					//in fov, but gt obj blocking sight
					else
					{
						// old method
						//targetPos = lastSeenPos;	// AI would move to last seen pos

						Debug.DrawRay(ray.origin, ray.direction*transform.localScale.x*col.radius, Color.red, 1);
					}
				}
			}
			else
			{
				Debug.DrawRay(ray.origin, ray.direction*transform.localScale.x*col.radius, Color.black, 1);
			}

			// if player running near AI or is seen
			if((player.GetComponent<CharacterController>().isGrounded
			&&  player.GetComponent<FpsMovement>().isRunning) 
			||  alert.playerInSight)
			{
				// (calc of alert incr depending on player being seen or not is done in this fn.)
				alert.AlertIncr(player.transform.position);
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		alert.playerInSight = false;		// reset
		alert.playerInRange = false;		// reset
	}
}
