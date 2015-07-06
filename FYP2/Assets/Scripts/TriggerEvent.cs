using UnityEngine;
using System.Collections;

public class TriggerEvent : MonoBehaviour {

	public float SFXdelay = 0;
	public AudioClip eventSFX;

	bool playSFX;
	Animator animationEvent;

	void Start() 
	{
		playSFX = false;
		animationEvent = transform.parent.GetComponent<Animator>();
	}
	
	void Update () 
	{
		if(playSFX)
		{
			SFXdelay -= Time.deltaTime;
			if(SFXdelay <= 0)
			{
				if(eventSFX != null)
				{
					playSFX = false;
					AudioSource.PlayClipAtPoint(eventSFX, transform.parent.position);
				}
			}
		}
	}
	
	public void PlayEvent()
	{
		if(!animationEvent.enabled)
		{
			animationEvent.enabled = true;			// cheat way... dk how play once - research hw ltr
			playSFX = true;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			if(!animationEvent.enabled)
			{
				animationEvent.enabled = true;
				playSFX = true;
			}
		}
	}
}
