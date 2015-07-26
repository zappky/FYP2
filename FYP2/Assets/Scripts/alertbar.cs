using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class alertbar : MonoBehaviour {

	public Image ab;//alertbar

	EnemyManager AImanager;

	void Start () 
	{
		if(ab == null)
			ab = GameObject.Find("ab bar").GetComponent<Image>();

		ab.fillAmount = 0.0f; 

		AImanager = (GameObject.FindGameObjectWithTag("AImanager")).GetComponent<EnemyManager>();
	}

	void Update () 
	{
		// get alertness of nearest AI
		ab.fillAmount = AImanager.GetClosestAlertness()*0.01f;
		Mathf.Clamp(ab.fillAmount, 0.0f, 1.0f);
	}
}
