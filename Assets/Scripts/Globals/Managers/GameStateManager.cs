using System;
using UnityEngine;
using System.Xml.Linq;
using System.IO;

public class CharacterState {
	public string name;
	public CharacterState() {
		this.name = "Noname";
	}
}

public class GameState {
	public CharacterState charState = null;
	public SectorState sector = null;
	public LocationState location = null;

	public bool returnToLocationFlag = false;

	private string saveFileName = "";
	private const string RES_EXT = ".rsf";
	private const string GS_EXT = ".gsf";
	private const string SAVE_ROOT_ELEMENT_NAME = "gamestate";
	private const string SAVE_SECTORID_ELEMENT_NAME = "sectorid";
	private const string SAVE_LOCID_ELEMENT_NAME = "locationid";

	private void SaveGameStateToFile(string filename) {
		XElement rootElement = new XElement(SAVE_ROOT_ELEMENT_NAME);
		XDocument xmlLevelData = new XDocument (rootElement);

		rootElement.Add (new XElement (SAVE_SECTORID_ELEMENT_NAME, sector == null ? 0 : sector.GetId()));
		rootElement.Add (new XElement (SAVE_LOCID_ELEMENT_NAME, location == null ? 0 : location.GetId()));

		Directory.CreateDirectory (Directory.GetParent(filename).FullName);
		xmlLevelData.Save (filename);
	}

	private void LoadGameStateFromFile(string filename) {
		if (!File.Exists (filename)) {
			Debug.LogWarning ("Save file not exists");
			return;
		}
		XDocument xmlLevelData = XDocument.Load (filename);
		XElement rootElement = (XElement)xmlLevelData.LastNode;
		if (rootElement.Name.LocalName != SAVE_ROOT_ELEMENT_NAME) {
			Debug.LogWarning ("Save file is not compatible format");
			return;
		}

		sector = null;
		location = null;

		foreach (XElement groupElement in rootElement.Nodes()) {
			switch (groupElement.Name.LocalName) {
			case SAVE_SECTORID_ELEMENT_NAME: {
					sector = GlobalData.resourcesManager.GetById<SectorState> (uint.Parse (groupElement.Value));
					break;
				}
			case SAVE_LOCID_ELEMENT_NAME: {
					location = GlobalData.resourcesManager.GetById<LocationState> (uint.Parse (groupElement.Value));
					break;
				}
			}
		}
	}

	public void SetSaveFileName(string saveFileName) {
		this.saveFileName = saveFileName;
	}

	public void LoadDataFromFiles() {
		GlobalData.resourcesManager.LoadResourcesFromFile (saveFileName + RES_EXT);
		LoadGameStateFromFile (saveFileName + GS_EXT);
	}

	public void SaveDataToFiles() {
		GlobalData.resourcesManager.SaveResourcesToFile (saveFileName + RES_EXT);
		SaveGameStateToFile (saveFileName + GS_EXT);
	}

	public void GetInfoFromSavedFile() {
		Debug.LogWarning ("Make loading gamestate save info!");
	}

	public override String ToString() {
		String resultString = "";

		if (charState == null)
			resultString += "[charState:null, ";
		else
			resultString += "[charState:" + charState.name + ", ";

		if (sector == null)
			resultString += "sector:null, ";
		else
			resultString += "sector:" + sector.GetName() + ", ";

		if (location == null)
			resultString += "location:null]";
		else
			resultString += "location:" + location.GetName() + "]";
		return resultString;
	}
}

public class GameStateManager
{
	private LevelData levelData = null;
	private GameState gameState = new GameState();

	public GameStateManager(LevelData levelData) {
		this.levelData = levelData;
	}

	public GameState GetCurrentGameState() {
		return gameState;
	}
	
	public GameState GetNewGameState() {
		GameState newGameState = new GameState ();

		if (levelData.sectors.ContainsKey (levelData.startParameters.sectorID))
			newGameState.sector = GlobalData.resourcesManager.GetById<SectorState> (levelData.startParameters.sectorID);
		else 
			newGameState.sector = null;

		if (levelData.locations.ContainsKey (levelData.startParameters.locationID))
			newGameState.location = GlobalData.resourcesManager.GetById<LocationState> (levelData.startParameters.locationID);
		else 
			newGameState.location = null;

		newGameState.charState = new CharacterState ();

		Debug.Log(newGameState.ToString() + " Game state created");

		return newGameState;
	}
	
	public GameState GetSavedGameState(int saveSlot) {
		GameState savedGameState = new GameState ();
		savedGameState.SetSaveFileName ("save" + saveSlot.ToString ());
		return savedGameState;
	}
	
	public void ApplyGameState(GameState gameState) {
		GlobalData.resourcesManager.ResetResources ();
		this.gameState = gameState;
		gameState.LoadDataFromFiles ();	 
		Debug.Log(gameState.ToString() + " Game state applied");
	}
	
	public void SaveCurrentGameState(int saveSlot) {
		gameState.SetSaveFileName ("save" + saveSlot.ToString ());
		gameState.SaveDataToFiles ();
	}
}

