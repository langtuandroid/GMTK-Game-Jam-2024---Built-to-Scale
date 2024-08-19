using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.AI;

public class SceepleScript : MonoBehaviour {

	public SceepleSpawnerScript.Sceeple stats;
	public SplineFollower splineFollower;
	public NavMeshAgent navAgent;
	public float targetSpeed;
	private bool wander;
	private float angle;

	void Start() {

		//Set the wander state to false
		SetWander(false);

		//Update the angle calc
		UpdateAngle();

		//Set the initial follow speed
		targetSpeed = 2;

		//Debug.Log(stats.skillLevel);

	}

	private void Update() {

        //Has this sceeple passed the ticket booth?
		if (stats.hasPassedTickedBooth) {

			//If not wandering, calculate the spline follower speeds
			if (!wander) {


				//Have they passed the ticket booth? 
				if (stats.hasPassedTickedBooth) {

					//Update the angle calc
					UpdateAngle();

					//Get the programatic target speed
					targetSpeed = stats.skillLevel * angle;
				}

				//Haven't passed the ticket booth yet, so set fixed speed
				else {
					targetSpeed = 2;
				}

				//Debug.Log(stats.skillLevel * angle);

				//Calculate the speed change variable
				var speedChange = Time.deltaTime * 5;

				//If the target speed is greater than the speed
				if (targetSpeed > splineFollower.followSpeed) {

					//Set the spline speed
					splineFollower.followSpeed = Mathf.Clamp(splineFollower.followSpeed + speedChange, splineFollower.followSpeed, targetSpeed);
				}

				//If the target speed is less than
				else if (targetSpeed < splineFollower.followSpeed) {

					//Set the spline speed
					splineFollower.followSpeed = Mathf.Clamp(splineFollower.followSpeed - speedChange, targetSpeed, splineFollower.followSpeed);
				}
				//Debug.Log(splineFollower.followSpeed);
			}

			//If wander is true
			else {
				//TODO: write the wander code
			}
		}
		
		//Hasn't passed the ticket booth yet, 

	}

	public void SetWander(bool state) {

		//Set the wander variable
		wander = state;

		//Set the sceeple spline follow to the inverse 
		splineFollower.follow = !state;

		if (state) {
			splineFollower.followSpeed = 0;
		}
	}


	private void SetWanderPoint() {

	}

	private void UpdateAngle() {
		//Set the current elevatiojn angle/grade
		var tempAngle = Mathf.DeltaAngle(0, transform.eulerAngles.x);


		if (tempAngle < 0) {
			angle = 1 - (Mathf.Abs(tempAngle) / 90);
		}
		else {
			angle = 1 + (Mathf.Abs(tempAngle) / 90);
		}
	}

}
