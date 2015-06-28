using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class alertbar : MonoBehaviour {

	public Image ab;//alertbar

	FpsMovement fpsScript;
	EnemyManager AImanager;

	void Start () 
	{
		fpsScript = GetComponent<FpsMovement> ();

		if(ab == null)
			ab = GameObject.Find("ab bar").GetComponent<Image>();

		ab.fillAmount = 0.0f; 

		AImanager = (GameObject.FindGameObjectWithTag("AImanager")).GetComponent<EnemyManager>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// get alertness of nearest AI
		ab.fillAmount = AImanager.GetClosestAlertness()*0.01f;
		// old testing method
//		ab.fillAmount -= 0.01f * Time.deltaTime;
//
//		if(fpsScript.runSound.isPlaying)
//			ab.fillAmount += 0.003f;
//
//		if(ab.fillAmount < 0.0f)
//			ab.fillAmount = 0.0f;
//
//		if(ab.fillAmount >= 1.0f)
//			Application.LoadLevel("losescreen");
	}

	// old testing method
//	void OnControllerColliderHit(ControllerColliderHit col)
//	{
//		if (col.transform.tag == "Alert")
//		{
//			ab.fillAmount += 0.3f * Time.deltaTime;
//		}
//	}
}
