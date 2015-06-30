using UnityEngine;
using System.Collections;

public class ScreenManager : MonoBehaviour {
	public float prevScreenWidth;
	public float prevScreenHeight;
	public float currScreenWidth;
	public float currScreenHeight;
	private bool aspectchanged = false;
	// Use this for initialization
	void Start () {
		prevScreenWidth = currScreenWidth = Screen.width;
		prevScreenHeight = currScreenHeight = Screen.height;
	}
	
	// Update is called once per frame
	void Update () {

		currScreenWidth = Screen.width;
		currScreenHeight = Screen.height;
	
		if(prevScreenWidth == currScreenWidth && prevScreenHeight == currScreenHeight)
		{
			aspectchanged = false;
		}else
		{
			aspectchanged = true;

			if( prevScreenWidth != currScreenWidth)
			{
				prevScreenWidth = currScreenWidth;
			}		
			if( prevScreenHeight != currScreenHeight)
			{
				prevScreenHeight = currScreenHeight;
			}
		}
		//print ("aspect changed?" + aspectchanged);
	}

	public bool CheckAspectChanged()
	{
		return aspectchanged;
	}
}
