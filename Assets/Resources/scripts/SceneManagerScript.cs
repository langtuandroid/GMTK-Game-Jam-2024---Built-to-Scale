using System;
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

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		fader.color = Color.black;
	}

	public UnityEngine.UI.Image fader;

	private async void Start() {
		//Fade the scene back in
		await FadeScreenIn();
		Debug.Log(123);
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

		//If the alpha is less than 255 aka full visible
		while (fader.color.a < 255) {

			//Increase the alpha a bit
			fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, Mathf.Clamp(fader.color.a + (Time.deltaTime / fadeTime), 0, 255));

			//Yeild to other processes
			await UniTask.Yield(PlayerLoopTiming.Update);
		}
	}

	public async void ChangeScene(string scene) {
		
		//Fade the screen out
		await FadeScreenOut();
		
		//Load the new scene
		SceneManager.LoadScene(scene);
		
		//Fade the scene back in
		await FadeScreenIn();
	}

	public async void LoadGame(string scene) {
		
		//Fade the screen out
		await FadeScreenOut();
		
		//Load the new scene
		SceneManager.LoadScene(scene);
		
		//Fade the scene back in
		await FadeScreenIn();
	}

	public async void ShowHowToPlay(string scene) {
		
		//Fade the screen out
		await FadeScreenOut();
		
		//Load the new scene
		SceneManager.LoadScene(scene);
		
		//Fade the scene back in
		await FadeScreenIn();
	}

	public async void ShowHighScores(string scene) {

		uiHighScores.SetActive(true);
	}
}
