using System.Collections.Generic;
using RandomNameGen;
using UnityEngine;
using Random = System.Random;

public class SceepleSpawnerScript : MonoBehaviour {

	public List<SceepleScript> sceeples;
	public GameObject sceeplePrefab;

	private Random rand;
	private RandomName nameGen;

	public struct Sceeple {
		public string name;
		public float money;
		public float skillLevel;
		public float disposition;
	}

	private GameController gc;


	void Start() {
		gc = FindFirstObjectByType<GameController>();


		//Create a new randomness class, and set a seed
		rand = new Random((int)Time.time);

		//Create a new randome name generator
		nameGen = new RandomName(rand);
		//Create a sceeple name
		Debug.Log(nameGen.RandomNames(1, 1)[0]);
	}

	// Update is called once per frame
	void Update() {

	}

	public void TrySpawnSceeple() {
		if (gc.maxSceeples < sceeples.Count) {
			SpawnSceeple();
		}
	}

	private void SpawnSceeple() {

		//Spawn a sceeple, and add it to the sceeple holder
		var sceeple = Instantiate(sceeplePrefab, transform).GetComponent<SceepleScript>();

		//Move it to the start point inside the sceeple holder
		sceeple.transform.localPosition = Vector3.zero;

		//Create a sceeple name
		string Names = nameGen.RandomNames(1, 1)[0];

		//Set the unique sceeple stats
		sceeple.stats = new Sceeple {
			name = "asd",
			money = 0,
			skillLevel = 0,
			disposition = 0,
		};

		sceeples.Add(sceeple);
	}
}
