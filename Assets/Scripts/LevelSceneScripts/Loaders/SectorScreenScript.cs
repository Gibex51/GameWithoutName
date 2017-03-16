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

	private void onLocationClickListener(LocationState locState) {
		GameState currGameState = GlobalData.gameStateManager.GetCurrentGameState ();
		currGameState.location = locState;
		levelScreenSelector.SelectScreen ((int)LevelScreenIndices.LOADING);
	}

	private void addLocationToContainer(LocationState locState, int locationIndex) {
		GameObject currLocationObj = Instantiate (locationPointPrefab, new Vector3 (), new Quaternion ()) as GameObject;

		// Реакция на нажатие
		GameObject LocButton = currLocationObj.transform.FindChild ("Button").gameObject;
		LocButton.GetComponent<Button> ().onClick.AddListener (() => onLocationClickListener(locState));

		// Текст
		GameObject LocText = LocButton.transform.FindChild ("Text").gameObject;
		string locHiddenMark = locState.isVisible ? "" : "[*] ";
		LocText.GetComponent<Text> ().text = locHiddenMark + locState.getName();

		// Позиционирование
		RectTransform AnswRT = currLocationObj.GetComponent<RectTransform> ();
		AnswRT.SetParent(locationsContainer);
		AnswRT.localScale = new Vector3 (1, 1, 1);
		AnswRT.anchorMin = new Vector2 (0.2f + Random.Range(0.0f, 0.6f), 0.2f + Random.Range(0.0f, 0.6f));
		AnswRT.anchorMax = AnswRT.anchorMin;
		AnswRT.offsetMax = new Vector2 (0, 0);
		AnswRT.offsetMin = new Vector2 (0, 0);
	}


	public void OnLoad() {
		Debug.Log ("[" + ToString() + "] Loaded");
		GameState currGameState = GlobalData.gameStateManager.GetCurrentGameState ();
		ClearContainer ();
		Debug.Log ("Locations in sector: " + currGameState.sector.getLocationsCount ());
		for (int locInd = 0; locInd < currGameState.sector.getLocationsCount (); locInd++) {
			addLocationToContainer (currGameState.sector.getLocation(locInd), locInd);
		}
	}
}
