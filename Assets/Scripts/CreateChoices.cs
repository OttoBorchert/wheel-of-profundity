using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CreateChoices : MonoBehaviour {

	//Used to add choices in the inspector
	[SerializeField]
	private List<string> choices;

	//The default prefab for a button
	[SerializeField]
	private GameObject button;

	//What we add the button to
	[SerializeField]
	private Transform canvas;

	//Reference to the button that causes the spinning (so we can deactivate it)
	[SerializeField]
	private GameObject spinButton;

	//Animation variables
	private bool isAnimating = false;
	private List<Vector3> finalDestinations = new List<Vector3>(); //Final resting places of winners
	private int finalDestinationIndex = 0; //Current index within the finalPositions list (plaec we are moving to)
	private Vector3 originalLocation; //Original location of currently animating button
	private float alphaTime = 0.0f; //Alpha time of the lerp time parameter for the animation

	//Spinning variables
	private bool isSpinning = false;
	private int activeChoice = 0; //Choice that is colored to show it is active
	private List<GameObject> buttons = new List<GameObject> (); //List of all of our buttons, slowly removed from as we pick winners
	private float timeToNextSwitch = 0f;
	private float speed; //In button switches / second


	// Use this for initialization
	void Start () {

		//Determine the location to start the buttons 
		//TODO: Take out the hardcoded variables here
		int x = Screen.width / 2 - 80;
		int y = Screen.height - 90;

		//Make buttons based on the values entered in the inspector
		for (int i = 0; i < choices.Count; i++) {
			GameObject newButton = Instantiate (button);
			newButton.transform.SetParent (canvas, false);
			Text text = newButton.GetComponentInChildren<Text> ();
			text.text = choices[i];
			RectTransform rectTransform = newButton.GetComponent<RectTransform> ();
			Vector3 newPosition = rectTransform.position;
			newPosition.x = x;
			newPosition.y = y;
			rectTransform.position = newPosition;
			finalDestinations.Add (new Vector3 (95, y, 0));
			buttons.Add (newButton);
			y -= 50;
		}
	}
	
	void Update () {

		if (isSpinning) {
			if (timeToNextSwitch <= 0.0f) {
				activeChoice += 1;
				activeChoice %= buttons.Count;
				timeToNextSwitch = 1 / speed;
				speed -= 1; //TODO: Also should not be hardcoded. Might be fun to be a little random too. 
			}
			timeToNextSwitch -= Time.deltaTime;
			if (speed < 1) { //TODO: More hardcoded values
				isAnimating = true;
				isSpinning = false;
				originalLocation = buttons [activeChoice].GetComponent<RectTransform> ().position;
			}
		}

		if (isAnimating) {
			alphaTime += Time.deltaTime * .6f;
			Vector3 newPosition = Vector3.Lerp (originalLocation, finalDestinations [finalDestinationIndex], alphaTime);
			buttons [activeChoice].GetComponent<RectTransform> ().position = newPosition;
			if (alphaTime > 1) {
				isAnimating = false;
				buttons.Remove (buttons [activeChoice]);
				choices.RemoveAt (activeChoice);
				activeChoice = 0;
				spinButton.SetActive (true);
				alphaTime = 0;
				finalDestinationIndex++;
				//If there's only two buttons left, don't make them spin it again...
				if (buttons.Count == 1) {
					isAnimating = true;
					spinButton.SetActive (false);
					originalLocation = buttons [activeChoice].GetComponent<RectTransform> ().position;
				} else if (buttons.Count == 0) {
					isAnimating = false;
					spinButton.SetActive (false);
				}
			}
		}

		//Setting the colors of the buttons
		for (int i = 0; i < buttons.Count; i++) {
			Image image = buttons [i].GetComponent<Image> ();
			image.color = new Color (1, 1, 1); //TODO: Colors should be user selectable - both foreground and background
		}
		if (buttons.Count > 0) {
			Image activeImage = buttons [activeChoice].GetComponent<Image> ();
			activeImage.color = new Color (200, 0, 0); //TODO: Active color should be user selectable - both foreground and background
		}
		//TODO: Might be more fun if the colors near the active color were a different color too.
	}

	//Called by the Spin Wheel button, to start the WHEEL OF PROFUNDITY!
	public void SpinWheel() {
		if (!isSpinning) {
			speed = (int)(Random.value * 50) + 20; //TODO: You can tell I was in a hurry, more hardcoded values.
			timeToNextSwitch = 1 / speed;
			isSpinning = true;
			spinButton.SetActive (false);
		}
	}
}
