using UnityEngine;
using System.Collections;

public class SoundEffect : MonoBehaviour {

	public AudioSource sound; 

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void PlaySound()
	{
		sound.Play();
	}
	
	public void StopSound()
	{
		sound.Stop ();
	}
}
