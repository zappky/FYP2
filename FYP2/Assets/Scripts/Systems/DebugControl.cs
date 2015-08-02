using UnityEngine;
using System.Collections;

public class DebugControl : MonoBehaviour {

	public bool display = false;
	public bool initedBefore = false;
	public bool debugMode = false;
	public string inputCommand = "";
	public Rect inputField;
	public bool inputStopper = false;

	public float keyDelay = 0.1f; 
	private float timePassed = 0f;

	public static DebugControl instance = null;

	public FallResponse fallScript = null;
	public FpsMovement movementScript = null;
	public LevelManager levelmanagerScript = null;
	public Inventory inventoryScript = null;
	public HideCursorScript cursorScript = null;

	public Rect notificationDisplayRect;
	public string notificationText = "";
	public float notificationTimer  = 0.0f;
	public float notificaitonTimerLimit = 5.0f;//5 sec
	public GUIStyle notificationlabelStyle;

	public bool godMode = false;
	public bool flyMode = false;

	GameObject playerobj;

	public static DebugControl Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("Debug Control").AddComponent<DebugControl>();
				DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}
	public void Initialize()
	{
		Initialize(true);// allow reinitalize of this class by default
	}
	
	public void Initialize(bool re_init)
	{
		if(initedBefore == false || re_init == true)
		{
			inputField = new Rect(0.0f,Screen.height*0.5f,Screen.width*0.1f,20.0f);

			notificationDisplayRect = new Rect(0.0f,inputField.y+inputField.height,inputField.width,inputField.height);
			notificationlabelStyle = new GUIStyle();
			notificationlabelStyle.normal.textColor = Color.red;
			notificationlabelStyle.normal.background = Texture2D.blackTexture;
			//notificationlabelStyle.alignment = TextAnchor.;

			fallScript = GameObject.FindObjectOfType<FallResponse>();
			if(fallScript == null)
			{
				Debug.Log("ERROR: DebugControl has null fallScript");
			}

			movementScript = GameObject.FindObjectOfType<FpsMovement>();
			if(movementScript == null)
			{
				Debug.Log("ERROR: DebugControl has null movementScript");
			}

			levelmanagerScript = GameObject.FindObjectOfType<LevelManager>();
			if(levelmanagerScript == null)
			{
				Debug.Log("ERROR: DebugControl has null levelmanagerScript");
			}
			inventoryScript = GameObject.FindObjectOfType<Inventory>();
			if(inventoryScript == null)
			{
				Debug.Log("ERROR: DebugControl has null inventoryScript");
			}
			cursorScript = GameObject.FindObjectOfType<HideCursorScript>();
			if(cursorScript == null)
			{
				Debug.Log("ERROR: DebugControl has null cursorScript");
			}

			playerobj = GameObject.FindGameObjectWithTag("Player");
			if(playerobj == null)
			{
				Debug.Log("ERROR: DebugControl has null playerobj");
			}

			initedBefore = true;
		}
	}

	// Update is called once per frame
	void Update () {
		timePassed += Time.deltaTime;

		if(Input.GetKeyDown(KeyCode.Home) && timePassed >= keyDelay)
		{
			display = !display;
			cursorScript.manualOverride = display;
			Cursor.visible = display;

			timePassed = 0.0f;
		}
	}

	void OnGUI()
	{
		if(display == true)
		{
			if(notificationText != "")
			{
				GUI.Box(notificationDisplayRect,notificationText,notificationlabelStyle);
			}

			if(inputStopper == false)
			{
				inputCommand = GUI.TextField (inputField, inputCommand); 

				if(inputCommand != "")
				{
					Vector2 predictionSize = GUI.skin.label.CalcSize(new GUIContent(inputCommand));
					if(predictionSize.x > inputField.size.x)
					{
						inputField.size = GUI.skin.label.CalcSize(new GUIContent(inputCommand));
					}
					
				}
			}

			if(Input.GetKeyDown(KeyCode.Return) && timePassed >= keyDelay)
			{
				timePassed = 0.0f;

				string[] splittedstring = inputCommand.Split(' ');

				if(splittedstring.Length == 1)//due to our own decided formatting
				{
					ActivateDebugEffect(splittedstring[0],-1);
				}else if(splittedstring.Length >= 2)
				{
					int n = -1;
					bool isNumeric = int.TryParse(splittedstring[1], out n);

					if(isNumeric == true)
					{
						ActivateDebugEffect(splittedstring[0],n);
					}else
					{
						string contacatedstring = splittedstring[0];
						for(int i = 1 ; i < splittedstring.Length;++i)
						{
							if(splittedstring[i].Length >= 2)
							{
								contacatedstring += splittedstring[i];
							}else if(splittedstring[i].Length == 1)
							{
								n = -1;
								isNumeric = int.TryParse(splittedstring[i], out n);
								if(isNumeric == false)
								{
									n =-1;
								}
								break;
							}
						}
						ActivateDebugEffect(contacatedstring,n);
					}
				}

			}
		}
	}

	public bool ActivateDebugEffect(string command,int appendCode)
	{
		if(debugMode == false)
		{
			switch(command)
			{
				case "debug":
				case "Debug":
				case "DEBUG":
					switch(appendCode)
					{
					case -1:
						Debug.Log("Toggling debug Mode");
						debugMode = ! debugMode;
						if(debugMode == true)
						{
							notificationlabelStyle.normal.textColor = Color.green;
							notificationText = "DEBUG MODE GRANTED";
						}
						return debugMode;
						
					case 0:
						//by right is to deactive debug mode,redudant in here
						break;

					case 1:
						Debug.Log("Activating debug Mode");
						debugMode = true;
						notificationlabelStyle.normal.textColor = Color.green;
						notificationText = "DEBUG MODE GRANTED";
						return true;
					default:
						Debug.Log("ERROR: Unhandled appendCode code: " + appendCode + " of " + command);
						break;
					}
				break;

				default:
					Debug.Log("ERROR: Unhandled Command code: " + command);
				break;
			}
			notificationlabelStyle.normal.textColor = Color.red;
			notificationText = "COMMAND DENIED";
			return false;

		}

		//notificationText = "";
		notificationlabelStyle.normal.textColor = Color.green;

		switch(command)
		{
			case "Add Item":
		 	case "Add item":
			case "Additem":
			case "additem":
				if(inventoryScript.AddItem(appendCode) == true)
				{
					notificationText = "ADDED ITEM ID:" + appendCode;
					return true;
				}else
				{
					notificationlabelStyle.normal.textColor = Color.red;
					notificationText = "DENIED ADDED ITEM ID:" + appendCode;
					return false;
				}
		
			case "Check point":
			case "check point":
			case "Checkpoint":
			case "checkpoint":
				if(appendCode >=0 && appendCode < levelmanagerScript.checkPointList.Count )
				{
					notificationText = "TELEPORTED TO CHECKPOINT INDEX:" + appendCode;
					playerobj.transform.position = levelmanagerScript.checkPointList[appendCode].transform.position;
				}else
			{
				notificationlabelStyle.normal.textColor = Color.red;
				notificationText = "DENIED TELEPORT TO CHECKPOINT INDEX:" + appendCode;
			}
			return true;

			case "weight limit":
			case "weightlimit":
			case "Weightlimit":
				if(appendCode >= 0)
				{
					notificationText = "CHANGED INVENTORY WEIGHTLIMIT TO:" + appendCode;
					inventoryScript.weightlimit = appendCode;
				}else{
				notificationText = "DENIED CHANGED INVENTORY WEIGHTLIMIT TO:" + appendCode;
					inventoryScript.weightlimit = appendCode;
				}

				return true;

			case "Weight":
			case "weight":
			if(appendCode >= 0)
			{
				notificationText = "CHANGED INVENTORY WEIGHT TO:" + appendCode;
				inventoryScript.currentweight = appendCode;
			}else{
				notificationText = "DENIED CHANGED INVENTORY WEIGHT TO:" + appendCode;
				inventoryScript.currentweight = appendCode;
			}

			return true;

			case "debug":
			case "Debug":
			case "DEBUG":
				switch(appendCode)
				{
				case -1:
				Debug.Log("Toggling debug Mode");
				debugMode = ! debugMode;
				if(debugMode == true)
				{
					notificationlabelStyle.normal.textColor = Color.green;
					notificationText = "DEBUG MODE GRANTED";
				}else
				{
					notificationlabelStyle.normal.textColor = Color.red;
					notificationText = "DEBUG MODE REVOKED";
				}
					return true;
					
				case 0:
				Debug.Log("DeActivating debug Mode");
				debugMode = false;
				notificationlabelStyle.normal.textColor = Color.red;
				notificationText = "DEBUG MODE REVOKED";
					return true;
				case 1:
				Debug.Log("Activating debug Mode");
				debugMode = true;
				notificationlabelStyle.normal.textColor = Color.green;
				notificationText = "DEBUG MODE GRANTED";
					return true;
				default:
					Debug.Log("ERROR: Unhandled debug appendCode: " + appendCode +" of" + command);
				notificationlabelStyle.normal.textColor = Color.red;
				notificationText = "ERROR: Unhandled debug appendCode: " + appendCode.ToString() +" of" + command.ToString();
					break;
				}
				break;

			case "god mode":
			case "godmode":
				switch(appendCode)
				{
				case -1:
				Debug.Log("toggling god mode");
				godMode = !godMode;
				//fallScript.active = !fallScript.active;
				if(godMode == true)
				{
					notificationlabelStyle.normal.textColor = Color.green;
					notificationText = "GOD MODE ENABLED";
				}else
				{
					notificationlabelStyle.normal.textColor = Color.red;
					notificationText = "GOD MODE DISABLED";
				}
				return true;

				case 0:
					Debug.Log("DeActivating god mode");
				notificationlabelStyle.normal.textColor = Color.red;
				notificationText = "GOD MODE DISABLED";
				//fallScript.active = true;
				godMode = false;
				//godMode = fallScript.active;
					return true;
				case 1:
					Debug.Log("Activating god mode");
				notificationlabelStyle.normal.textColor = Color.green;
				notificationText = "GOD MODE ENABLED";
				//fallScript.active = false;
				godMode = true;
				//godMode = fallScript.active;
					return true;
				default:
					Debug.Log("ERROR: Unhandled debug appendCode: " + appendCode +" of" + command);
				notificationlabelStyle.normal.textColor = Color.red;
				notificationText = "ERROR: Unhandled debug appendCode: " + appendCode.ToString() +" of" + command.ToString();
					break;
				}
			break;

			case "fly mode":
			case "flymode":
				switch(appendCode)
				{
				case -1:
				Debug.Log("toggling fly mode");
				//movementScript.debugFlyMode = !movementScript.debugFlyMode;
				flyMode = !flyMode;
				if(flyMode == true)
				{
					notificationlabelStyle.normal.textColor = Color.green;
					notificationText = "FLY MODE ENABLED";
				}else
				{
					notificationlabelStyle.normal.textColor = Color.red;
					notificationText = "FLY MODE DISABLED";
				}
				//flyMode = movementScript.debugFlyMode;
				return true;

				case 0:
					Debug.Log("DeActivating fly mode");
				//movementScript.debugFlyMode = false;
				notificationlabelStyle.normal.textColor = Color.red;
				notificationText = "FLY MODE DISABLED";
				flyMode = false;
				//flyMode = movementScript.debugFlyMode;
				return true;

				case 1:
					Debug.Log("Activating fly mode");
				//movementScript.debugFlyMode = true;
				notificationlabelStyle.normal.textColor = Color.green;
				notificationText = "FLY MODE ENABLED";
				//flyMode = movementScript.debugFlyMode;
				flyMode = true;
				return true;

				default:
					Debug.Log("ERROR: Unhandled debug appendCode: " + appendCode +" of" + command);
					notificationlabelStyle.normal.textColor = Color.red;
					notificationText = "ERROR: Unhandled debug appendCode: " + appendCode.ToString() +" of" + command.ToString();
					break;
				}
			break;

			default:
				Debug.Log("ERROR: Unhandled debug command: " + command);
			notificationlabelStyle.normal.textColor = Color.red;
			notificationText = "ERROR: Unhandled debug command: " + command.ToString();
			break;
		}
		return false;
	}
}
