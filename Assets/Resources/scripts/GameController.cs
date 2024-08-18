using Cinemachine;
using UnityEngine;
using Dreamteck.Splines;
public class GameController : MonoBehaviour {

	[Header("State stuff")]
	public string state;
	private string lastState;
	public float ticketPrice = 1;
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
	public GameObject uiBuildModeIndicator;
	public GameObject uiPlayModeIndicator;
	public GameObject uiControls;


	[Header("Park stuff")]
	public SplineComputer pathUp;
	public SplineComputer pathDown;
	public int funds;


	[Header("Player stuff")]
	public Transform player;
	public CinemachineVirtualCamera virtualCamera;



	// Start is called before the first frame update
	private void Start() {
		SetState("initialise");
	}

	// Update is called once per frame
	private void Update() {
		if (state != lastState) {
			lastState = state;
			StateChanged();
		}

		StateEachFrame();

		//Set the mouse lock state
		SetMouseLock(mouseLockState);

	}

	//These actions are run only when the state changes
	private void StateChanged() {
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
		}
	}


	//These actions are run every frame for the current state
	private void StateEachFrame() {
		switch (state) {

			//Run once when the game is started
			case "initialise":
				break;

			case "build":
				if (Input.GetKeyDown(KeyCode.P)) {
					ToggleMouseLock();
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

	//Sets the new state
	public void ResetUI() {

		//Hide the build mode indicator
		uiBuildModeIndicator.SetActive(false);

		//Hide the play mode indicator
		uiPlayModeIndicator.SetActive(false);

		//Hide the controls indicator
		uiControls.SetActive(false);
	}

	//Sets the new state
	public void ShowBuildModeUI() {

		//Reset the ui
		ResetUI();

		//Hide the build mode indicator
		uiBuildModeIndicator.SetActive(true);

		//Hide the controls indicator
		uiControls.SetActive(true);
	}

	//Sets the new state
	public void ShowPlayModeUI() {

		//Reset the ui
		ResetUI();

		//Hide the play mode indicator
		uiPlayModeIndicator.SetActive(false);
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
}
