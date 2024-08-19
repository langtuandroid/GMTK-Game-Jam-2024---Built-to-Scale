using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.AI;

public class SceepleScript : MonoBehaviour {

	public SceepleSpawnerScript.Sceeple stats;
	public SplineFollower splineFollower;
	public NavMeshAgent navAgent;
	public float targetSpeed = 2;
	private bool wander;
	private bool hasClimbed;
	private bool turnedBack;
	public float angle;
	private Vector3 lastPos;

	[Header("Models")]
	public GameObject helmet;
	public GameObject hat;
	public GameObject shirt;

	private GameController gc;


	async void Start() {


		//Shorthand the game controller
		gc = FindFirstObjectByType<GameController>();



		helmet.SetActive(stats.showHelmet);
		hat.SetActive(stats.showHat);
		shirt.SetActive(stats.showShirt);

		//Set the helment colour based on the sceeples skill
		helmet.GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", gc.sceepleSpawner.helmetColours.Evaluate((stats.skillLevel - gc.minSceepleSkillLevel) / (gc.maxSceepleSkillLevel - gc.minSceepleSkillLevel)));

		Debug.Log((stats.skillLevel - gc.minSceepleSkillLevel) / (gc.maxSceepleSkillLevel - gc.minSceepleSkillLevel));

		await UniTask.Delay(100);

		//toggle the nav agent on
		navAgent.enabled = true;
	}

	private void Update() {


		//Sync the helmet state 
		helmet.SetActive(stats.showHelmet);

		//Sync the hat state 
		hat.SetActive(stats.showHat);

		//Sync the shirt state 
		shirt.SetActive(stats.showShirt);


		//Has this sceeple passed the ticket booth?
		if (stats.hasPassedTickedBooth) {

			//If not wandering, calculate the spline follower speeds
			if (!wander) {


				//If they haven't climbed
				if (!hasClimbed) {

					//Make them face the travel direction
					FaceTravelDirection();

					//And the agent is active
					if (navAgent.enabled) {

						//No agent paths pending 
						if (!navAgent.pathPending) {

							//Agent distance to target <= stopping distance + buffer
							if (navAgent.remainingDistance <= (navAgent.stoppingDistance)) {

								//Clear the nav agent path data
								navAgent.ResetPath();

								//Disable the nav agent
								navAgent.enabled = false;

								//Make them follow the spline
								splineFollower.follow = true;

								//Enable the spline follower
								splineFollower.enabled = true;

								//Mark that they've climbed
								hasClimbed = true;
							}
						}
					}
				}

				else {

					//Update the angle calc
					UpdateAngle();

					//Get the programatic target speed
					targetSpeed = stats.skillLevel * angle;
				}

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
			}

			//If wander is true
			else {

				//Make the face the travel direction
				FaceTravelDirection();

				//TODO: write the wander code
			}
		}

		//Hasn't passed the ticket booth yet,
		else {

			//Is the nav agent enabled, and are they missing a path?
			if (navAgent.enabled && !navAgent.hasPath) {

				//Se the path as the first point on the spline
				navAgent.SetDestination(splineFollower.spline.GetPoint(0).position);
			}
		}

	}

	private static Vector3 GetRandomPoint(Vector3 center, float maxDistance) {

		// Get Random Point inside Sphere which position is center, radius is maxDistance
		var randomPos = Random.insideUnitSphere * maxDistance + center;


		// from randomPos find a nearest point on NavMesh surface in range of maxDistance
		NavMesh.SamplePosition(randomPos, out var hit, maxDistance, NavMesh.AllAreas);

		//Hand back the position
		return hit.position;
	}

	private void FaceTravelDirection() {

		var direction = (transform.position - lastPos).normalized;
		if (direction.sqrMagnitude > 0.01f) {
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), 5f * Time.deltaTime);
		}

	}

	public void SetWander(bool state) {

		//Set the wander variable
		wander = state;

		//Set the sceeple spline follow to the inverse 
		splineFollower.follow = !state;

		//Set the spline follower enable to the inverse
		splineFollower.enabled = !state;

	}


	private void SetWanderPoint() {
		navAgent.SetDestination(GetRandomPoint(transform.position, 10));
	}

	private void UpdateAngle() {

		//Set the current elevation angle/grade
		var tempAngle = Mathf.DeltaAngle(0, transform.eulerAngles.x);


		if (tempAngle < 0) {
			angle = 1 - (Mathf.Abs(tempAngle) / 90);
		}
		else {
			angle = 1 + (Mathf.Abs(tempAngle) / 90);
		}
	}

	public void TurnBack() {
		turnedBack = true;
		splineFollower.direction = Spline.Direction.Backward;
	}

}
