using UnityEngine;
using System.Collections;

public class CTimer : MonoBehaviour {
	public float timer  = 10.0f; // var to hold the time in sec
	public float previoustimer  = 10.0f;//var to hold the time in sec set previously,also the time which timer restore to.
	public float timerUpperLimit = 20.0f; //count up limit
	public float timerLowerLimit = 0.0f; // count down limit
	public bool operate   = false; // whether to update
	public bool alert   = false; // whether to ring alarm

	public enum TimerState
	{
		countdown
		,countup
	}
	public TimerState timerstate= TimerState.countdown;
	// Use this for initialization
	void Start () {
		operate = false;
	}
	
	// Update is called once per frame
	void Update () {

		if(operate == true)
		{
			switch(timerstate)
			{
			case TimerState.countdown:
				
				CountDown();
				break;
			case TimerState.countup:
				
				CountUp();
				break;
				
			}	
		}

	}
	public void CountDown()
	{
		timer -= Time.deltaTime;
		
		if(timer <= timerLowerLimit)
		{
			timer = timerLowerLimit;
			alert = true;
			operate = false;
		}
	}
	public void CountUp()
	{
		timer += Time.deltaTime;
		
		if(timer >= timerUpperLimit)
		{
			timer = timerUpperLimit;
			alert = true;
			operate = false;
		}
	}
	public void Reset()
	{
		timer = previoustimer;
		operate = true;
		alert = false;
	}
	public void SetTimer( float timesec)
	{
		previoustimer = timer = timesec;
	}
	public void OnGUI()
	{
		if(operate == true)
		{
			GUI.Box(new Rect(10,10,50,20), "" + timer.ToString("0"));
		}
	}
	public void ToggleOperation()
	{
		operate = !operate;
	}
	public void SetOperate( bool start )
	{
		operate = start;
	}
	public void OnLookInteract()//when being selected
	{
		SetOperate(true);
	}
}
