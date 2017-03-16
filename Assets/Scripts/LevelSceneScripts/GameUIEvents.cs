using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameUIEvents : MonoBehaviour {

	public ScreenSelector levelScreenSelector;

	public void menuButtonClick(){
		SceneManager.LoadScene (GlobalData.NAME_MAIN_SCENE);
	}

    public void exitButtonClick(){
        Application.Quit();
    }
		
	public void returnToSectorButtonClick() {
		GameState currGameState = GlobalData.gameStateManager.GetCurrentGameState ();
		currGameState.location = null;
		levelScreenSelector.SelectScreen ((int)LevelScreenIndices.LOADING);
	}

	public void returnToMapButtonClick() {
		GameState currGameState = GlobalData.gameStateManager.GetCurrentGameState ();
		currGameState.sector = null;
		levelScreenSelector.SelectScreen ((int)LevelScreenIndices.LOADING);
	}

	public void gotoLocation(uint locationId) {

	}

	public void saveButtonClick() {
		GlobalData.gameStateManager.SaveCurrentGameState (1);
	}
}
