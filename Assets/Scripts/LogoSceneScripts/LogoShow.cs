using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogoShow : MonoBehaviour {

	public Image logo;
	private bool inverse;
	private float hideTime;

	void Start () {
		Color logoColor = logo.color;
		logoColor.a = 0;
		logo.color = logoColor;

		inverse = false;
		hideTime = 0;
	}

	void FixedUpdate () {
		if (!inverse) {
			if (logo.color.a < 1) {
				Color logoColor = logo.color;
				logoColor.a += 0.01f;
				logo.color = logoColor;
			} else 
				inverse = true;
		} else {
			if (logo.color.a > 0) {
				Color logoColor = logo.color;
				logoColor.a -= 0.01f;
				logo.color = logoColor;
				if (logo.color.a <= 0.02) hideTime = Time.time;
			}
		};
		if ((Input.GetKeyDown(KeyCode.Escape)) || ((hideTime != 0) && (Time.time - hideTime > 0.10f)))
			SceneManager.LoadScene (GlobalData.NAME_MAIN_SCENE);
	}
}
