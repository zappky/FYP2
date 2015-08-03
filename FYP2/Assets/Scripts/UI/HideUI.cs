using UnityEngine;
using System.Collections;

public class HideUI : MonoBehaviour {

	bool showUI = true;

	Transform minimap;
	alertbar alertBarUI;
	CastSlot castSlotUI;
	CTimer timerUI;
	DialogInterface dialogUI;
	QuestManager obectivesUI;

	void Start () {
		Transform player = GameObject.FindGameObjectWithTag("Player").transform;

		minimap = player.FindChild("MiniMapCam");
		alertBarUI = player.GetComponent<alertbar>();
		castSlotUI = player.GetComponent<CastSlot>();
		timerUI = GetComponent<CTimer>();
		dialogUI = DialogInterface.Instance;
		obectivesUI = QuestManager.Instance;
	}
	
	void Update () {
		if(Input.GetKeyDown("end"))
		{
			showUI = !showUI;
			HideAllUI();
		}
	}

	void HideAllUI()
	{
		minimap.gameObject.SetActive(showUI);
		alertBarUI.display = showUI;
		castSlotUI.display = showUI;
		timerUI.display = showUI;
		dialogUI.display = showUI;
		obectivesUI.display = showUI;
	}
}
