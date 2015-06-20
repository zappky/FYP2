using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour 
{
	public float fieldOfViewAngle = 110f; 	// how far AI can see 
	public bool playerInSight;

	public Vector3 targetPos;				// pos to move towards to (player/last seen pos)

	Vector3 lastSeenPos;					// player last seen pos
	SphereCollider col;
	GameObject player;
	//EnemyAlert alert;						// AI's alertness calculations

	Renderer debugRenderer;					// to access AI renderer for color chg to show alert state (debug only)

	void Start() {
		col = GetComponent<SphereCollider>();
		player = GameObject.FindGameObjectWithTag("Player");
		debugRenderer = gameObject.GetComponent<Renderer>();
	}


	void Update() {
	}


	void OnTriggerStay(Collider other)
	{
		// if player enters sphere col
		if(other.gameObject == player)
		{
			playerInSight = false;		// reset

			Vector3 dir = other.transform.position - transform.position+transform.up;
			float angle = Vector3.Angle(dir, transform.forward);

			// debug ray (line of view)
			Ray ray = new Ray(transform.position, dir);	
			
			// check if player is within AI's fov  
			if(angle < fieldOfViewAngle * 0.5f)
			{ 
				RaycastHit hit;
				if(Physics.Raycast(transform.position, dir.normalized, out hit, transform.localScale.x*col.radius))
				{
					// if ray hits player (i.e. AI spotted player)
					if(hit.collider.gameObject == player)
					{
						// add alertness
						// alert.increase();
						// if alert value > sth, chase player
						playerInSight = true;
						targetPos = new Vector3(player.transform.position.x,
						                        transform.position.y,			// since AI is giant, use back his own y pos
						                        player.transform.position.z);
						lastSeenPos = targetPos;

						// turn AI render color to red (alert)
						debugRenderer.material.color = Color.red;
						Debug.DrawRay(ray.origin, ray.direction*transform.localScale.x*col.radius, Color.green, 1);
					}
					//in fov, but gt obj blocking sight
					else
					{
						targetPos = lastSeenPos;	// AI would move to last seen pos
						
						// reset AI render color to green (!alert)
						debugRenderer.material.color = Color.green;	
						Debug.DrawRay(ray.origin, ray.direction*transform.localScale.x*col.radius, Color.red, 1);
					}
				}
			}
			// not in AI's fov
			else
			{
				// if player running or make noise, add alertness

				debugRenderer.material.color = Color.green;
				Debug.DrawRay(ray.origin, ray.direction*transform.localScale.x*col.radius, Color.black, 1);
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		gameObject.GetComponent<Renderer>().material.color = Color.green;
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
