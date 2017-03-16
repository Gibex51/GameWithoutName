using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SectorScreenScript : MonoBehaviour, CommonScreenInterface {

	public GameObject locationPointPrefab;
	public Transform locationsContainer;
	public ScreenSelector levelScreenSelector;

	private void ClearContainer() {
		for (int childInd = 0; childInd < locationsContainer.childCount; childInd++) {
			DestroyObject (locationsContainer.GetChild (childInd).gameObject);
		}
	}

	private void OnLocationClickListener(LocationState locState) {
		GameState currGameState = GlobalData.gameStateManager.GetCurrentGameState ();
		currGameState.location = locState;
		levelScreenSelector.SelectScreen ((int)LevelScreenIndices.LOADING);
	}

	private void AddLocationToContainer(LocationState locState, int locationIndex) {
		GameObject currLocationObj = Instantiate (locationPointPrefab, new Vector3 (), new Quaternion ()) as GameObject;

		// Реакция на нажатие
		GameObject locButton = currLocationObj.transform.FindChild ("Button").gameObject;
		locButton.GetComponent<Button> ().onClick.AddListener (() => OnLocationClickListener(locState));

		// Текст
		GameObject locText = locButton.transform.FindChild ("Text").gameObject;
		string locHiddenMark = locState.isVisible ? "" : "[*] ";
		locText.GetComponent<Text> ().text = locHiddenMark + locState.GetName();

		// Позиционирование
		RectTransform answRT = currLocationObj.GetComponent<RectTransform> ();
		answRT.SetParent(locationsContainer);
		answRT.localScale = new Vector3 (1, 1, 1);
		answRT.anchorMin = new Vector2 (0.2f + Random.Range(0.0f, 0.6f), 0.2f + Random.Range(0.0f, 0.6f));
		answRT.anchorMax = answRT.anchorMin;
		answRT.offsetMax = new Vector2 (0, 0);
		answRT.offsetMin = new Vector2 (0, 0);
	}


	public void OnLoad() {
		Debug.Log ("[" + ToString() + "] Loaded");
		GameState currGameState = GlobalData.gameStateManager.GetCurrentGameState ();
		ClearContainer ();
		Debug.Log ("Locations in sector: " + currGameState.sector.GetLocationsCount ());
		for (int locInd = 0; locInd < currGameState.sector.GetLocationsCount (); locInd++) {
			AddLocationToContainer (currGameState.sector.GetLocation(locInd), locInd);
		}
	}
}
