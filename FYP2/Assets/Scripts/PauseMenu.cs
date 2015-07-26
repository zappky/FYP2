using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public bool paused = false;
	Canvas pauseMenu;
	GameObject Player;

	AudioSource clickSound;

	void Start () 
	{
		Player = GameObject.FindGameObjectWithTag("Player");
		pauseMenu = GetComponent<Canvas>();
		pauseMenu.enabled = false;
		Time.timeScale = 1.0f;
		clickSound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("Escape"))
		{
			if(DialogInterface.Instance.display)
			{
				DialogInterface.Instance.display = false;
				Player.GetComponent<CastSlot>().display = true;
			}
			else if(Player.GetComponent<Inventory>().display)
			{
				Player.GetComponent<Inventory>().display = false;
			}
			else
			{
				paused = togglePause();
			}
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
			Time.timeScale = 1.0f;
			Player.GetComponent<CastSlot>().display = true;
			return (false);
		}
		else
		{
			Time.timeScale = 0.0f;
			Player.GetComponent<CastSlot>().display = false;
			Player.GetComponent<CSelect>().display = false;
			Player.GetComponent<Inventory>().display = false;
			return (true);
		}
	}
	
	public void reloadCheckpoint()
	{
		playClickSound();
		Application.LoadLevel(Application.loadedLevelName);
		Inventory playerinventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
		playerinventory.ClearInventoryItem();
		LevelManager.Instance.loadFromContinue = true;
		LevelManager.Instance.LoadPlayerInfo();

	}

	public void restartlevel()
	{
		playClickSound();
		LevelManager.Instance.loadFromContinue = false;
		LevelManager.Instance.LoadLevel(Application.loadedLevelName, true);
	}
	public void MainMenu()
	{
		playClickSound();
		Application.LoadLevel("main_menu");
	}

	void playClickSound()
	{
		clickSound.Play();
	}
}
