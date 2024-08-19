using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RandomNameGen;
using UnityEngine;
using sysRand = System.Random;

public class SceepleSpawnerScript : MonoBehaviour {

	public List<SceepleScript> sceeples;
	public GameObject sceeplePrefab;
	public Transform[] spawnPoints;
	public Gradient helmetColours;

	private sysRand rand;
	private RandomName nameGen;

	public struct Sceeple {
		public string name;
		public float money;
		public float skillLevel;
		public float disposition;
		public float distanceClimbed;
		public bool reachedSummit;
		public bool showHelmet;
		public bool showShirt;
		public bool showHat;
		public bool hasPassedTickedBooth;
		public Color shirtColour;
		public Color hatColour;
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

	//Spawns a sceeple, ignoring any caps and checks
	private void SpawnSceeple(Transform spawnPoint) {

		//Spawn a sceeple, and add it to the sceeple holder
		var sceeple = Instantiate(sceeplePrefab, transform).GetComponent<SceepleScript>();

		//Move it to the given spawn point
		sceeple.transform.position = spawnPoint.position;

		//Create the sceeple's stats
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

			//Set the default shirt colour
			shirtColour = new Color(255, 255, 255, 255),

			//Set the default hat colour
			hatColour = new Color(255, 255, 255, 255),

		};

		//Assign the path up to the sceeple spline follower
		sceeple.splineFollower.spline = gc.pathUp;

		sceeple.splineFollower.motion.offset = new Vector2(Random.Range(-2.6f, 2.6f), 0);

		//toggle the nav agent off
		sceeple.navAgent.enabled = false;

		//Add the sceeple to the list
		sceeples.Add(sceeple);
	}

	//Tries to spawn a sceeple
	public void TrySpawnSceeple() {
		if (sceeples.Count < gc.maxSceeples) {
			SpawnSceeple(spawnPoints[Random.Range(0, spawnPoints.Length)]);
		}
	}


	public void ChangeSpeed(SceepleScript sceeple, float speed) {
		sceeple.targetSpeed = speed;
	}

	public void PayForTicket(SceepleScript sceeple) {

		//Shorthand the stats
		var stats = sceeple.stats;

		//Remove the ticket price from their wallet... YOINK!
		stats.money -= gc.ticketPrice;

		//Add that yoinked cash to your own wallet
		gc.funds += gc.ticketPrice;

		//Set the stats back
		sceeple.stats = stats;
	}

	public void AddHelmet(SceepleScript sceeple) {
		//Shorthand the stats
		var stats = sceeple.stats;

		//Remove the ticket price from their walled... YOINK!
		stats.showHelmet = true;

		//Set the stats back
		sceeple.stats = stats;
	}

	public void SceeplePassedTicketBooth(SceepleScript sceeple, float speed) {
		PayForTicket(sceeple);
		AddHelmet(sceeple);

		//Shorthand the stats
		var stats = sceeple.stats;

		//Set that they've passed the ticketbooth
		stats.hasPassedTickedBooth = true;

		//Set the stats back
		sceeple.stats = stats;

		//Chalk another visitor on the board
		gc.todaysVisitorCount++;

	}

	public void DifficultyCheck(SceepleScript sceeple, float speed) {

		 
		//If the target speed is less than 6
		if (sceeple.targetSpeed < 6) {
			
			//"Roll" a dice if it's above 70 (aka 30%) 
			if (Random.Range(1, 101) > (70 * sceeple.angle)) {
				sceeple.TurnBack();
			}
		}
		
		//If the target speed is greater than 6, but less than 8; above 8 just never fails the check
		else if (sceeple.targetSpeed < 8) {
			
		}
	}

	public async void ReachedSummit(SceepleScript sceeple, float speed) {
		sceeple.navAgent.enabled = true;

		//Shorthand the stats
		var stats = sceeple.stats;

		//Remove the ticket price from their walled... YOINK!
		stats.reachedSummit = true;

		//Set the stats back
		sceeple.stats = stats;

		//Set the sceeple to wander
		sceeple.SetWander(true);

		//Wait for a random time between 2 and 5 seconds 
		UniTask.Delay(Random.Range(2000, 5001));

		//Set the sceeple follow spline to the path down
		sceeple.splineFollower.spline = gc.pathDown;

		//Set the sceeple to wander
		sceeple.SetWander(false);

	}



}
