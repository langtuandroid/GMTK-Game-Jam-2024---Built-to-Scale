using System.Collections.Generic;
using RandomNameGen;
using UnityEngine;
using sysRand = System.Random;

public class SceepleSpawnerScript : MonoBehaviour {

	public List<SceepleScript> sceeples;
	public GameObject sceeplePrefab;

	private sysRand rand;
	private RandomName nameGen;

	public struct Sceeple {
		public string name;
		public float money;
		public float skillLevel;
		public float disposition;
		public float distanceClimbed;
		public bool reachedSummit;
	}

	private GameController gc;


	void Start() {

		//Shorthand the game controller
		gc = FindFirstObjectByType<GameController>();

		//Create a new randomness class, and set a seed
		rand = new sysRand((int)Time.time);

		//Create a new randome name generator
		nameGen = new RandomName(rand);
	}

	// Update is called once per frame
	void Update() {

	}

	public void TrySpawnSceeple() {
		Debug.Log(11111);
		if (sceeples.Count < gc.maxSceeples) {
		Debug.Log(22222);
			SpawnSceeple();
		}
	}

	private void SpawnSceeple() {

		//Spawn a sceeple, and add it to the sceeple holder
		var sceeple = Instantiate(sceeplePrefab, transform).GetComponent<SceepleScript>();

		//Move it to the start point inside the sceeple holder
		sceeple.transform.localPosition = Vector3.zero;

		//Set the unique sceeple stats
		sceeple.stats = new Sceeple {

			//Create a sceeple name
			name = nameGen.RandomNames(1, 1)[0],

			//Get a random amount of money between the ticket price and the max money value
			money = Random.Range(gc.ticketPrice, gc.maxSceepleMoney),

			//Get a random skill level
			skillLevel = Random.Range(gc.minSceepleSkillLevel, gc.maxSceepleSkillLevel),

			//Get a random disposition: lower == harder to please, higher == easier to please (aka more likely to leave a good review, and or buy merch)
			disposition = Random.Range(gc.minSceepleDisposition, gc.maxSceepleDisposition),

			//How far did they get
			distanceClimbed = 0,

			//Did they reach the summit of the mountain
			reachedSummit = false,
		};

		//Assign the path up to the sceeple spline follower
		sceeple.splineFollower.spline = gc.pathUp;

		//Add the sceeple to the list
		sceeples.Add(sceeple);
	}

	public void SceeplePassedTicketBooth(SceepleScript sceeple) {
		
	}
}
