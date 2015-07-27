using UnityEngine;
using System.Collections;

public class EnemyStats : MonoBehaviour {
	public float moveSpeed = 20.0f;
	public float rotationSpeed = 450.0f;		// AI's turning spd
	public float alertSightMoveSpeed = 2.0f;	// move spd multiplier for when in alert state (due to sight of player)
	public float alertNoiseMoveSpeed = 1.2f;	// move spd multiplier for when in alert state (due to noise)
	public float IDLE_DELAY = 2.0f;				// how long AI stays in idle before roaming 
	public float ALERT_DELAY = 5.0f;			// how long AI stays in alert state before resetting (e.g. after losing sight of player) 
	public float attackRange = 2.0f; 			// AI's range of reach 
	public float toleranceLength = 10.0f;		// the length of which to determine whether AI has reach the waypoint
}