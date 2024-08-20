using TMPro;
using UnityEngine;

public class HighScoresScript : MonoBehaviour {

	public TextMeshProUGUI score1;
	public TextMeshProUGUI score2;
	public TextMeshProUGUI score3;
	public TextMeshProUGUI score4;
	public TextMeshProUGUI score5;
	
	void Start() {
		
	}

	private void OnEnable() {
		//Old: Days: 8888 - Income: $8888888 - Rating: 5/5
		//New: $Income
		var score1Int = PlayerPrefs.GetInt("Score - 1");
		var score2Int = PlayerPrefs.GetInt("Score - 2");
		var score3Int = PlayerPrefs.GetInt("Score - 3");
		var score4Int = PlayerPrefs.GetInt("Score - 4");
		var score5Int = PlayerPrefs.GetInt("Score - 5");
		
		score1.text = (score1Int == 0 ? "    -" : "$" + score1Int.ToString()); 
		score2.text = (score2Int == 0 ? "    -" : "$" + score2Int.ToString()); 
		score3.text = (score3Int == 0 ? "    -" : "$" + score3Int.ToString()); 
		score4.text = (score4Int == 0 ? "    -" : "$" + score4Int.ToString()); 
		score5.text = (score5Int == 0 ? "    -" : "$" + score5Int.ToString()); 
		
		
		 
 
 
 
 
	}

	// Update is called once per frame
	void Update() {

	}
}
