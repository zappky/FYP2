using UnityEngine;
using System.Collections;

public class EnemyAlert : MonoBehaviour {

	public float alert = 0;						// AI's current alertness value	
	public float alertAcc = 200;						
	public float ALARMED_VALUE = 50;			// value for AI to go into alarmed state
	public float ALERT_MAX = 100;				// gameover once alert = this value
	public float ALERTSPD_MAX = 15;				// max alert inc/dec spd (may be unnecessary)
	public float ALERT_DELAY = 5;				// delay (in s) to dec alertness value
	public float playerNoiseRange = 10;			// if whatever noise player makes (incl. decoy) 
												// is within this radius (on him), it will add to AI's alertness 
	
	int alertLevel;								// (range 1-5, alertSpd multiplier)
	float alertSpd = 0;	
	float alertDelay = 0;

	public bool isAlarmed = false;				// alert inc faster, wont dec for a while
	public bool isAlarmedByDecoy = false;		// is true if source of noise made is not within playerNoiseRange
	public bool playerInSight = false;			// if true, isAlarmed will always be true - alert wont decrease until player is caught/gone. 
												// (value is set by EnemySight) 
	public bool playerInRange = false;			// will only be true if player in atk range and playerInSight

	public Vector3 targetPos;					// pos to move towards to (player/last seen pos)

	float eyeHeight;
	GameObject player;
	SphereCollider col;							// AI's sight/hearing rng
	
	Renderer debugRenderer;					// to access AI renderer for color chg to show alert state (debug only)


	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		col = GetComponent<SphereCollider>();
		debugRenderer = GetComponentInChildren<Renderer>();

		eyeHeight = GetComponent<EnemySight>().eyeHeight;
	}

	// updates the dec of alert. Inc of alert done in EnemyAI
	void Update () {
		if(!isAlarmed)
		{
			// turn AI render color to green 
			debugRenderer.material.color = Color.green;
		}
		else
		{
			// turn AI render color to red 
			debugRenderer.material.color = Color.red;
		}

		if(alert < 0 || playerInSight)
			return;

		alertLevel = (int)(alert*0.05) + 1;
		
		// alert dec according to alertness levels(alertLvlMax-alertLvl, < alertLvl, > dec spd)
		alertSpd -= alertAcc*(5-alertLevel)*Time.deltaTime;
		if(alertSpd < -ALERTSPD_MAX)
			alertSpd = -ALERTSPD_MAX;
		
		if(alertDelay > 0)
		{
			alertDelay += alertSpd*Time.deltaTime;		// < alertLvl, < delay
			if(alertDelay < 0)
			{
				alertDelay = 0;
				alertSpd = 0;		// reset 
			}

			return;
		}

		alert += alertSpd * Time.deltaTime;
		if(alert < ALARMED_VALUE)
			isAlarmed = false;
		
		if(alert <= 0)
		{
			alert = 0;
			alertSpd = 0;	// reset
		}
	}

	// alertness increment fn. (srcPos is pos of source for AI's alertness (e.g. src of noise, player pos, last seen pos) )
	public void AlertIncr(Vector3 srcPos) {
		// transform.position = AI's feet pos
		Vector3 AIpos = transform.position+transform.up*eyeHeight*0.5f;
		// get dist of AI from src (the shorter the dist, the faster alert incr will be)
		float dist = (AIpos - srcPos).magnitude;	

		// if srcPos is not even in AI's sight/hearing rng
		if(dist > col.bounds.extents.x)	
			return;

		Vector3 playerPos = player.transform.position;

		// if player use decoy too close to himself (within AI's sight/hearing rng)
		// (is true if player is seen, since srcPos is playerPos)
		if((playerPos - srcPos).magnitude < playerNoiseRange)
		{
			alertLevel = (int)(alert*0.05) + 1;

			if(playerInSight)
				alertLevel += 1;		// extra inc if player in sight

			alertSpd += alertAcc*alertLevel; 
			if(alertSpd > ALERTSPD_MAX)
				alertSpd = ALERTSPD_MAX;

			// dist here was halved to make AI's 'hearing' more effective
			alert += (float)(alertSpd/dist*0.5f) * Time.deltaTime; 

			if(alert > ALARMED_VALUE && !isAlarmed)	
				isAlarmed = true;

			alertDelay = ALERT_DELAY;
		}
		else
		{
			isAlarmedByDecoy = true;	// will be set to false once AI reaches decoy's pos (done in EnemyAI)
		}

		// to do: every 5s then update targetPos, 
		// so that AI can create waypts to travel properly to target and go back once 
		// he reach/lose target
		// prob: if target is decoy how to move? (player also may give this prob)
		// basic soln (may nt be enuf): travel to waypt closest to target before going to target
		// (targetPos = waypt closest to decoy instead, targetPos = srcPos once waypt reached)
		targetPos = srcPos;
	}
}
