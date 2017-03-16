using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class FillSettingsUI : MonoBehaviour {
	public GameObject languageDropdown;

	private void FillLanguages() {
		languageDropdown.GetComponent<Dropdown> ().options.Clear ();
		List<string> langList = GlobalData.gameSettingsManager.GetLanguagesList ();
		if (langList.Count > 0) {
			Dropdown dropdown = languageDropdown.GetComponent<Dropdown>();
			foreach (string langName in langList) {
				Dropdown.OptionData item = new Dropdown.OptionData();
				item.text = langName;
				dropdown.options.Add (item); ;
			}
			dropdown.value = (int)GlobalData.gameSettingsManager.GetSelectedLanguageIndex();
			dropdown.captionText.text = dropdown.options[dropdown.value].text;
		}
	}

	public void UpdateUI() {
		FillLanguages ();
	}
}