using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Dreamteck.Splines;
using TMPro;
public class GameController : MonoBehaviour {


	[Header("Park stuff")]
	public SplineComputer pathUp;
	public SplineComputer pathDown;
	public Transform leaveMountainPoint;
	public int ticketPrice = 12;
	public int funds;
	public List<GiftShopItemScript> unlockedGiftShopItems;
	[HideInInspector] public int todaysVisitorCount;

	
	[Header("State stuff")]
	public string state;
	private string lastState;
	public bool mouseLockState = true;


	[Header("Sceeple stuff")]
	public SceepleSpawnerScript sceepleSpawner;
	public int maxSceeples = 5;
	public float maxSceepleMoney = 115;
	public float minSceepleSkillLevel = 5;
	public float maxSceepleSkillLevel = 15;
	public float minSceepleDisposition = 1;
	public float maxSceepleDisposition = 1000;


	[Header("UI stuff")]
	//public GameObject uiBuildModeIndicator;
	//public GameObject uiPlayModeIndicator;
	//public GameObject uiControls;
	public GameObject uiPlayMode;
	public GameObject uiBuildMode;
	public TextMeshProUGUI uiFundsIndicator;
	public TextMeshProUGUI uiTodaysVisitorCount;
	public AudioSource sfxOnShot;
	public AudioClip sfxInGameMenu;


	[Header("Player stuff")]
	public Transform player;
	public CinemachineVirtualCamera virtualCamera;

	private SceneManagerScript sm;

	// Start is called before the first frame update
	private async void Start() {

		//Shorthand the game controller
		sm = FindFirstObjectByType<SceneManagerScript>();

		//Stare the game on the initialise state
		SetState("initialise");
	}

	// Update is called once per frame
	private void Update() {

		//If the state has changed
		if (state != lastState) {

			//Update the last state to the current state
			lastState = state;

			//Run the on change function for the current state
			StateChanged();
		}

		//Run this every frame for the current state
		StateEachFrame();

		//Set the mouse lock state
		SetMouseLock(mouseLockState);

		//Update all ui text to the current values, visible or not
		UpdateUITextValues();

		if (Input.GetKeyDown(KeyCode.M)) {
			if (state != "paused") {
				sm.ShowInGameMenu();
				sfxOnShot.PlayOneShot(sfxInGameMenu);
			}
			else {
				sm.ResumePlaying();
			}

		}


	}

	//These actions are run only when the state changes
	private void StateChanged() {
		
		//Reset the mouse lock state on each state change, and let the state correct it if need be
		mouseLockState = true;
		
		switch (state) {
			case "initialise":

				//Clear the ui
				ResetUI();

				//Go to build mode
				SetState("build");

				break;

			case "build":

				//Show the build ui
				ShowBuildModeUI();

				break;

			case "play":

				//Show the play ui
				ShowPlayModeUI();

				break;

			case "paused":
				mouseLockState = false;
				break;
		}
	}


	//These actions are run every frame for the current state
	private void StateEachFrame() {
		switch (state) {

			//Run once when the game is started
			case "initialise":
				break;

			case "build":
				
						
				if (Input.GetKeyDown(KeyCode.B)) {
					StartDay();
				}
						
				if (Input.GetKeyDown(KeyCode.P)) {
					sm.ShowPurchaseMenu();
				}
						
				if (Input.GetKeyDown(KeyCode.U)) {
					sm.ShowUpgradeMenu();
				}
				

				break;

			case "play":
				break;

		}
	}


	//Sets the new state
	public void SetState(string _state) {
		state = _state;
	}

	private async void StartDay() {
		SetState("play");

		await sceepleSpawner.SpawnAllSceeple();
	}

	//Sets the new state
	public void ResetUI() {

		//Hide the build mode indicator
		//uiBuildModeIndicator.SetActive(false);

		//Hide the play mode indicator
		//uiPlayModeIndicator.SetActive(false);

		//Hide the controls indicator
		//uiControls.SetActive(false);
	}


	//Sets the new state
	public void ShowBuildModeUI() {

		//Reset the ui
		ResetUI();

		//Show build mode stuff
		uiBuildMode.SetActive(true);
	}


	//Sets the new state
	public void ShowPlayModeUI() {

		//Reset the ui
		ResetUI();

		//Show build mode stuff
		uiPlayMode.SetActive(true);
	}


	//Sets the new state
	public void SetMouseLock(bool lockState) {
		Cursor.lockState = lockState ? CursorLockMode.Locked : CursorLockMode.Confined;
		Cursor.visible = !lockState;
	}


	//Sets the new state
	public void ToggleMouseLock() {

		mouseLockState = !mouseLockState;
		SetMouseLock(mouseLockState);
	}

	private void UpdateUITextValues() {

		//Update the funds indicator
		uiFundsIndicator.text = $"${funds}";

		//Update the funds indicator
		uiTodaysVisitorCount.text = $"Visitors: {todaysVisitorCount}";

	}

}
