using UnityEngine;
using System.Collections;

public enum MainScreenIndices {
	MAIN_MENU = 0,
	START_GAME = 1,
}

public enum MenuScreenIndices {
	LOGO = 0,
	STATISTIC = 1,
	ACHIVEMENTS = 2,
	SHOP = 3,
	SETTINGS = 4,
	INFO = 5,
	ACCAUNT = 6
}

public enum StartGameScreenIndices {
	PLAYER = 0,
	SHIP = 1,
	PARTY = 2
}

public class MainSceneLoader : MonoBehaviour {

	public ScreenSelector mainScreenSelector;

	void LoadGameSettingsFromFile() {
		GlobalData.gameSettingsManager.SetSelectedLanguageIndex (0);
	}

	void Start () {
		mainScreenSelector.ClearScreens ();
		mainScreenSelector.SelectScreen ((int)MainScreenIndices.MAIN_MENU);
		LoadGameSettingsFromFile ();
	}
}
