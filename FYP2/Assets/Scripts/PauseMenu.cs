using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public bool paused = false;
	Canvas pauseMenu;
	GameObject Player;

	// Use this for initialization
	void Start () 
	{
		Player = GameObject.FindGameObjectWithTag("Player");
		pauseMenu = GetComponent<Canvas>();
		pauseMenu.enabled = false;
		Time.timeScale = 1.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("Escape"))
		{
			paused = togglePause();
			Player.GetComponent<CastSlot>().ToggleDisplay();
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
	
	public void reloadCheckpoint()
	{
		Inventory playerinventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
		playerinventory.ClearInventoryItem();
		LevelManager.Instance.LoadPlayerInfo();
	}

	public void restartlevel()
	{
		LevelManager.Instance.loadFromContinue = false;
		LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevelName, true);
	}

	public void MainMenu()
	{
		Application.LoadLevel("main_menu");
	}
}
