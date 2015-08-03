using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class alertbar : MonoBehaviour {

	public bool display = true;
	Transform bar;		//alertbar
	Transform barBG;	// alertbar BG
	EnemyManager AImanager;
	float xScale;	// bar's orig x scale

	void Start () 
	{
		bar = GameObject.FindGameObjectWithTag("Alert").transform;
		barBG = bar.parent.FindChild("barBG");
		xScale = bar.localScale.x;

		AImanager = EnemyManager.Instance;
	}

	void Update () 
	{
		if(!display)
			return;

		// get alertness of nearest AI
		float alertValue = AImanager.GetClosestAlertness();
		float alarmedValue = AImanager.alarmedValue;

		if(alertValue > alarmedValue)
			bar.GetComponent<Image>().color = new Color(1, 0, 0, 0.8f);
		else
			bar.GetComponent<Image>().color = new Color(0, 0.75f, 1, 0.8f);

		alertValue *= 0.01f;
		alertValue = Mathf.Clamp(alertValue, 0.0f, 1.0f);

		bar.localScale = new Vector3(xScale*alertValue, bar.localScale.y, bar.localScale.z);

		bool showBG = false;
		if(alertValue > 0)
			showBG = true;
		barBG.gameObject.SetActive(showBG);
	}
}
