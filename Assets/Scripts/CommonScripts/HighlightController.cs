using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[Serializable]
public struct CHighlightObject
{
    public Image compImage;
}

public class HighlightController : MonoBehaviour {

    public CHighlightObject[] Objects;
    private int currentObject = 0;
	public Color DefaultColor;
	public Color HighlightedColor;

    // Use this for initialization
    public void ClearHighlights () {
        for (int objInd = 0; objInd < Objects.Length; objInd++){
			Objects[objInd].compImage.color = DefaultColor;
        }
		currentObject = -1;
		Debug.Log ("["+ transform.gameObject.name +"] Highlights Cleared");
    }
	
	public void HighlightObject(int ObjectIndex){
		if (currentObject >= 0)
			Objects[currentObject].compImage.color = DefaultColor;
		if ((ObjectIndex >= 0) && (ObjectIndex < Objects.Length)) {
			Objects[ObjectIndex].compImage.color = HighlightedColor;
			currentObject = ObjectIndex;
		} else {
			currentObject = -1;
		}
		Debug.Log ("["+ transform.gameObject.name +"] Highlight:" + currentObject.ToString ());
	}
}
