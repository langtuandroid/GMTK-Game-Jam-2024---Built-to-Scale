using Cinemachine;
using UnityEngine;
using Dreamteck.Splines;
using UnityEngine.InputSystem.Processors;
public class GameController : MonoBehaviour {
	public string state;
	private string lastState;



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




					/*
					//Get the current position on the splice as a percentage
					var playerPercent = playerFollower.result.percent;

					//Evaluate the next direction as a normalised Vector3
					var playerDirection = (playerFollower.spline.EvaluatePosition(playerPercent + 0.01f) - playerFollower.spline.EvaluatePosition(playerPercent)).normalized;

					//shorthand the virtual camera's forwards vector
					var virtualCameraForwards = virtualCamera.transform.forward;

					//Clear the virtual cameras forwards y component
					virtualCameraForwards.y = 0;

					//Clear the virtual cameras forwards z component
					virtualCameraForwards.z = 0;

					//Get the dot product of the virtual camera forwards and the normalised player direction
					var playerDirectionDot = Vector3.Dot(virtualCameraForwards, playerDirection);

					//Set the player movement direction based on the camera forwards direction, based on the dot product evaluating to: x > 0 = forwards, x =< 0 == backwards
					playerFollower.direction = Vector3.Dot(virtualCameraForwards, playerDirection) > 0 ? Spline.Direction.Forward : Spline.Direction.Backward;
					*/



					/*
					// Get the current position on the spline as a percentage
					var playerPercent = playerFollower.result.percent;

					// Evaluate the next direction as a normalized Vector3
					var playerDirection = (playerFollower.spline.EvaluatePosition(playerPercent + 0.01f) - playerFollower.spline.EvaluatePosition(playerPercent)).normalized;

					// Get the virtual camera's forward vector
					var virtualCameraForwards = virtualCamera.transform.forward;

					// Clear the Y component of the virtual camera's forward vector
					virtualCameraForwards.y = 0;
					virtualCameraForwards.Normalize(); // Normalize after clearing Y

					// Get the angle between the virtual camera's forward vector and the player's direction
					var angle = Quaternion.Angle(Quaternion.LookRotation(virtualCameraForwards), Quaternion.LookRotation(playerDirection));

					// Set the player movement direction based on the angle
					playerFollower.direction = angle <= 90 ? Spline.Direction.Forward : Spline.Direction.Backward;

					// If the input movement is less than 0, invert the movement direction
					if (movement < 0) {
						// Invert direction
						playerFollower.direction = playerFollower.direction == Spline.Direction.Forward
							? Spline.Direction.Backward
							: Spline.Direction.Forward;
					}

					// Calculate the movement speed based on how aligned the camera is with the spline direction
					float alignmentFactor = 1 - (angle / 180f); // Closer to 0 means less aligned, closer to 1 means more aligned

					// Move the player along the spline in the direction determined above
					playerFollower.Move(Mathf.Abs(movement) * alignmentFactor * (Input.GetButton("Boost") ? 0.5f : 0.1f));
					*/


					/*
					//If the input movement is less than 0, invert the movement direction
					if (movement < 0) {

						//Was it forwards?
						if (playerFollower.direction == Spline.Direction.Forward) {

							//Make it backwards
							playerFollower.direction = Spline.Direction.Backward;
						}

						//Was it forwards
						else if (playerFollower.direction == Spline.Direction.Backward) {

							//Make it backwards?
							playerFollower.direction = Spline.Direction.Forward;
						}
					}


					//Move the player along the spline in the direction determined above
					playerFollower.Move(Mathf.Abs((movement) * (Input.GetButton("Boost") ? 0.5f : 0.1f)) * Mathf.Abs(playerDirectionDot));
					*/



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
