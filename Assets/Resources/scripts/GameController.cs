using Cinemachine;
using UnityEngine;
using Dreamteck.Splines;
public class GameController : MonoBehaviour {
	[Header("State stuff")]
	public string state;
	private string lastState;



	[Header("Sceeple stuff")]
	public int maxSceeples = 5;
	public SceepleSpawnerScript sceepleSpawner;



	[Header("UI stuff")]
	public GameObject uiBuildModeIndicator;
	public GameObject uiPlayModeIndicator;
	public GameObject uiControls;




	[Header("Player stuff")]
	public Transform player;
	public SplineFollower playerFollower;
	public SplinePositioner playerPositioner;
	public CinemachineVirtualCamera virtualCamera;
	public float playerSpeed = 1;




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

				var movement = Input.GetAxis("Vertical");

				if (movement != 0) {



				}

				break;

			case "play":
				break;

		}
	}


	//Sets the new state
	public void SetState(string _state) {
		Debug.Log(_state);
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
}
