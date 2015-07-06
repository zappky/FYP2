using UnityEngine;
using System.Collections;

public class volumecontrol : MonoBehaviour 
{
	public static float hSliderValue = 1.0f;
	public static bool bMute = false;
	public static AudioSource BGM;
	private GUISkin volumeslider;
	public Vector2 pos;
	public int width = 250;
	public int height = 20;

	public MainMenu MMscript;

	// Use this for initialization
	void Start () 
	{
		BGM = this.GetComponent<AudioSource>();

		volumeslider = Resources.Load ("slider") as GUISkin;
	}

	void adjustvolume()
	{
		hSliderValue = GUI.HorizontalSlider (new Rect(pos.x + 400, pos.y + 300, width, height),hSliderValue, 0.0f, 1.0f);
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

	}
	
	// Update is called once per frame
	void Update () 
	{
		BGM.GetComponent<AudioSource>().volume = hSliderValue;
	}	
}
