using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameUIEvents : MonoBehaviour {

	public ScreenSelector levelScreenSelector;

	public void MenuButtonClick(){
		SceneManager.LoadScene (GlobalData.NAME_MAIN_SCENE);
	}

    public void ExitButtonClick(){
        Application.Quit();
    }
		
	public void ReturnToSectorButtonClick() {
		GameState currGameState = GlobalData.gameStateManager.GetCurrentGameState ();
		currGameState.location = null;
		levelScreenSelector.SelectScreen ((int)LevelScreenIndices.LOADING);
	}

	public void ReturnToMapButtonClick() {
		GameState currGameState = GlobalData.gameStateManager.GetCurrentGameState ();
		currGameState.sector = null;
		levelScreenSelector.SelectScreen ((int)LevelScreenIndices.LOADING);
	}

	public void GotoLocation(uint locationId) {

	}

	public void SaveButtonClick() {
		GlobalData.gameStateManager.SaveCurrentGameState (1);
	}
}
