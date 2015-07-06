using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public Canvas quitMenu;
	public Canvas startMenu;
	public Canvas LSMenu;    //level select
	public Canvas extraMenu;
	public Canvas achieveMenu;
	public Canvas creditMenu;
	public Canvas optionMenu;
	public Button startB;
	public Button optionB;
	public Button extraB;
	public Button exitB;

	void Start () 
	{
		quitMenu = quitMenu.GetComponent<Canvas>();
		quitMenu.enabled = false;
		startMenu = startMenu.GetComponent<Canvas>();
		startMenu.enabled = false;
		LSMenu = LSMenu.GetComponent<Canvas>();
		LSMenu.enabled = false;
		extraMenu = extraMenu.GetComponent<Canvas>();
		extraMenu.enabled = false;
		achieveMenu = achieveMenu.GetComponent<Canvas>();
		achieveMenu.enabled = false;
		creditMenu = creditMenu.GetComponent<Canvas>();
		creditMenu.enabled = false;
		optionMenu = optionMenu.GetComponent<Canvas>();
		optionMenu.enabled = false;

		startB = startB.GetComponent<Button>();
		optionB = optionB.GetComponent<Button>();
		extraB = extraB.GetComponent<Button>();
		exitB = exitB.GetComponent<Button>();
	}

	void DisableButton()
	{
		startB.enabled = false;
		optionB.enabled = false;
		extraB.enabled = false;
		exitB.enabled = false;
	}

	public void QuitPress()
	{
		quitMenu.enabled = true;
		DisableButton();
	}

	public void StartPress()
	{
		startMenu.enabled = true;
		LSMenu.enabled = false;
		DisableButton();
	}

	public void LevelPress()
	{
		LSMenu.enabled = true;
		startMenu.enabled = false;
		DisableButton();
	}

	public void NotPress()
	{
		quitMenu.enabled = false;
		startMenu.enabled = false;
		extraMenu.enabled = false;
		optionMenu.enabled = false;
		startB.enabled = true;
		optionB.enabled = true;
		extraB.enabled = true;
		exitB.enabled = true;
	}

	public void ExtraPress()
	{
		extraMenu.enabled = true;
		achieveMenu.enabled = false;
		creditMenu.enabled = false;
		DisableButton();
	}

	public void AchievePress()
	{
		achieveMenu.enabled = true;
		extraMenu.enabled = false;
	}

	public void CreditPress()
	{
		creditMenu.enabled = true;
		extraMenu.enabled = false;
	}

	public void OptionPress()
	{
		optionMenu.enabled = true;
		DisableButton();
	}

	public void Level1()
	{
		Application.LoadLevel("main-scene");
	}

	/*public void Level2()
	{
		Application.LoadLevel("main-scene");
	}

	public void Level3()
	{
		Application.LoadLevel("main-scene");
	}*/

	public void QuitGame()
	{
		Application.Quit ();
	}
}
