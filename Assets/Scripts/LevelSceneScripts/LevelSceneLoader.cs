using UnityEngine;
using System.Collections;

public enum LevelScreenIndices {
	LOADING = 0,
	MAP = 1,
	LOCATION = 2,
	SECTOR = 3,
	LOCATION_SHOP = 4,
	LOCATION_DIALOG = 5,
	LOCATION_SERVICE = 6,
	ENDING = 7,
	SHIP_SETTINGS = 8,
	CUT_SCENE = 9
}

public class LevelSceneLoader : MonoBehaviour {

	public ScreenSelector levelScreenSelector;

	void Start () {
		levelScreenSelector.ClearScreens ();
		levelScreenSelector.SelectScreen ((int)LevelScreenIndices.LOADING);
	}

}
