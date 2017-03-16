using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class GlobalData {
	#region Singleton
	static GlobalData(){
		levelData = new LevelData ();
		levelData.LoadFromXML ("gameData");

		gameStateManager = new GameStateManager (levelData);
		gameSettingsManager = new GameSettingsManager (levelData);
		resourcesManager = new ResourcesManager (levelData);

		Debug.Log ("Global Data Initialized");
	}
	private static readonly GlobalData _instance = new GlobalData();
	public static GlobalData Instance{ get { return _instance; } }
	#endregion

	public const string NAME_MAIN_SCENE = "MainScene";
	public const string NAME_LEVEL_SCENE = "LevelScene";
	
	// Level Data
	private static LevelData levelData;

	// Game State Manager
	public static GameStateManager gameStateManager;

	// Game Settings Manager
	public static GameSettingsManager gameSettingsManager;

	// Resource Manager
	public static ResourcesManager resourcesManager;

}
