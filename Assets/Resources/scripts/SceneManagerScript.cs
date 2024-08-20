using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour {
	public float fadeTime = 1;
	public GameObject uiInGameMenu;
	public GameObject uiMainMenu;
	public GameObject uiHowToPlay;
	public GameObject uiOptions;
	public GameObject uiHighScores;
	public GameObject uiEndOfDayReport;
	public GameObject uiPurchaseMenu;
	public GameObject uiUpgradeMenu;
	public GameObject uiEndOfDayReportFinish;
	public GameObject uiEndOfDayReportNext;
	public TextMeshProUGUI uiEndOfDayReportVisitors;
	public TextMeshProUGUI uiEndOfDayReportIncome;
	public TextMeshProUGUI uiEndOfDayReportRating;
	[HideInInspector] public string savedGameControllerState;

	public float cameraSensitivity;
	public float cameraSpeed;

	public SliderScript cameraSpeedSlider;
	public SliderScript cameraSensitivitySlider;
	private static GameObject instance;



	private GameController gc;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);

		if (instance == null) {
			instance = gameObject;
			fader.color = Color.black;
		}
		else {
			Debug.Log(123123123123123123);
			DestroyImmediate(gameObject);
		}

	}

	public UnityEngine.UI.Image fader;

	private async void Start() {

		cameraSpeed = PlayerPrefs.GetFloat("cameraSpeed", 50f);
		cameraSensitivity = PlayerPrefs.GetFloat("cameraSensitivity", 0.75f);


		cameraSpeedSlider.slider.value = cameraSpeed;
		cameraSensitivitySlider.slider.value = cameraSensitivity;


		gc = getGameController();

		//Clean up
		ResetUI();

		//Show the main menu
		ShowMainMenu();

		//Fade the scene back in
		await FadeScreenIn();
	}

	private void Update() {
		gc = getGameController();

	}

	private GameController getGameController() {
		return FindFirstObjectByType<GameController>();
	}

	public void ResetUI() {

		cameraSpeed = cameraSpeedSlider.slider.value;
		cameraSensitivity = cameraSensitivitySlider.slider.value;
		uiInGameMenu.SetActive(false);
		uiMainMenu.SetActive(false);
		uiHowToPlay.SetActive(false);
		uiOptions.SetActive(false);
		uiHighScores.SetActive(false);
		uiEndOfDayReport.SetActive(false);


		try {
			if (uiPurchaseMenu is not null) {
				uiPurchaseMenu.SetActive(false);
			}
		}
		catch (Exception e) {
			//print("error");
		}
		try {
			if (uiUpgradeMenu is not null && uiUpgradeMenu.activeInHierarchy) {
				uiUpgradeMenu.SetActive(false);
			}
		}
		catch (Exception e) {
			//print("error");
		}


	}

	public async UniTask FadeScreenIn() {

		fader.raycastTarget = true;

		//If the alpha is greater than 0 aka invisible
		while (fader.color.a > 0) {

			//Reduce the alpha a bit
			fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, Mathf.Clamp(fader.color.a - (Time.deltaTime / fadeTime), 0, 255));

			//Yeild to other processes
			await UniTask.Yield(PlayerLoopTiming.Update);
		}
		fader.raycastTarget = false;
	}

	public async UniTask FadeScreenOut() {

		fader.raycastTarget = true;

		//If the alpha is less than 1 aka full visible
		while (fader.color.a < 1) {

			//Increase the alpha a bit
			fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, Mathf.Clamp(fader.color.a + (Time.deltaTime / fadeTime), 0, 1));

			//Yeild to other processes
			await UniTask.Yield(PlayerLoopTiming.Update);
		}

	}
	public async void ChangeSceneVoid(string scene) {
		await ChangeScene(scene, false);
	}

	public async UniTask ChangeScene(string scene, bool skipFade) {

		//Fade the screen out
		if (!skipFade) {
			await FadeScreenOut();
		}

		//Wait a beat
		await UniTask.Delay(100);

		//Hide the ui
		ResetUI();

		//Load the new scene
		SceneManager.LoadScene(scene);

		//Wait few beats
		await UniTask.Delay(300);

		//Fade the scene back in
		if (!skipFade) {
			await FadeScreenIn();
		}
	}

	public async void SaveGame() {

		//TODO: write the game saver
	}

	public async void LoadGame() {
		//TODO: write the game loader
	}

	public async void ShowMainMenu() {

		//Hide all the menu items
		ResetUI();

		//Show the Main Menu screen
		uiMainMenu.SetActive(true);
	}

	public async void ShowInGameMenu() {

		//Hide all the menu items
		ResetUI();

		PausePlaying();

		//Show the how tom play screen
		uiInGameMenu.SetActive(true);
	}

	public async void ShowHowToPlay() {

		//Hide all the menu items
		ResetUI();

		//Show the how to play screen
		uiHowToPlay.SetActive(true);
	}

	public async void ShowHighScores() {

		//Hide all the menu items
		ResetUI();

		//Show the highscores screen
		uiHighScores.SetActive(true);
	}

	public async void ShowOptions() {
		//Hide all the menu items
		ResetUI();

		//Show the options screen
		uiOptions.SetActive(true);
	}

	public async void ShowPurchaseMenu() {
		//Hide all the menu items
		ResetUI();

		//Show the options screen
		uiPurchaseMenu.SetActive(true);

		if (gc is not null) {

			//Save the last gameplay state
			savedGameControllerState = gc.state;

			//Show the how tom play screen
			gc.SetState("paused");
		}
	}

	public void ShowUpgradeMenu() {
		//Hide all the menu items
		ResetUI();

		//Show the options screen
		uiUpgradeMenu.SetActive(true);

		if (gc is not null) {

			//Save the last gameplay state
			savedGameControllerState = gc.state;

			//Show the how tom play screen
			gc.SetState("paused");
		}
	}

	public void PausePlaying() {

		//Hide all the menu items
		ResetUI();

		//Show the in game menu screen
		uiInGameMenu.SetActive(true);

		if (gc is not null) {

			//Save the last gameplay state
			savedGameControllerState = gc.state;

			//Show the how tom play screen
			gc.SetState("paused");
		}

	}

	public void ResumePlaying() {

		//Save any options data here
		SaveData();

		//Hide all the menu items
		ResetUI();

		gc = getGameController();

		//Was a game controller found? must be playing
		if (gc is not null) {

			//Restore the last gameplay state
			gc.SetState(savedGameControllerState);

			//Clear the last gameplay state
			savedGameControllerState = null;
		}

		//No gc? Must still be on the main menu
		else {
			ShowMainMenu();
		}

	}

	public void SaveData() {

		PlayerPrefs.SetFloat("cameraSpeed", cameraSpeed);
		PlayerPrefs.SetFloat("cameraSensitivity", cameraSensitivity);

		PlayerPrefs.Save();
	}

	public void SaveHighscore(int score) {
		var scores = new List<int> {
			PlayerPrefs.GetInt("Score - 1"),
			PlayerPrefs.GetInt("Score - 2"),
			PlayerPrefs.GetInt("Score - 3"),
			PlayerPrefs.GetInt("Score - 4"),
			PlayerPrefs.GetInt("Score - 5"),
			score,
		};


		scores.Sort();

		PlayerPrefs.SetInt("Score - 1", scores[^1]);
		PlayerPrefs.SetInt("Score - 2", scores[^2]);
		PlayerPrefs.SetInt("Score - 3", scores[^3]);
		PlayerPrefs.SetInt("Score - 4", scores[^4]);
		PlayerPrefs.SetInt("Score - 5", scores[^5]);
		PlayerPrefs.Save();

	}

	public void ShowEndOfDayReport() {

		//Hide all the menu items
		ResetUI();

		gc = getGameController();

		//Was a game controller found? must be playing
		if (gc is not null) {
			uiEndOfDayReportVisitors.text = gc.dayVisitorCount.ToString();
			uiEndOfDayReportIncome.text = gc.dayIncome.ToString();
			uiEndOfDayReportRating.text = (gc.dayRatingRaw / gc.dayVisitorCount).ToString("F1");
		}

		//Show the options screen
		uiEndOfDayReport.SetActive(true);

		uiEndOfDayReportFinish.SetActive(false);
		uiEndOfDayReportNext.SetActive(false);
		if (gc.day == gc.totalDays) {
			uiEndOfDayReportFinish.SetActive(true);
		}
		else {
			uiEndOfDayReportNext.SetActive(true);
		}
	}

	public void ShowEndOfDayReportNext() {

		//Hide all the menu items
		ResetUI();

		gc = getGameController();

		//Was a game controller found? must be playing
		if (gc is not null) {
			gc.day++;
			gc.SetState("build");
		}

	}

	public async void FinishGame() {

		savedGameControllerState = null;
		SaveHighscore(gc.totalIncome);
		await FadeScreenOut();
		transform.SetParent(gc.transform);
		ChangeScene("menu", true);
	}
}
