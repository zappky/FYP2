using UnityEngine;
using System.Collections;

// this script is suppose to attach to a blank object and initalize the singeton in the game
public class SingetonSetUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		FileManager.Instance.Initialize();
		ItemDatabase.Instance.Initialize();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
