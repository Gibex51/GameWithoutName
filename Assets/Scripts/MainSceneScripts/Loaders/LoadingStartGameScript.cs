using UnityEngine;
using System.Collections;

public class LoadingStartGameScript : MonoBehaviour, CommonScreenInterface {

	public HighlightController highlightController;
	public ScreenSelector screenSelector;
	public FillNewGameUI fillNewGameUI;

	public void OnLoad() {
		Debug.Log ("[" + ToString() + "] Loaded");
		screenSelector.ClearScreens ();
		screenSelector.SelectScreen ((int) StartGameScreenIndices.PLAYER);
		highlightController.ClearHighlights ();
		highlightController.HighlightObject (0);
		fillNewGameUI.UpdateUI ();
	}
}
