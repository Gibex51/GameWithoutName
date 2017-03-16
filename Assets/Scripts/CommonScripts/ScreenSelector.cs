using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct CScreen {
	public GameObject screen;
}

public interface CommonScreenInterface {
	void OnLoad();
}

public class ScreenSelector : MonoBehaviour {

	public CScreen[] Screens;
	private int currentScreen = -1;

    public void ClearScreens(){
		for (int scrInd = 0; scrInd < Screens.Length; scrInd++) {
			Screens[scrInd].screen.SetActive(false);
		} 
		Debug.Log ("[" + transform.gameObject.name+ "] Screens Cleared");
        //SelectScreen(0);
    }

	private IEnumerator ExecuteEvents() {
		MonoBehaviour[] components = Screens[currentScreen].screen.GetComponents<MonoBehaviour> ();
		for (int compInd = 0; compInd < components.Length; compInd++) {
			if (!(components[compInd] is CommonScreenInterface)) continue;
			(components[compInd] as CommonScreenInterface).OnLoad ();
		}
		yield break;
	}

	public void SelectScreen(int screenIndex) {
        if ((screenIndex < 0) || (screenIndex >= Screens.Length)){        
            Debug.Log("["+ transform.name + "] " + "Screen selection: Failed");
            return;
        }

		if (currentScreen >= 0)
			Screens [currentScreen].screen.SetActive (false);
		Screens [screenIndex].screen.SetActive (true);

		currentScreen = screenIndex;
		Debug.Log ("[" + transform.name + "] " + "Screen selected: " + screenIndex);

		StartCoroutine (ExecuteEvents ());
	}
}
