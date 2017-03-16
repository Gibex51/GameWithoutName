using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuEvents : MonoBehaviour {

	public Text charNameText;
	public ScreenSelector mainScreenSelector;

	private void CreateAndSetNewGameState() {
		GameState newGameState = GlobalData.gameStateManager.GetNewGameState ();
		newGameState.charState.name = charNameText.text;
		GlobalData.gameStateManager.ApplyGameState (newGameState);
	}

	public void continueButtonClick(){
		SceneManager.LoadScene (GlobalData.NAME_LEVEL_SCENE);
	}

	public void startGameButtonClick(){
		CreateAndSetNewGameState ();
		SceneManager.LoadScene (GlobalData.NAME_LEVEL_SCENE);
	}

	public void newGameButtonClick(){
		mainScreenSelector.SelectScreen ((int)MainScreenIndices.START_GAME);
	}

	public void exitButtonClick(){
		Application.Quit ();
	}

	public void loadButtonClick() {
		GameState newGameState = GlobalData.gameStateManager.GetSavedGameState (1);
		GlobalData.gameStateManager.ApplyGameState (newGameState);
		SceneManager.LoadScene (GlobalData.NAME_LEVEL_SCENE);
	}
}
