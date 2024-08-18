using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TriggerScript : MonoBehaviour {

	public float speed = -1;

	// All of the functions to run when a sceeple enters the trigger
	public UnityEvent<SceepleScript, float> onEnter;

	// All of the functions to run when a sceeple leaves the trigger
	public UnityEvent<SceepleScript, float> onExit;


	//Listen for trigger enter
	private void OnTriggerEnter(Collider other) {

		//Was it a sceeple?
		if (other.CompareTag("Sceeple")) {

			//Fire the onEnter event
			var sceeple = other.GetComponent<SceepleScript>();
			onEnter.Invoke(sceeple, speed == -1 ? sceeple.splineFollower.followSpeed : speed);
		}
	}

	//Listen for trigger exit
	private void OnTriggerExit(Collider other) {

		//Was it a sceeple?
		if (other.CompareTag("Sceeple")) {

			//Fire the onExit event
			var sceeple = other.GetComponent<SceepleScript>();
			onExit.Invoke(sceeple, speed == -1 ? sceeple.splineFollower.followSpeed : speed);
		}
	}

}
