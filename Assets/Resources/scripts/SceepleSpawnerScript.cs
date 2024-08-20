using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using RandomNameGen;
using UnityEngine;
using sysRand = System.Random;

public class SceepleSpawnerScript : MonoBehaviour {

	public List<SceepleScript> sceeples;
	public GameObject sceeplePrefab;
	public Transform[] spawnPoints;
	public Transform[] summitLookoutPoints;
	public Gradient helmetColours;

	private sysRand rand;
	private RandomName nameGen;

	public struct Sceeple {
		public string name;
		public float money;
		public float skillLevel;
		public float disposition;
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

			//Set the default shirt colour
			shirtColour = new Color(1, 1, 1, 1),

			//Set the default hat colour
			hatColour = new Color(1, 1, 1, 1),

		};

		//How far did they get
		sceeple.distanceClimbed = 0;

		//Did they reach the summit of the mountain
		sceeple.reachedSummit = false;

		//Assign the path up to the sceeple spline follower
		sceeple.splineFollower.spline = gc.pathUp;

		//Give them an offset on the path
		sceeple.splineFollower.motion.offset = new Vector2(Random.Range(-2.6f, 2.6f), 0);

		//toggle the nav agent off
		sceeple.navAgent.enabled = false;

		//Add the sceeple to the list
		sceeples.Add(sceeple);

		//Chalk another visitor on the board
		gc.todaysVisitorCount++;

		gc.dayVisitorCount++;
	}

	//Tries to spawn a sceeple
	public bool TrySpawnSceeple() {
		if (sceeples.Count < gc.maxSceeples) {
			SpawnSceeple(spawnPoints[Random.Range(0, spawnPoints.Length)]);
			return true;
		}

		return false;
	}


	public async UniTask SpawnAllSceeple() {

		while (TrySpawnSceeple()) {
			await UniTask.Delay(Random.Range(1200, 4000));
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

		//Add that yoinked cash to the day
		gc.dayIncome += gc.ticketPrice;
		
		//Add that yoinked cash to the day
		gc.totalIncome += gc.ticketPrice;

		//Set the stats back
		sceeple.stats = stats;
	}

	public void AddHelmet(SceepleScript sceeple) {

		//Set the helmet to shown
		sceeple.showHelmet = true;
	}

	public void RemoveHelmet(SceepleScript sceeple) {

		//Set the helmet to shown
		sceeple.showHelmet = false;
	}

	public void AddHat(SceepleScript sceeple, GiftShopItemScript item) {
		sceeple.hatMesh.material.SetTexture("_BaseMap", item.texture);
		sceeple.showHat = true;
	}

	public void AddShirt(SceepleScript sceeple, GiftShopItemScript item) {
		sceeple.shirtMesh.material.SetTexture("_BaseMap", item.texture);

		//Set the shirt to be shown
		sceeple.showShirt = true;
	}

	public void AddPlushie(SceepleScript sceeple, GiftShopItemScript item) {
		sceeple.plushieMesh.material.SetTexture("_BaseMap", item.texture);

		//Set the shirt to be shown
		sceeple.showPlushie = true;
	}

	public void SceeplePassedTicketBooth(SceepleScript sceeple, float speed) {

		//Make sure they've not been through before
		if (!sceeple.hasPassedTickedBooth) {

			//Make them pay for a ticket
			PayForTicket(sceeple);

			//Put on their helmet
			AddHelmet(sceeple);

			//Set that they've passed the ticketbooth
			sceeple.hasPassedTickedBooth = true;

		}

	}

	public void DifficultyCheck(SceepleScript sceeple, float speed) {

		if (!sceeple.reachedSummit && !sceeple.turnedBack) {
			Debug.Log($"{sceeple.stats.name} reached a checkpoint");

			if (ShouldTurnBack(sceeple)) {

				sceeple.distanceClimbed = (float)sceeple.splineFollower.GetPercent() * 100;

				sceeple.rating = CalculateRating(sceeple);

				Debug.Log($"{sceeple.stats.name} new check failed @ {sceeple.distanceClimbed}% and will now turn back");
				sceeple.TurnBack();

			}
			else {
				Debug.Log($"{sceeple.stats.name} new check passed");
			}

		}

	}


	// Calculate the chance to turn back
	public bool ShouldTurnBack(SceepleScript sceeple) {

		//Normalize disposition
		var normalizedDisposition = Mathf.InverseLerp(gc.minSceepleDisposition, gc.maxSceepleDisposition, sceeple.stats.disposition);

		//Normalize skill level
		var normalizedSkillLevel = Mathf.InverseLerp(gc.minSceepleSkillLevel, gc.maxSceepleSkillLevel, sceeple.stats.skillLevel);

		// Normalize the angle
		var remappedAngle = Mathf.InverseLerp(-1f, 1f, sceeple.angle);

		// Higher angle increases the chance, higher disposition and skill level decrease it
		var turnBackChance = remappedAngle * (1f - normalizedDisposition) * (1f - normalizedSkillLevel);

		// Generate a random value and determine if the sceeple should turn back
		return Random.value < turnBackChance;
	}


	public async void ReachedSummit(SceepleScript sceeple, float speed) {


		//Make sure they've not been through before
		if (!sceeple.reachedSummit) {

			Debug.Log($"{sceeple.stats.name} made it to the summit");

			//Mark the sceeple as having reached the summit
			sceeple.reachedSummit = true;

			gc.funds += 25;
			gc.totalIncome += 25;
			gc.dayIncome += 25;
				

			sceeple.distanceClimbed = 100;

			sceeple.rating = CalculateRating(sceeple);

			//Set the sceeple to wander
			await sceeple.SetWander(true);

			sceeple.navAgent.SetDestination(summitLookoutPoints[Random.Range(0, summitLookoutPoints.Length)].position);
			await sceeple.AwaitDestinationReached();
			await UniTask.Delay(1000);

			sceeple.navAgent.SetDestination(summitLookoutPoints[Random.Range(0, summitLookoutPoints.Length)].position);
			await sceeple.AwaitDestinationReached();
			await UniTask.Delay(1000);

			sceeple.navAgent.SetDestination(summitLookoutPoints[Random.Range(0, summitLookoutPoints.Length)].position);
			await sceeple.AwaitDestinationReached();
			await UniTask.Delay(1000);

			sceeple.navAgent.SetDestination(summitLookoutPoints[Random.Range(0, summitLookoutPoints.Length)].position);
			await sceeple.AwaitDestinationReached();


			Debug.Log($"{sceeple.stats.name} is heading back down after a successful climb");

			//Walk back to the spline
			sceeple.navAgent.SetDestination(sceeple.splineFollower.spline.GetPoint(sceeple.splineFollower.spline.pointCount - 1).position);
			await sceeple.AwaitDestinationReached();
			

			//Set the sceeple to wander
			await sceeple.SetWander(false);

			//Move the sceeple to the end of the spline
			sceeple.splineFollower.SetPercent(100);

			//Set the sceeple follow spline to the path down
			sceeple.splineDirection = Spline.Direction.Backward;


		}
	}

	public async void SceepleLeavingMountain(SceepleScript sceeple, float speed) {

		//Make them move faster
		sceeple.navAgent.speed = 7;


		//If the sceeple either reached the summit, or turned back
		if (sceeple.reachedSummit || sceeple.turnedBack) {

			//Stop following the spline
			sceeple.splineFollower.follow = false;

			//Disable the spline follower
			sceeple.splineFollower.enabled = false;

			//Enable the navagent
			sceeple.navAgent.enabled = true;

			//Set the target destination as the leaving point (through the gift shoppe ;) )
			sceeple.navAgent.SetDestination(gc.leaveMountainPoint.position);

			//Wait for them to get there
			await sceeple.AwaitDestinationReached();

			var rating = CalculateRating(sceeple);
			Debug.Log($"{sceeple.stats.name} gave rating: {rating} - Disp.:{sceeple.stats.disposition} - distance.:{sceeple.distanceClimbed}");

			gc.dayRatingRaw += rating;


			//Remove them
			Destroy(sceeple.gameObject);

			//Chalk another visitor on the board
			gc.todaysVisitorCount--;

			if (gc.todaysVisitorCount <= 0) {
				gc.EndDay();
			}
		}

	}

	private void RunActionBasedOnItemType(SceepleScript sceeple, GiftShopItemScript item) {

		switch (item.type) {
			case "hat":
				AddHat(sceeple, item);
				break;

			case "shirt":
				AddShirt(sceeple, item);
				break;

			case "plushie":
				AddPlushie(sceeple, item);
				break;

		}
	}


	private void TryAndBuyRandomGiftItem(SceepleScript sceeple) {

		//If we actually have gift items unlocked
		if (gc.unlockedGiftShopItems.Count > 0) {

			//Get a random gift shop item
			var item = gc.unlockedGiftShopItems[Random.Range(0, gc.unlockedGiftShopItems.Count)];

			//Add that items price to the player's funds
			gc.funds += item.price;

			//Add that to the day's income
			gc.dayIncome += item.price;

			//Add that to the day's income
			gc.totalIncome += item.price;

			//Run any associated actions for this item
			RunActionBasedOnItemType(sceeple, item);

			Debug.Log($"{sceeple.stats.name} bought: {item.visibleName}");
		}
	}


	public void CheckGiftShopPurchase(SceepleScript sceeple, float speed) {


		//Take the helmet off
		RemoveHelmet(sceeple);

		Debug.Log($"{sceeple.stats.name} rating: {sceeple.rating}, has ${sceeple.stats.money}");

		//If they hit 50%
		if (sceeple.rating > 2.5) {

			//Try and buy a item
			TryAndBuyRandomGiftItem(sceeple);
		}

		//If they hit 100%
		if (sceeple.rating > 4) {

			//Try and buy ANOTHER item
			TryAndBuyRandomGiftItem(sceeple);
		}
	}

// Function to calculate a rating from 1 to 5 stars
	public float CalculateRating(SceepleScript sceeple) {

		// Normalize disposition within the given range
		var normalizedDisposition = Mathf.InverseLerp(gc.minSceepleDisposition, gc.maxSceepleDisposition, sceeple.stats.disposition);

		// Weigh the contribution of disposition and distance to the final rating
		var dispositionWeight = 0.7f; // 70% weight to disposition
		var distanceWeight = 0.3f; // 30% weight to distance traveled

		// Normalize the distance (percentage) traveled
		var normalizedDistance = Mathf.InverseLerp(0, 100, sceeple.distanceClimbed);

		// Combine both normalized values
		var combinedScore = (normalizedDisposition * dispositionWeight) + (normalizedDistance * distanceWeight);

		// Map combined score to star rating (1 to 5)
		var starRating = Mathf.Clamp(combinedScore * 5, 1, 5);

		return starRating;
	}


}
