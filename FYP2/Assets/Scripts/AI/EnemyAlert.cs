using UnityEngine;
using System.Collections;

public class EnemyAlert : MonoBehaviour {

	public float alert = 0;					// AI's current alertness value		
	public float alarmValue = 50;			// value for AI to go into alert state
	public float ALERT_MAX = 100;
	float alertSpd = 0;						// alert inc/dec spd (+ve for inc, -ve for dec)
	public float alertMultiNormal = 1.2f;	// alert inc/dec spd multiplier - 'accel.' (used when enemy not alerted)
	public float alertMultiLow = 2;			// " (used when enemy alerted, heard noise/lost sight of player)
	public float alertMultiHigh = 4;		// " (used when enemy alerted, player in sight)
	public float ALERTSPD_MAX = 5;			// max alert inc/dec spd
	
	public bool isAlarmed = false;			// is true once AI's alert reaches alarmVal. Alert wont dec for a while, dec slowly
	public bool playerInSight = false;		// if true, isAlarmed will always be true - alert wont decrease until player is caught/gone. 
											// (value is set by EnemyAI) 

	SphereCollider col = null;						// AI's sight/hearing rng

	// updates the dec of alert. Inc of alert done in EnemyAI
	void Update () {
		if(!isAlarmed)
		{
			alertSpd -= alertMultiHigh*Time.deltaTime;	// since AI not alarmed, alert dec is faster

			if(alertSpd < -ALERTSPD_MAX)
				alertSpd = -ALERTSPD_MAX;

			if(alert > 0)
			{
				alert += alertSpd * Time.deltaTime;

				if(alert < 0)
				{
					alert = 0;
					alertSpd = 0;	// reset
				}
			}
		}
		else
		{
			if(!playerInSight)
			{
				alertSpd -= alertMultiNormal*Time.deltaTime;	// since AI is alarmed, alert dec is slower
				
				if(alertSpd < -ALERTSPD_MAX)
					alertSpd = -ALERTSPD_MAX;

				alert += alertSpd * Time.deltaTime;
				
				if(alert < alarmValue)
					isAlarmed = false;
			}
			// no need for alert dec if player is in sight
		}
	}

	// alertness increment fn. (srcPos is pos of source for AI's alertness (e.g. src of noise, player pos, last seen pos) )
	public void AlertIncr(Vector3 srcPos) {
		// get dist of AI from src (the shorter the dist, the faster alert incr will be)
		float dist = (transform.position - srcPos).magnitude;	

		if(dist > col.bounds.size.x*0.5f)	// if for some reason alertIncr is called when src is not even in AI's sight/hearing rng
			return;

		if(!playerInSight)
		{
			if(!isAlarmed)
				alertSpd += alertMultiNormal*Time.deltaTime; 
			else
				alertSpd += alertMultiLow*Time.deltaTime; 
		}
		else
			alertSpd += alertMultiHigh*Time.deltaTime; 

		
		if(alertSpd > ALERTSPD_MAX)
			alertSpd = ALERTSPD_MAX;

		alert += (float)(alertSpd/dist) * Time.deltaTime;

		if(alert > alarmValue && !isAlarmed)			
			isAlarmed = true;

	}
}
