using UnityEngine;
using System.Collections;

public class volumecontrol : MonoBehaviour 
{
	public static float hSliderValue = 1.0f;
	public static bool bMute = false;
	public static AudioSource BGM;
	private GUISkin volumeslider;
	public Vector2 pos;
	public float width = 250;
	public float height = 20;

	public MainMenu MMscript;
	public PauseMenu PMscript;

	// Use this for initialization
	void Start () 
	{
		BGM = this.GetComponent<AudioSource>();

		volumeslider = Resources.Load ("Skins/slider") as GUISkin;

		if(LevelManager.Instance.CurrentLevelName == "Level1" || LevelManager.Instance.CurrentLevelName == "Level2")
			PMscript = GameObject.FindObjectOfType<PauseMenu>();
		else
			MMscript = GameObject.FindObjectOfType<MainMenu>();

		width *= (float)Screen.width/Screen.height;
	}

	void adjustvolume()
	{
		hSliderValue = GUI.HorizontalSlider (new Rect((Screen.width  * 0.5f) - width*0.5f, (Screen.height * 0.5f), width, height), hSliderValue, 0.0f, 1.0f, volumeslider.GetStyle("horizontalslider"), volumeslider.GetStyle("horizontalsliderthumb"));
	}

	void OnGUI()
	{
		if(LevelManager.Instance.CurrentLevelName == "main_menu")
		{
			GUI.skin = volumeslider;
			if(MMscript.GetComponent<MainMenu>().optionMenu.enabled)
			{
				adjustvolume();
			}
		}

		if(LevelManager.Instance.CurrentLevelName == "Level1" || LevelManager.Instance.CurrentLevelName == "Level2")
		{
			GUI.skin = volumeslider;
			if(PMscript.GetComponent<PauseMenu>().optionMenu.enabled)
			{
				adjustvolume();
			}
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		BGM.GetComponent<AudioSource>().volume = hSliderValue;
	}	
}
