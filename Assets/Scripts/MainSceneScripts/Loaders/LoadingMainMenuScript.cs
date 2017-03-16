using UnityEngine;
using System.Collections;

public class LoadingMainMenuScript : MonoBehaviour, CommonScreenInterface {

	public HighlightController highlightController;

	public void OnLoad() {
		Debug.Log ("[" + ToString() + "] Loaded");
		highlightController.ClearHighlights ();
	}
}
