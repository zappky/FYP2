using UnityEngine;
using System.Collections;

//this script will hold persistent data about the whole game about player interface stuff
public class ApplicationPlayerBoard : MonoBehaviour {

	public static ApplicationPlayerBoard instance = null;
	public static bool initedBefore = false;

	public static ApplicationPlayerBoard Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("Application Player Board").AddComponent<ApplicationPlayerBoard>();
				DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}
	public void Initialize()
	{
		Initialize(false);//don allow reinitalize of this class by default
	}
	public void Initialize(bool re_init)
	{
		if(initedBefore == false || re_init == true)
		{
			instance.gameObject.AddComponent<HideCursorScript>();
			initedBefore = true;
		}
	}
	
	public void LoadGameLevelInformation()
	{
		
	}
	
	public void SaveGameLevelInformation()
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
