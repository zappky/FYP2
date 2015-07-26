using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour 
{
	public float eyeHeight = 67.5f;
	public float fieldOfViewAngle = 110f; 	// how far AI can see 
	//public bool playerInSight;
	public LayerMask rayLayers;				// layers raycast would touch

	//public Vector3 targetPos;				// pos to move towards to (player/last seen pos)

	Vector3 lastSeenPos;					// player last seen pos
	SphereCollider col;
	GameObject player;
	EnemyAlert alert;						// AI's alertness calculations

	void Start() {
		col = GetComponent<SphereCollider>();
		player = GameObject.FindGameObjectWithTag("Player");
		alert = gameObject.GetComponent<EnemyAlert>();
	}


	void Update() {
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

			Vector3 origin = transform.position+transform.up*eyeHeight;
			Vector3 dir = other.transform.position+other.transform.up - origin;
			float angle = Vector3.Angle(dir, transform.forward);

			Ray ray = new Ray(origin, dir.normalized);	
			// check if player is within AI's fov  
			if(angle < fieldOfViewAngle * 0.5f)
			{ 
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, transform.localScale.x*col.radius, rayLayers))
				{
					print (hit.collider.name);
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
				Debug.DrawRay(ray.origin, ray.direction*transform.localScale.x*col.radius, Color.black, 1);
			
			// if player running near AI or is seen
			if((player.GetComponent<CharacterController>().isGrounded
			&&  player.GetComponent<FpsMovement>().isRunning) 
			||  alert.playerInSight)
			{
				// (calc of alert incr depending on player being seen or not is done in this fn.)
				alert.AlertIncr(new Vector3(player.transform.position.x,
				                        	transform.position.y,			// since AI is giant, use his y pos
				                        	player.transform.position.z));
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
	}

	void OnDrawGizmos()
	{
		// AI's fov
//		Matrix4x4 temp = Gizmos.matrix;
//		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
//		Gizmos.DrawFrustum(Vector3.zero, fieldOfViewAngle*0.5f, transform.localScale.x, 1, 1);
//		Gizmos.matrix = temp;
	}
}
