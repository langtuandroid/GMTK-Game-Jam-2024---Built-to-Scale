using UnityEngine;

public class GameController : MonoBehaviour {
	public string state;
	private string lastState;



	// Start is called before the first frame update
	private void Start() {
	}

	// Update is called once per frame
	private void Update() {
		if (state != lastState) {
			state = lastState;
			StateChanged();
		}

		StateEachFrame();

	}

	//These actions are run only when the state changes
	private void StateChanged() {
		switch (state) {
			case "initialise":
				break;
		}
	}


	//These actions are run every frame for the current state
	private void StateEachFrame() {
		switch (state) {
			case "initialise":
				break;
		}
	}
}
