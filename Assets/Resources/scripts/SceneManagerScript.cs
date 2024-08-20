using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour {
	public float fadeTime = 1;
	public GameObject uiInGameMenu;
	public GameObject uiMainMenu;
	public GameObject uiHowToPlay;
	public GameObject uiOptions;
	public GameObject uiHighScores;
	public GameObject uiPurchaseMenu;
	public GameObject uiUpgradeMenu;
	public string savedGameControllerState;

	private GameController gc;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		fader.color = Color.black;
	}

	public UnityEngine.UI.Image fader;

	private async void Start() {

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
		if (gc is not null) {
			return gc;
		}
		else {
			return FindFirstObjectByType<GameController>();
		}
	}

	private async void ResetUI() {
		uiInGameMenu.SetActive(false);
		uiMainMenu.SetActive(false);
		uiHowToPlay.SetActive(false);
		uiOptions.SetActive(false);
		uiHighScores.SetActive(false);
	}

	public async UniTask FadeScreenIn() {

		//If the alpha is greater than 0 aka invisible
		while (fader.color.a > 0) {

			//Reduce the alpha a bit
			fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, Mathf.Clamp(fader.color.a - (Time.deltaTime / fadeTime), 0, 255));

			//Yeild to other processes
			await UniTask.Yield(PlayerLoopTiming.Update);
		}
	}

	public async UniTask FadeScreenOut() {

		//If the alpha is less than 1 aka full visible
		while (fader.color.a < 1) {

			Debug.Log(fader.color.a);
			//Increase the alpha a bit
			fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, Mathf.Clamp(fader.color.a + (Time.deltaTime / fadeTime), 0, 1));

			//Yeild to other processes
			await UniTask.Yield(PlayerLoopTiming.Update);
		}

	}

	public async void ChangeScene(string scene) {

		//Fade the screen out
		await FadeScreenOut();

		//Wait a beat
		await UniTask.Delay(100);

		//Hide the ui
		ResetUI();

		//Load the new scene
		SceneManager.LoadScene(scene);

		//Wait few beats
		await UniTask.Delay(300);

		//Fade the scene back in
		await FadeScreenIn();
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
	}

	public async void ShowUpgradeMenu() {
		//Hide all the menu items
		ResetUI();

		//Show the options screen
		uiUpgradeMenu.SetActive(true);
	}

	public async void PausePlaying() {

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


	public async void ResumePlaying() {

		//Hide all the menu items
		ResetUI();

		gc = getGameController();

		if (gc is not null) {

			//Restore the last gameplay state
			gc.SetState(savedGameControllerState);
			
			//Clear the last gameplay state
			savedGameControllerState = null;
		}

	}

}
