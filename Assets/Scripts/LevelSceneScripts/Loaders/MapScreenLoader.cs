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

	private void OnSectorClickListener(SectorState sectorState) {
		GameState currGameState = GlobalData.gameStateManager.GetCurrentGameState ();
		currGameState.sector = sectorState;
		levelScreenSelector.SelectScreen ((int)LevelScreenIndices.LOADING);
	}

	private void AddSectorToContainer(SectorState sectorState, int sectorIndex) {
		GameObject currLocationObj = Instantiate (sectorPointPrefab, new Vector3 (), new Quaternion ()) as GameObject;

		// Реакция на нажатие
		GameObject secButton = currLocationObj.transform.FindChild ("Button").gameObject;
		secButton.GetComponent<Button> ().onClick.AddListener (() => OnSectorClickListener(sectorState));

		// Текст
		GameObject secText = secButton.transform.FindChild ("Text").gameObject;
		string secHiddenMark = sectorState.isVisible ? "" : "[*] ";
		secText.GetComponent<Text> ().text = secHiddenMark + sectorState.GetName();

		// Позиционирование
		RectTransform answRT = currLocationObj.GetComponent<RectTransform> ();
		answRT.SetParent(sectorsContainer);
		answRT.localScale = new Vector3 (1, 1, 1);
		answRT.anchorMin = sectorState.GetNormalCoords ();
		answRT.anchorMax = answRT.anchorMin;
		answRT.offsetMax = new Vector2 (0, 0);
		answRT.offsetMin = new Vector2 (0, 0);
	}

	public void OnLoad() {
		Debug.Log ("[" + ToString() + "] Loaded");
		ClearContainer ();
		List<SectorState> sectorList = GlobalData.resourcesManager.GetSectors ();
		Debug.Log ("Sectors on map: " + sectorList.Count);
		for (int secInd = 0; secInd < sectorList.Count; secInd++) {
			AddSectorToContainer (sectorList[secInd], secInd);
		}
	}
}
