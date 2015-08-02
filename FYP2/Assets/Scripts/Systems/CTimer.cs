using UnityEngine;
using System.Collections;

//this script is to make a alarm clock item funtional

[System.Serializable]
public class my_rectdata// this class to to hold information for unity rect class, to ease data handling
{
	public float left = 10.0f;
	public float top = 10.0f;
	public float width = 50.0f;
	public float height = 20.0f;

	public my_rectdata ()
	{
	}
	public my_rectdata (my_rectdata another)
	{
		this.left = another.left;
		this.top = another.top;
		this.width = another.width;
		this.height = another.height;
	}
	public void ClearAll()
	{
		left = top = width = height = 0.0f;
	}
	public void EqualisedWidthAndHeight()//used for like making the rect to become a square
	{
		width = height = (width + height)*0.5f;
	}
}

[System.Serializable]
public class my_timedata {
	public float minute_base = 60.0f;
	public float minute_tick = 1.0f;
	public float second = 0;
	public float minute = 0;

	public my_timedata (float minute ,float second)
	{
		this.minute = minute;
		this.second = second;
	}
	public my_timedata (my_timedata another)
	{
		this.minute_base = another.minute_base;
		this.minute_tick = another.minute_tick;
		this.minute = another.minute;
		this.second = another.second;
	}
	public float GetTotalTimeInSecond()
	{
		return minute * minute_base + second;
	}
	
	public float GetTotalTimeInMinute()
	{
		return minute + second/minute_base;
	}
	
	public void FlattenMinuteToSecond()
	{
		second += minute*minute_base;
		minute = 0.0f;
	}
	
	public void SetTime(float min,float sec)
	{
		minute = min;
		second = sec;
	}
	
	public void ClearAll()
	{
		minute = second = 0.0f;
	}
	public void SetMinuteToSecond(float min)
	{
		this.minute = min;
		FlattenMinuteToSecond();
	}
	public void SetSecondSafe(float sec)
	{
		while(sec >minute_base)
		{
			++minute;
			sec -= minute_base;
		}
		second += sec;
	}
	
	public void SetEqual(my_timedata another)
	{
		this.second = another.second;
		this.minute = another.minute;
	}
}

public class CTimer :MonoBehaviour {
	public my_timedata timer; // var to hold the time 
	public my_timedata previoustimer;//var to hold the time in sec set previously,also the time which timer restore to.
	public my_timedata timerUpperLimit ; //count up limit
	public my_timedata timerLowerLimit ; // count down limit
	public bool operate   = false; // whether to update
	public bool display = false;  // whether to draw the ui
	public bool alert   = false; // whether to ring alarm
	public Rect timerRect;
	public float timerWidth = 50.0f;
	public float timerHeight = 20.0f;


	//public my_timedata resetTimer;
	public my_timedata resetDelay = new my_timedata(0.0f,5.0f);//amount of wait time before reset timer.
	public SoundEffect sound ;
	public enum TimerState
	{
		countdown
		,countup
	}
	public enum TimerFormat
	{
		SEC,
		MIN_SEC,
	}
	public TimerState timerstate = TimerState.countdown;
	public TimerFormat timerformat = TimerFormat.MIN_SEC;
	// Use this for initialization
	void Start () {
		//operate = false;
		sound = this.GetComponent<SoundEffect>();		
		timerLowerLimit.second = 0.0f;
		UpdateTimerRect();

		switch(this.tag)//configuration,cos i want to reuse this script as game clock
		{
			default:
			break;
			
			case "Alarm":
				previoustimer.second = timer.second = 10.0f;	
				timerformat = TimerFormat.MIN_SEC;
				timerstate= TimerState.countdown;

				timerRect = new Rect(Screen.width*0.01f,Screen.height*0.01f,timerWidth,timerHeight);
			break;
			
			case "Game Session":
				timerRect = new Rect(Screen.width - timerWidth,Screen.height*0.01f,timerWidth,timerHeight);
				
				previoustimer.second = timer.second = 0.0f;
				
				timerUpperLimit.minute = 99.0f;//some ridiculous time
				timerUpperLimit.second = 59.0f;
				
				timerformat = TimerFormat.MIN_SEC;
				timerstate= TimerState.countup;
				
				SetOperate(true);
			break;
		}
	}
	void UpdateTimerRect()
	{
		timerWidth = Screen.width *0.05f;
		//timerHeight = timerWidth*0.2f;
	}
	void UpdateDisplayRect()
	{
		UpdateTimerRect();
		switch(this.tag)
		{
		default:
			break;
			
		case "Alarm":			
			timerRect = new Rect(Screen.width*0.01f,Screen.height*0.01f,timerWidth,timerHeight);
			break;
			
		case "Game Session":
			timerRect = new Rect(Screen.width - timerWidth,Screen.height*0.01f,timerWidth,timerHeight);
			break;
		}
	}
	// Update is called once per frame
	void Update () {

		if(operate == true)
		{
			if(alert == false)//on normal routine
			{
				switch(timerstate)
				{
				case TimerState.countdown:	

					CountDown();

					if(CheckReachTimeLimit(timerstate) == true)
					{
						ActivateEffect();
						display = false;
					}

					break;
				case TimerState.countup:

					CountUp();

					if(CheckReachTimeLimit(timerstate) == true)
					{
						ActivateEffect();
						display = false;
					}

					break;
					
				}	
			}else//on reset timer routine
			{
				CountUp();
				//print ("counting reset timer");
				if( timer.GetTotalTimeInSecond() >= resetDelay.GetTotalTimeInSecond() == true)
				{
					//print ("resetting timer");
					Reset();
				}
			}

		}

	}
	private void StopSoundEffect()
	{
		if (sound.isActiveAndEnabled == true)
		{
			sound.StopSound();
		}
	}
	private void ActivateEffect()
	{
		sound.PlaySound();
		alert = true;
	}
	public bool CheckReachTimeLimit(TimerState state)
	{
		switch (state)
		{
			case TimerState.countdown:
				return CheckReachTimeLimit(false);

			case TimerState.countup:
				return CheckReachTimeLimit(true);

		}
		return false;
	}
	public bool CheckReachTimeLimit(bool limitmode)
	{
		if(limitmode == true) //test for upperlimit
		{
			if(timer.GetTotalTimeInSecond() >= timerUpperLimit.GetTotalTimeInSecond())
			{
				timer.SetEqual(timerUpperLimit);
				return true;
			}
		}else//test for lowerlimit
		{
			if(timer.GetTotalTimeInSecond() <= timerLowerLimit.GetTotalTimeInSecond())
			{
				timer.SetEqual(timerLowerLimit);
				return true;
			}
		}

		return false;
	}
	public void CountDown()
	{	
		switch(timerformat)
		{
			case TimerFormat.MIN_SEC:
			{
				if(timer.second >0.0f)
				{
					timer.second -= Time.deltaTime;
				}else 
				{	
					if(timer.minute >= timer.minute_tick)
					{
						timer.minute -= timer.minute_tick;
						timer.second = timer.minute_base - Time.deltaTime;
					}else
					{
						timer.ClearAll();
					}
				}	
			}
			break;
	
			default:
			case TimerFormat.SEC:
			{
				if(timer.second >0.0f)
				{
					timer.second -= Time.deltaTime;
				}else
				{
					timer.second = 0.0f;
				}
			}
			break;
			
		}
	}
	public void CountUp()
	{

		switch(timerformat)
		{
			case TimerFormat.MIN_SEC:
			{
				if(timer.second <timer.minute_base)
				{
					timer.second += Time.deltaTime;
				}else 
				{	
					timer.second -= timer.minute_base;
					timer.minute += timer.minute_tick;
				}	
				
			}break;
			
			default:
			case TimerFormat.SEC:
			{
				timer.second += Time.deltaTime;
			}break;
		}
	}
	public void Reset()
	{
		timer.SetEqual(previoustimer);
		SetOperate(false);
		alert = false;
		StopSoundEffect();
	}
	public void SetTimer( float time_min,float time_sec)
	{
		switch (timerformat)
		{
			default:
			case TimerFormat.SEC:
			{
				previoustimer.second = timer.second = time_sec;
			}break;
			
			case TimerFormat.MIN_SEC:
			{
				previoustimer.minute = timer.minute = time_min;
				previoustimer.SetSecondSafe(time_sec);
				timer.SetSecondSafe(time_sec);
			}
			break;
		}

	}
	public void OnGUI()
	{
		if(operate == true)
		{
			if(display == true)
			{
				if(ScreenManager.Instance.CheckAspectChanged() == true)
				{
					UpdateDisplayRect();
				}

				switch (timerformat)
				{
				default:
				case TimerFormat.SEC:
				{
					
					//rect.left
					GUI.Box(timerRect, "" + timer.second.ToString("0"));
				}break;
					
				case TimerFormat.MIN_SEC:
				{
					
					if(Mathf.Round(timer.second) <= 9.0f)
					{
						GUI.Box(timerRect,timer.minute.ToString("f0") + ":0" + timer.second.ToString("f0"));
					}
					else
					{
						GUI.Box(timerRect, timer.minute.ToString("f0") + ":" + timer.second.ToString("f0"));
					}
				}break;
				}
			}
		}
	}
	public void ToggleOperation()
	{
		operate = !operate;
	}
	public void ToggleDisplay()
	{
		display = !display;
	}
	public void SetOperate( bool start )
	{
		display = operate = start;
	}
	public void OnLookInteract()//when being selected
	{
		SetOperate(true);

	}
}
