using UnityEngine;
using System.Collections;

public class EnemyAlert : MonoBehaviour {

	public float alert = 0;						// all AI's current alertness value		
	public float alarmValue = 50;				// value for AI to go into alert state
	public float ALERT_MAX = 100;
	float alertSpd = 0;							// alert inc/dec spd (+ve for inc, -ve for dec)
	public float alertMultiNormal = 120;		// alert inc/dec spd multiplier - 'accel.' (used when enemy not alerted)
	public float alertMultiLow = 200;			// " (used when enemy alerted, heard noise/lost sight of player)
	public float alertMultiHigh = 400;			// " (used when enemy alerted, player in sight)
	public float ALERTSPD_MAX = 500;			// max alert inc/dec spd
	public float playerNoiseRange = 10;			// if whatever noise made by player (incl. decoy) 
												// is within this radius, it will add to AI's alertness 
												// NOTE!!! add & use sphere col. to player if it doesnt give probs
	public bool isAlarmed = false;				// is true once AI's alert reaches alarmVal. Alert wont dec for a while, dec slowly
	public bool isAlarmedByDecoy = false;		// is true if source of noise made is far from player (e.g. decoy)
	public bool playerInSight = false;			// if true, isAlarmed will always be true - alert wont decrease until player is caught/gone. 
												// (value is set by EnemySight) 
	public bool playerInRange = false;			// will only be true if player in atk range and playerInSight

	public Vector3 targetPos;					// pos to move towards to (player/last seen pos)

	GameObject player;
	SphereCollider col;							// AI's sight/hearing rng
	
	Renderer debugRenderer;					// to access AI renderer for color chg to show alert state (debug only)


	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		col = gameObject.GetComponent<SphereCollider>();
		debugRenderer = transform.GetChild(0).GetComponent<Renderer>();
	}

	// updates the dec of alert. Inc of alert done in EnemyAI
	void Update () {
		if(alert > 0)
		{
			if(!isAlarmed)
			{
				// turn AI render color to green (!alarmed)
				debugRenderer.material.color = Color.green;

				alertSpd -= alertMultiLow*0.5f*Time.deltaTime;	// since AI not alarmed, alert dec is faster

				if(alertSpd < -ALERTSPD_MAX)
					alertSpd = -ALERTSPD_MAX;

				if(alert > 0)
				{
					alert += alertSpd * Time.deltaTime;

					if(alert <= 0)
					{
						alert = 0;
						alertSpd = 0;	// reset
					}
				}
			}
			else
			{
				// turn AI render color to red (alarmed)
				debugRenderer.material.color = Color.red;

				if(!playerInSight)
				{
					alertSpd -= alertMultiNormal*0.5f*Time.deltaTime;	// since AI is alarmed, alert dec is slower
					
					if(alertSpd < -ALERTSPD_MAX)
						alertSpd = -ALERTSPD_MAX;

					alert += alertSpd * Time.deltaTime;
					
					if(alert < alarmValue)
						isAlarmed = false;
				}

				// no alert dec if player is in sight
			}
		}
	}

	// alertness increment fn. (srcPos is pos of source for AI's alertness (e.g. src of noise, player pos, last seen pos) )
	public void AlertIncr(Vector3 srcPos) {
		// get dist of AI from src (the shorter the dist, the faster alert incr will be)
		float dist = (transform.position - srcPos).magnitude;	

		// if srcPos is not even in AI's sight/hearing rng
		if(dist > col.bounds.size.x*0.5f)	
			return;

		Vector3 playerPos = new Vector3(player.transform.position.x,
		                                transform.position.y,			// cos AI is a 'giant'..
		                                player.transform.position.z);

		// if player use decoy too close to himself (within AI's sight/hearing rng)
		// (is true if player is seen)
		if((playerPos - srcPos).magnitude < playerNoiseRange)
		{
			if(!playerInSight)	// i.e. player running near AI/cause noise (decoy) near AI 
			{
				if(!isAlarmed)
					alertSpd += alertMultiNormal*Time.deltaTime; 
				else
					alertSpd += alertMultiLow*Time.deltaTime; 
			}
			else
			{
				if(!playerInRange)
					alertSpd += alertMultiHigh*Time.deltaTime; 
				else
					alertSpd += (alertMultiLow+alertMultiHigh)*Time.deltaTime; 
			}

			
			if(alertSpd > ALERTSPD_MAX)
				alertSpd = ALERTSPD_MAX;

			// dist here was halved to make AI's 'hearing' more effective
			alert += (float)(alertSpd/dist*0.5f) * Time.deltaTime; 

			if(alert > alarmValue && !isAlarmed)	
			{
				isAlarmed = true;
			}

			if(alert > ALERT_MAX)
				alert = ALERT_MAX;
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
