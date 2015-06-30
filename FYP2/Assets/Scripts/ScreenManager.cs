using UnityEngine;
using System.Collections;

public class ScreenManager : MonoBehaviour {
	public float prevScreenWidth;
	public float prevScreenHeight;
	public float currScreenWidth;
	public float currScreenHeight;
	private bool screenAspectChanged = false;

	public static ScreenManager instance = null;

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
		prevScreenWidth = currScreenWidth = Screen.width;
		prevScreenHeight = currScreenHeight = Screen.height;
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

	public float CurrrentScreenWidth
	{
		get
		{
			return this.currScreenWidth;
		}
	}
	public float CurrrentScreenHeight
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
