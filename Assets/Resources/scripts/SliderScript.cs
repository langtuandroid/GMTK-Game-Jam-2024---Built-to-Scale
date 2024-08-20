using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour {

	public float min;
	public float max;

	public Image image;
	public Sprite spriteBase;
	public Sprite spriteActive;
	private Sprite currentSprite;
	public Slider slider;

	// Start is called before the first frame update
	void Start() {

		slider = GetComponent<Slider>();
		currentSprite = spriteBase;
		slider.minValue = min;
		slider.maxValue = max;
	}

	// Update is called once per frame
	void Update() {
		if (EventSystem.current.currentSelectedGameObject == gameObject) {
			currentSprite = spriteActive;
		}
		else {
			currentSprite = spriteBase;
		}

		image.sprite = currentSprite;
	}

}
