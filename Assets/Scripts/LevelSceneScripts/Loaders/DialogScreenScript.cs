using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class DialogScreenScript : MonoBehaviour, CommonScreenInterface {

	private class AnswerHeightInfo {
		public float heightCoef;
		public RectTransform rt;

		public AnswerHeightInfo(float heightCoef, RectTransform rt) {
			this.heightCoef = heightCoef;
			this.rt = rt;
		}
	}

	public ScreenSelector levelScreenSelector;

	private float answerCoefFirstLine = 0.15f;
	private float answerCoefPerLine = 0.07f;
	private List<AnswerHeightInfo> answerCoefList = new List<AnswerHeightInfo> ();
	private NPCState currentNpc = null;
	private uint nextPhraseId = 0;

	public Text npcNameText;
	public Text npcPhraseText;
	public Transform answersContainer;
	public GameObject answerItemPrefab;

	public bool showHiddenAnswers = true;

	private void ClearContainer() {
		for (int childInd = 0; childInd < answersContainer.childCount; childInd++) {
			DestroyObject (answersContainer.GetChild (childInd).gameObject);
		}
		answerCoefList.Clear ();
	}

	private void ResizeContainer() {
		RectTransform rt = (RectTransform) answersContainer;

		float coefSum = 0;
		foreach (AnswerHeightInfo ai in answerCoefList)
			coefSum += ai.heightCoef;

		if (coefSum <= 1) {
			rt.anchorMin = new Vector2 (0, 0);
			float offset = 0;
			foreach (AnswerHeightInfo ai in answerCoefList) {
				ai.rt.anchorMin = new Vector2 (0, 1.0f - (offset + ai.heightCoef));
				ai.rt.anchorMax = new Vector2 (1, 1.0f - offset);
				offset += ai.heightCoef;
			}
		} else {
			rt.anchorMin = new Vector2 (0, 1.0f - coefSum);
			float offset = 0;
			foreach (AnswerHeightInfo ai in answerCoefList) {
				ai.rt.anchorMin = new Vector2 (0, 1.0f - (offset + ai.heightCoef / coefSum));
				ai.rt.anchorMax = new Vector2 (1, 1.0f - offset);
				offset += ai.heightCoef / coefSum;
			}
		}
			
		rt.localScale = new Vector3 (1, 1, 1);
		rt.anchorMax = new Vector2 (1, 1);
		rt.offsetMax = new Vector2 (0, 0);
		rt.offsetMin = new Vector2 (0, 0);
	}

	private void runAction(QuestAction action) {
		switch (action.actionType) {
		case ActionType.ACT_TYPE_ADD_MEMBER: // [ MAKE IT! ]
			{
				//member id
				Debug.Log("Action: Add member");
				break;
			}
		case ActionType.ACT_TYPE_CHANGE_QUEST:
			{
				GlobalData.resourcesManager.getById<QuestState>(uint.Parse (action.actionVariables [0]))
					.status = (QuestStatus) uint.Parse (action.actionVariables [1]);
				Debug.Log("Action: Change quest");
				break;
			}
		case ActionType.ACT_TYPE_CHANGE_RESOURCE: // [ MAKE IT! ]
			{
				//res id
				//res change
				Debug.Log("Action: Change resource");
				break;
			}
		case ActionType.ACT_TYPE_CHANGE_SPEECH:
			{
				GlobalData.resourcesManager.getById<NPCState>(uint.Parse (action.actionVariables [0]))
					.GetPhrase(uint.Parse (action.actionVariables [1]))
					.getAnswer(uint.Parse (action.actionVariables [2]))
					.isActive = bool.Parse (action.actionVariables [3]);
				Debug.Log("Action: Change speech");
				break;
			}
		case ActionType.ACT_TYPE_CHANGE_SUBQUEST: // [ MAKE IT! ]
			{
				//quest id
				//subquest id
				//subquest change 
				Debug.Log("Action: Change subquest");
				break;
			}
		case ActionType.ACT_TYPE_ENDGAME: // [ MAKE IT! ]
			{
				//hero is dead
				Debug.Log("Action: End game");
				break;
			}
		case ActionType.ACT_TYPE_GOTO_DIALOG:
			{
				uint nextNpcId = uint.Parse (action.actionVariables [0]);
				nextPhraseId = uint.Parse (action.actionVariables [1]);
				SelectNPC (nextNpcId);
				Debug.Log("Action: Goto dialog (npc id: " + nextNpcId + "; phrase id: " + nextPhraseId + ")");
				break;
			}
		case ActionType.ACT_TYPE_LOC_VISIBLE:
			{
				uint changedLocId = uint.Parse (action.actionVariables [0]);
				LocationState locState = GlobalData.resourcesManager.getById<LocationState> (changedLocId);
				if (locState != null)
					locState.isVisible = bool.Parse(action.actionVariables [1]);
				Debug.Log("Action: Location visibility change");
				break;
			}
		case ActionType.ACT_TYPE_MOVE_NPC:
			{
				uint changedLocId = uint.Parse (action.actionVariables [1]);
				LocationState locState = GlobalData.resourcesManager.getById<LocationState> (changedLocId);
				locState.firstPhraseNpcId = uint.Parse (action.actionVariables [0]);
				locState.firstPhraseId = uint.Parse (action.actionVariables [2]);

				Debug.Log("Action: Move NPC");
				break;
			}
		case ActionType.ACT_TYPE_REMOVE_RANDOM_MEMBER: // [ MAKE IT! ]
			{
				// N/A
				Debug.Log("Action: Remove random member");
				break;
			}
		case ActionType.ACT_TYPE_SECT_VISIBLE:
			{
				uint changedSecId = uint.Parse (action.actionVariables [0]);
				SectorState secState = GlobalData.resourcesManager.getById<SectorState> (changedSecId);
				if (secState != null)
					secState.isVisible = bool.Parse(action.actionVariables [1]);
				Debug.Log("Action: Sector visibility change");
				break;
			}
		case ActionType.ACT_TYPE_CHANGE_COUNTER:
			{
				AnswerState changedAnswer = GlobalData.resourcesManager.getById<NPCState> (uint.Parse (action.actionVariables [0]))
					.GetPhrase (uint.Parse (action.actionVariables [1]))
					.getAnswer (uint.Parse (action.actionVariables [2]));
				changedAnswer.decreaseCounter ();

				if (changedAnswer.getCounter () <= 0) {
					foreach (QuestAction cntAction in changedAnswer.actionsList) {
						if (!cntAction.isCounterAction)
							continue;

						try {
							runAction (cntAction);
						} catch (Exception e) {
							Debug.LogException (e);
							Debug.LogError ("Invalid Counter Action: " + changedAnswer.ToString ());
						}
					}
				}

				Debug.Log("Action: Change counter");
				break;
			}
		}
	}
		
	private void onAnswerClickListener(AnswerState answer) {
		Debug.Log ("Actions count: " + answer.actionsList.Count);

		nextPhraseId = answer.getNextPhraseId();

		if (answer.isAutohide())
			answer.isActive = false;

		foreach (QuestAction action in answer.actionsList) {
			if (action.isCounterAction) continue;

			try {
				runAction(action);
			} catch (Exception e) {
				Debug.LogException (e);
				Debug.LogError("Invalid Action: " + answer.ToString());
			}
		}
			
		Debug.Log("Clicked: " + nextPhraseId);
		GoToNextPhrase(nextPhraseId);
	}

	private void AddAnswerToContainer(AnswerState answerState, int answerIndex) {
		GameObject currAnswerObj = Instantiate (answerItemPrefab, new Vector3 (), new Quaternion ()) as GameObject;

		// Реакция на нажатие
		GameObject answButton = currAnswerObj.transform.FindChild ("Button").gameObject;
		answButton.GetComponent<Button> ().onClick.AddListener (() => onAnswerClickListener(answerState));

		// Текст
		GameObject answText = answButton.transform.FindChild ("Text").gameObject;
		string answerHiddenMark = answerState.isActive ? "" : "[*] ";
		Text answerTextComp = answText.GetComponent<Text> ();
		answerTextComp.text = answerHiddenMark + answerState.getText();

		// Позиционирование
		RectTransform answRT = currAnswerObj.GetComponent<RectTransform> ();
		answRT.SetParent(answersContainer);
		answRT.localScale = new Vector3 (1, 1, 1);
		answRT.anchorMin = new Vector2 (0, 0);
		answRT.anchorMax = new Vector2 (1, 1);
		answRT.offsetMax = new Vector2 (0, 0);
		answRT.offsetMin = new Vector2 (0, 0);

		Canvas.ForceUpdateCanvases();
		answerCoefList.Add (new AnswerHeightInfo (answerCoefFirstLine + answerCoefPerLine * (answerTextComp.cachedTextGenerator.lineCount - 1), answRT));
	}

	private void ReturnToLocation() {
		GameState currGameState = GlobalData.gameStateManager.GetCurrentGameState ();
		currGameState.returnToLocationFlag = true;
		levelScreenSelector.SelectScreen ((int)LevelScreenIndices.LOADING);
	}

	private void GoToNextPhrase(uint phraseId) {
		if (phraseId == 0) {
			ReturnToLocation ();
			return;
		}

		PhraseState phrase = currentNpc.GetPhrase (phraseId);
		if (phrase == null) {
			Debug.LogWarning ("Current dialog phrase is null!");
			return;
		}
		Debug.Log ("Goto Phrase: " + phrase.ToString());

		npcPhraseText.text = phrase.getText ();

		List<AnswerState> answers = phrase.getAnswers ();

		ClearContainer ();
		for (int answInd = 0; answInd < answers.Count; answInd++) {
			AnswerState answer = answers [answInd];
			if ((answer.isActive) || (showHiddenAnswers)) {
				AddAnswerToContainer (answer, answInd);
				Debug.Log ("Answer: " + answer.ToString ());
			} else {
				Debug.Log ("Skip hidden answer: " + answer.ToString ());
			}
		}
		ResizeContainer ();
	}

	private void SelectNPC(uint npcId) {
		currentNpc = GlobalData.resourcesManager.getById<NPCState> (npcId);
		if (currentNpc == null) {
			Debug.LogWarning ("Current dialog npc is null!");
			return;
		}
		Debug.Log ("NPC selected: " + currentNpc.ToString());

		npcNameText.text = currentNpc.getName ();
	}

	public void OnLoad() {
		Debug.Log ("[" + ToString() + "] Loaded");
		GameState currGameState = GlobalData.gameStateManager.GetCurrentGameState ();

		if (currGameState.location == null) {
			Debug.LogWarning ("Current dialog location is null!");
			return;
		}

		SelectNPC(currGameState.location.firstPhraseNpcId);
		GoToNextPhrase (currGameState.location.firstPhraseId);
	}
}
