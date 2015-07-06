using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public bool paused = false;
	public Canvas pauseMenu;

	// Use this for initialization
	void Start () 
	{
		pauseMenu = pauseMenu.GetComponent<Canvas>();
		pauseMenu.enabled = false;
		Time.timeScale = 1.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("Escape"))
		{
			paused = togglePause();
		}

		if(paused)
		{
			pauseMenu.enabled = true;
		}
		else
		{
			pauseMenu.enabled = false;
		}
	}

	bool togglePause()
	{
		if(Time.timeScale == 0.0f)
		{
			Cursor.visible = false;
			Time.timeScale = 1.0f;
			return (false);
		}
		else
		{
			Cursor.visible = true;
			Time.timeScale = 0.0f;
			return (true);
		}
	}

	public void restartlevel()
	{
		Application.LoadLevel(Application.loadedLevelName);
	}

	public void MainMenu()
	{
		Application.LoadLevel("main_menu");
	}
}
