using UnityEngine;
using UnityEngine.Events;

public class GiftShopItemScript : MonoBehaviour {

	//The name that's printed for the player
	public Texture texture;
	
	//The name that's printed for the player
	public string visibleName;
	
	//This is the backend name used for running actions
	public string type;
	
	//Cost for a the player to buy
	public int unlockPrice;
	
	//Cost for a sceeple to buy
	public int price;
	
	//The image for the unlock menu
	public Sprite sprite;
}
