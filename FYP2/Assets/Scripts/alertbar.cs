using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class alertbar : MonoBehaviour {

	Transform bar;	//alertbar
	EnemyManager AImanager;

	void Start () 
	{
		bar = GameObject.FindGameObjectWithTag("Alert").transform;
		bar.localScale = new Vector3(0.0f, bar.localScale.y, bar.localScale.z);

		AImanager = (GameObject.FindGameObjectWithTag("AImanager")).GetComponent<EnemyManager>();
	}

	void Update () 
	{
		// get alertness of nearest AI
		float alertValue = AImanager.GetClosestAlertness()*0.01f;
		Mathf.Clamp(alertValue, 0.0f, 1.0f);

		if(alertValue > 0.5f)
			bar.GetComponent<Image>().color = new Color(1, 0, 0, 0.8f);
		else
			bar.GetComponent<Image>().color = new Color(0, 0.75f, 1, 0.8f);

		bar.localScale = new Vector3(alertValue, bar.localScale.y, bar.localScale.z);
	}
}
