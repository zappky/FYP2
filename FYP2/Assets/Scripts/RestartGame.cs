﻿using UnityEngine;
using System.Collections;

public class RestartGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Escape"))
		{
			LevelManager.Instance.Initialize();
			LevelManager.Instance.loadFromContinue = true;
			Application.LoadLevel("main-scene"); 

			//LevelManager.Instance.LoadPlayerInfo();
		}
	}
}
