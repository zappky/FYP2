using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour {

	public string playername = "hentaiislove";
	public float weight = 45; //in kg
	public bool gender = false; // false means girl

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string PrintGender()
	{
		if (gender == false)
		{
			return "female";
		}else
		{
			return "male";
		}
	}
}
