using UnityEngine;
using System.Collections;

public class PointSystem : MonoBehaviour {

	public CTimer gameSession;
	public float points = 0;
	public float gameSessionThreshold = 30.0f;//in minute
	public bool display = false;
	// Use this for initialization
	void Start () {
		gameSession = GameObject.FindGameObjectWithTag("Game Session").GetComponent<CTimer>();
	}
	
	// Update is called once per frame
	void Update () {
		CalculateAllPoint();
	}
	public float ConvertFloatToSecond(float value)
	{
		return value * gameSession.timer.minute_base;
	}
	public void CalculateAllPoint()
	{
		points = CalculatePointFromGameSession() + CalculatePointFromAchievement();
	}

	public float CalculatePointFromGameSession()
	{			
		return (ConvertFloatToSecond(gameSessionThreshold) / gameSession.timer.GetTotalTimeInSecond()) * (ConvertFloatToSecond(gameSessionThreshold));	
	}
	public float CalculatePointFromAchievement()
	{			
		return 0.0f;
	}
	
	void OnGUI()
	{
		if(display)
		{
			GUI.Box(new Rect(Screen.width*0.9f,Screen.height*0.9f,60,20),points.ToString("0"));
		}
	}
}
