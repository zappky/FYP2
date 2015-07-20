using UnityEngine;
using System.Collections;

public class FallResponse : MonoBehaviour {

	public float maxFallDist = 25.0f;		// max dist player can land safely
	float lastPosY = 0.0f;
	float fallDist = 0.0f;

	CharacterController cc;
	FpsMovement playerMovement;

	void Start () 
	{
		cc = GetComponent<CharacterController>();

		playerMovement = GetComponent<FpsMovement>();
	}

	void Update () 
	{
		float playerFeetPosY = transform.position.y - cc.bounds.size.y*0.5f;

		if(lastPosY > playerFeetPosY)					// if player is falling
			fallDist += lastPosY - playerFeetPosY;
		
		lastPosY = playerFeetPosY;

		if(playerMovement.useParachute || playerMovement.isGrappling)
			ResetFallDist();

		if(cc.isGrounded)
		{
			// check fall dist 
			if(fallDist >= maxFallDist)					// if fall ht too high
			{
				// reload from last checkpt
				ResetFallDist();
				Application.LoadLevel(Application.loadedLevelName);
				Inventory playerinventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
				playerinventory.ClearInventoryItem();
				LevelManager.Instance.loadFromContinue = true;
				LevelManager.Instance.LoadPlayerInfo();
			}
			else
			{
				ResetFallDist();
			}
		}
	}


	void ResetFallDist()
	{
		fallDist = lastPosY = 0;
	}
}