using System;
using UnityEngine;

public class LoadingScreenScript : MonoBehaviour, CommonScreenInterface {

	public ScreenSelector levelScreenSelector;

	private void LoadLocation(GameState gameState) {
		Debug.Log ("Start loading location: " + gameState.location.getName());

		if ((gameState.location.firstPhraseNpcId > 0) && (gameState.location.firstPhraseId > 0) && (!gameState.returnToLocationFlag)) {
			Debug.Log ("Go to dialog screen");
			levelScreenSelector.SelectScreen ((int)LevelScreenIndices.LOCATION_DIALOG);
			return;
		}

		gameState.returnToLocationFlag = false;
		levelScreenSelector.SelectScreen ((int)LevelScreenIndices.LOCATION);
	}
	
	private void LoadSector(GameState gameState) {
		Debug.Log ("Start loading sector: " + gameState.sector.getName());
		levelScreenSelector.SelectScreen ((int)LevelScreenIndices.SECTOR);
	}

	private void LoadMap(GameState gameState) {
		Debug.Log ("Start loading map.");
		levelScreenSelector.SelectScreen ((int)LevelScreenIndices.MAP);
	}

	public void OnLoad() {
		Debug.Log ("[" + ToString() + "] Loaded");
		GameState currGameState = GlobalData.gameStateManager.GetCurrentGameState ();
		if (currGameState.sector != null) {
			if (currGameState.location != null)
				LoadLocation (currGameState);
			else
				LoadSector (currGameState);
		} else {
			LoadMap (currGameState);
		}
	}
}

