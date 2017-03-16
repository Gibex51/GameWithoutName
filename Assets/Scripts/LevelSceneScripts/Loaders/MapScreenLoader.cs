using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapScreenLoader : MonoBehaviour, CommonScreenInterface {

	public GameObject sectorPointPrefab;
	public Transform sectorsContainer;
	public ScreenSelector levelScreenSelector;

	private void ClearContainer() {
		for (int childInd = 0; childInd < sectorsContainer.childCount; childInd++) {
			DestroyObject (sectorsContainer.GetChild (childInd).gameObject);
		}
	}

	private void onSectorClickListener(SectorState sectorState) {
		GameState currGameState = GlobalData.gameStateManager.GetCurrentGameState ();
		currGameState.sector = sectorState;
		levelScreenSelector.SelectScreen ((int)LevelScreenIndices.LOADING);
	}

	private void addSectorToContainer(SectorState sectorState, int sectorIndex) {
		GameObject currLocationObj = Instantiate (sectorPointPrefab, new Vector3 (), new Quaternion ()) as GameObject;

		// Реакция на нажатие
		GameObject SecButton = currLocationObj.transform.FindChild ("Button").gameObject;
		SecButton.GetComponent<Button> ().onClick.AddListener (() => onSectorClickListener(sectorState));

		// Текст
		GameObject SecText = SecButton.transform.FindChild ("Text").gameObject;
		string secHiddenMark = sectorState.isVisible ? "" : "[*] ";
		SecText.GetComponent<Text> ().text = secHiddenMark + sectorState.getName();

		// Позиционирование
		RectTransform AnswRT = currLocationObj.GetComponent<RectTransform> ();
		AnswRT.SetParent(sectorsContainer);
		AnswRT.localScale = new Vector3 (1, 1, 1);
		AnswRT.anchorMin = sectorState.getNormalCoords ();
		AnswRT.anchorMax = AnswRT.anchorMin;
		AnswRT.offsetMax = new Vector2 (0, 0);
		AnswRT.offsetMin = new Vector2 (0, 0);
	}

	public void OnLoad() {
		Debug.Log ("[" + ToString() + "] Loaded");
		ClearContainer ();
		List<SectorState> sectorList = GlobalData.resourcesManager.getSectors ();
		Debug.Log ("Sectors on map: " + sectorList.Count);
		for (int secInd = 0; secInd < sectorList.Count; secInd++) {
			addSectorToContainer (sectorList[secInd], secInd);
		}
	}
}
