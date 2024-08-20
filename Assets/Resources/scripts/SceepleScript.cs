using System.Threading;
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
	public bool turnedBack;
	public float distanceClimbed;
	public float rating;
	public bool reachedSummit;
	public bool showHelmet;
	public bool showShirt;
	public bool showHat;
	public bool showPlushie;
	public bool hasPassedTickedBooth;
	public Spline.Direction splineDirection = Spline.Direction.Forward;
	public float angle;
	private Vector3 lastPos;
	

	[Header("Models")]
	public GameObject helmet;
	public GameObject hat;
	public GameObject shirt;
	public GameObject plushie;

	private GameController gc;


	async void Start() {


		//Shorthand the game controller
		gc = FindFirstObjectByType<GameController>();

		//Set the Helmet's visibility (always off if the hat is visible)
		helmet.SetActive(!showHat && showHelmet);

		//Set the Hat's visibility
		hat.SetActive(showHat);

		//Set the Shirt's visibility
		shirt.SetActive(showShirt);

		//Set the helment colour based on the sceeples skill
		helmet.GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", gc.sceepleSpawner.helmetColours.Evaluate((stats.skillLevel - gc.minSceepleSkillLevel) / (gc.maxSceepleSkillLevel - gc.minSceepleSkillLevel)));

		//Wait a tick to let things settle
		await UniTask.Delay(16);

		//toggle the nav agent on
		navAgent.enabled = true;

		//Set the path as the first point on the spline
		navAgent.SetDestination(splineFollower.spline.GetPoint(0).position);

		//Wait for the sceeple to reach the start of the spline
		await AwaitDestinationReached();


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

	private void Update() {

		//Sync the helmet state 
		helmet.SetActive(!showHat && showHelmet);

		//Sync the hat state 
		hat.SetActive(showHat);

		//Sync the shirt state 
		shirt.SetActive(showShirt);

		//Sync the shirt state 
		plushie.SetActive(showPlushie);

		//Try to force the spline direction
		splineFollower.direction = splineDirection;

		//Update the angle calc
		UpdateAngle();

		//Get the programatic target speed
		targetSpeed = stats.skillLevel * angle;

		if (splineDirection == Spline.Direction.Backward) {
			targetSpeed = -6;
		}

		//Calculate the speed change variable
		var speedChange = Time.deltaTime * 5;

		if (gc.debug) {
			targetSpeed = targetSpeed * 20;
			speedChange = speedChange * 5;
		}

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


		//Make them face the travel direction
		FaceTravelDirection();

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

	public async UniTask SetWander(bool state) {

		//Set the wander variable
		wander = state;

		//Set the sceeple spline follow to the inverse 
		splineFollower.follow = !state;

		//Set the spline follower enable to the inverse
		splineFollower.enabled = !state;

		//Turn on the navagent
		navAgent.enabled = state;

	}

	private void SetWanderPoint() {
		navAgent.SetDestination(GetRandomPoint(transform.position, 10));
	}

	public async UniTask AwaitDestinationReached() {

		// Wait until the agent has a path, destination, and is not pending or null.
		while ((!navAgent.hasPath || navAgent.pathPending || navAgent.pathStatus != NavMeshPathStatus.PathComplete) && navAgent.remainingDistance < navAgent.stoppingDistance) {
			await UniTask.Yield(PlayerLoopTiming.Update);
		}

		// Wait until the agent reaches the destination or is close enough.
		while (navAgent.remainingDistance > navAgent.stoppingDistance) {
			await UniTask.Yield(PlayerLoopTiming.Update);
		}
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

		Debug.Log($"{stats.name} turned back");

		//Set the turned back flag
		turnedBack = true;

		//Reverse the spline direction
		splineDirection = Spline.Direction.Backward;
	}

}
