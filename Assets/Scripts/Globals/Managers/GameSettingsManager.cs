using System;
using UnityEngine;
using System.Collections.Generic;

public class GameSettingsManager
{
	private LevelData levelData;

	private uint selectedLanguageIndex = 0;
	private Language selectedLanguage = null;


	public GameSettingsManager (LevelData levelData) {
		this.levelData = levelData;
	}

	public string GetRandomName() {
		Debug.LogWarning ("[GameSettingsManager] Make random names");
		return "Рандовася";
	}

	public List<string> GetLanguagesList() {
		List<string> langList = new List<string>();
		foreach (Language language in levelData.languages.Values)
			langList.Add (language.name);
		return langList;
	}
	
	public uint GetSelectedLanguageIndex() {
		return selectedLanguageIndex;
	}

	public string GetSelectedLanguageCode() {
		if (selectedLanguage == null) {
			Debug.LogWarning("Selected Language is null. Used [ru] by default");
			return "ru";
		}
		return selectedLanguage.code;
	}
	
	public void SetSelectedLanguageIndex(uint index) {
		if (index >= levelData.languages.Count) 
			index = 0;
		selectedLanguageIndex = index;

		List<BaseClass> langList = new List<BaseClass>();
		langList.AddRange (levelData.languages.Values);

		selectedLanguage = (Language)langList[(int)selectedLanguageIndex];
	}
}


