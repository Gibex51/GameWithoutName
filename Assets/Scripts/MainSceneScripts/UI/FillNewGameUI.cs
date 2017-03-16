using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FillNewGameUI : MonoBehaviour {
	public InputField charTextInput;

	public void UpdateUI() {
		charTextInput.text = GlobalData.gameSettingsManager.GetRandomName ();
	}
}
