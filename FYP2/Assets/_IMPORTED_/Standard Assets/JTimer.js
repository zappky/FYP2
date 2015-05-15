#pragma strict

public var timer :float = 10.0f; // var to hold the time in sec
public var previoustimer: float = timer;//var to hold the time in sec set previously,also the time which timer restore to.
public var timerUpperLimit = 20.0f; //count up limit
public var timerLowerLimit = 0.0f; // count down limit
public var operate :boolean  = false; // whether to update
public var alert :boolean  = false; // whether to ring alarm

enum TimerState
{
	countdown
	,countup
}
public var timerstate:TimerState = TimerState.countdown;

function Start () {

}

function CountDown()
{
	timer -= Time.deltaTime;
	
	if(timer <= timerLowerLimit)
	{
		timer = timerLowerLimit;
		alert = true;
		operate = false;
	}
}
function CountUp()
{
	timer += Time.deltaTime;
	
	if(timer >= timerUpperLimit)
	{
		timer = timerUpperLimit;
		alert = true;
		operate = false;
	}
}
function Reset()
{
	timer = previoustimer;
	operate = true;
	alert = false;
}
function SetTimer( timesec:float)
{
	previoustimer = timer = timesec;
}
function OnGUI()
{
	if(operate == true)
	{
		GUI.Box(new Rect(10,10,50,20), "" + timer.ToString("0"));
	}
}
function ToggleOperation()
{
	operate = !operate;
}
function SetOperate( start:boolean)
{
	operate = start;
}
function OnLookInteract()//when being selected
{
	SetOperate(true);
}
function Update () {

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