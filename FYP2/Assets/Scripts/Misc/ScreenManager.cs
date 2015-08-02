using UnityEngine;
using System.Collections;

public class ScreenManager : MonoBehaviour {
	public float prevScreenWidth;
	public float prevScreenHeight;
	public float currScreenWidth;
	public float currScreenHeight;
	private bool screenAspectChanged = false;

	public static ScreenManager instance = null;
	public static bool initedBefore = false;

	public static ScreenManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("Screen Manager").AddComponent<ScreenManager>();
				DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}
	public void Initialize()
	{
		Initialize(true);// allow reinitalize of this class by default
	}
	
	public void Initialize(bool re_init)
	{
		if(initedBefore == false || re_init == true)
		{
			prevScreenWidth = currScreenWidth = Screen.width;
			prevScreenHeight = currScreenHeight = Screen.height;
			initedBefore = true;
		}
	}
	
	public void OnApplicationQuit()
	{
		DestroyInstance();
	}
	
	public void DestroyInstance()
	{
		instance = null;
	}
	
	// Update is called once per frame
	void Update () {

		currScreenWidth = Screen.width;
		currScreenHeight = Screen.height;
	
		if(prevScreenWidth == currScreenWidth && prevScreenHeight == currScreenHeight)
		{
			screenAspectChanged = false;
		}else
		{
			screenAspectChanged = true;

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

	public float CurrentScreenWidth
	{
		get
		{
			return this.currScreenWidth;
		}
	}
	public float CurrentScreenHeight
	{
		get
		{
			return this.currScreenHeight;
		}
	}

	public bool CheckAspectChanged()
	{
		return screenAspectChanged;
	}
}
