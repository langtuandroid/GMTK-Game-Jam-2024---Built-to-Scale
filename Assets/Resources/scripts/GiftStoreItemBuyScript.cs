using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GiftStoreItemBuyScript : MonoBehaviour {
	public Sprite toolbarImage;
	public Image toolbarImageHolder;
	public EventTrigger eventTrigger;
	public TextMeshProUGUI priceText;
	public string upgradeName;
	public int price;
	public GiftShopItemScript giftShopItem;
	private GameController gc;




	private void Start() {

		//Shorthand the game controller
		gc = FindFirstObjectByType<GameController>();
		price = giftShopItem.unlockPrice;
		upgradeName = giftShopItem.visibleName;

		toolbarImageHolder.sprite = toolbarImage;
		priceText.text = $"{upgradeName}: ${price}";

		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
		eventTrigger.triggers.Add(entry);
	}
	void Update() {
		if (toolbarImageHolder is not null) {
			if (gc.funds < price) {
				toolbarImageHolder.color = new Color(1, 1, 1, 0.5f);
				priceText.color = new Color(0, 0, 0, 0.5f);
			}
			else {
				toolbarImageHolder.color = new Color(255, 255, 255, 1f);
				priceText.color = new Color(0, 0, 0, 1f);
			}
		}

	}

	public void OnPointerDownDelegate(PointerEventData data) {
		gc.TryBuyGiftShopItem(this);
	}
}
