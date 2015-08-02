using UnityEngine;
using System.Collections;

// handles response of an interactable(movable) object's collision with another object
public class ObjectCollisionResponse : MonoBehaviour {

	public enum ObjectType
	{
		// (least alert) - if there's time to do so, add more alert depending on obj type
		PLASTIC,
		WOOD,	
		METAL,
		FRAGILE		// breaks once dropped  
		// (highest alert)
	}
	public ObjectType objType = ObjectType.PLASTIC; 	 
	public AudioClip hitSFX = null;		// sfx for obj when it collides w sth
	public AudioClip dropSFX = null;	// sfx for obj when it lands on sth

	public float SFX_DELAY = 0.2f;		// to prevent spam sfx
	float SFXdelay;

	bool wasDropped;					// to check if obj was dropped
	float distToGround;					// dist from this obj's ctr is to its btm end
	EnemyManager AImanager;				// to add alertness to nearby AIs when this obj collides
	AudioSource sfxSource;

	void Start () {
		wasDropped = false;
		SFXdelay = 0;

		// get the distance to ground
		distToGround = GetComponent<Collider>().bounds.extents.y;

		AImanager = (GameObject.FindGameObjectWithTag("AImanager")).GetComponent<EnemyManager>();

		sfxSource = gameObject.AddComponent<AudioSource>();
		sfxSource.clip = hitSFX;
	}

	void Update () {
		if(SFXdelay > 0)
			SFXdelay -= Time.deltaTime;

		if(!isGrounded())
			wasDropped = true;	// object was dropped
	}

	bool isGrounded() {
		// check if obj is grounded by raycasting obj btm to ground
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}

	void OnCollisionEnter(Collision other)
	{
		if(!wasDropped)	 
		{
//			// if this obj collides w other non-movable objects in the world
//			if((other.rigidbody == null 
//			|| (other.rigidbody.isKinematic && other.gameObject.tag != "Player")))
//				return;
//
//			switch(objType)
//			{
//				case ObjectType.PLASTIC:
//					break;
//				case ObjectType.WOOD:
//					break;
//				case ObjectType.METAL:
//					break;
//				case ObjectType.FRAGILE:
//					break;
//			}
//
//			if(SFXdelay <= 0)	 
//			{
//				SFXdelay = SFX_DELAY;
//				
//				if(hitSFX != null)
//					AudioSource.PlayClipAtPoint(hitSFX, transform.position);
//			}
//
//			
//			AImanager.AddAlertGlobally(transform.position);
		}
		else
		{
			wasDropped = false;		//reset since this obj landed on the floor/other obj

			if(SFXdelay <= 0)	 
			{
				SFXdelay = SFX_DELAY;

				if(dropSFX != null)
					AudioSource.PlayClipAtPoint(dropSFX, transform.position);
			}
			
			AImanager.AddAlertGlobally(transform.position);

			// add diff alert based on type if gt time
			switch(objType)
			{
				case ObjectType.PLASTIC: 
					break;
				case ObjectType.WOOD:
					break;
				case ObjectType.METAL:
					break;
				case ObjectType.FRAGILE:
					Destroy(this.gameObject);
					break;
			}
		}
	}

	public void playCollisionSFX()
	{
		if(!sfxSource.isPlaying)
			sfxSource.Play();
	}					
}
