using UnityEngine;
using Dreamteck.Splines;
public class GameController : MonoBehaviour {
	public string state;
	private string lastState;

	[Header("Player stuff")]
	public SplineFollower playerFollower;

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

				//TODO: actually init stuff
				SetState("build");
				break;

			case "build":

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
					Debug.Log(movement);
					//playerFollower.direction = movement > 0 ? Spline.Direction.Forward : Spline.Direction.Backward;
					playerFollower.Move(Mathf.Abs(movement * 0.1f));
				}

				break;
		}
	}


	//Sets the new state
	public void SetState(string _state) {
		Debug.Log(_state);
		state = _state;
	}
}
