using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using LangString = System.Collections.Generic.Dictionary<string, string>;
using System;
using System.IO;

public enum ItemType {
	ITEM_TYPE_UNKNOWN     = -1,
	ITEM_TYPE_OTHER       = 0,
	ITEM_TYPE_WEAPON      = 1,
	ITEM_TYPE_BOTMODULE   = 2,
	ITEM_TYPE_EQUIP       = 3,
	ITEM_TYPE_RESOURCES   = 4
};

public enum QuestStatus {
	QUEST_STATUS_NOT_TAKEN = 0,
	QUEST_STATUS_TAKEN     = 1,
	QUEST_STATUS_COMPLETE  = 2,
	QUEST_STATUS_FAILED    = 3,
	QUEST_STATUS_CANCELED  = 4
}

public enum ActionType {
	ACT_TYPE_NONE                 = -1,
	ACT_TYPE_CHANGE_RESOURCE      = 0,
	ACT_TYPE_MOVE_NPC             = 1,
	ACT_TYPE_CHANGE_QUEST         = 2,
	ACT_TYPE_CHANGE_SUBQUEST      = 3,
	ACT_TYPE_CHANGE_SPEECH        = 4,
	ACT_TYPE_ADD_MEMBER           = 5,
	ACT_TYPE_REMOVE_RANDOM_MEMBER = 6,
	ACT_TYPE_GOTO_DIALOG          = 7,
	ACT_TYPE_SECT_VISIBLE         = 8,
	ACT_TYPE_LOC_VISIBLE          = 9,
	ACT_TYPE_ENDGAME              = 10,
	ACT_TYPE_CHANGE_COUNTER       = 11,
}

public enum ShipCellType {
	SC_TYPE_VOID   = 0,
	SC_TYPE_EMPTY  = 1,
	SC_TYPE_FILLED = 2,
	SC_TYPE_OPENED = 3,
	SC_TYPE_CLOSED = 4
}

public enum ShipInterierType {
	SI_TYPE_EMPTY     = 0,
	SI_TYPE_FILL      = 1,
	SI_TYPE_ENGINE    = 2,
	SI_TYPE_WEAPON    = 3,
	SI_TYPE_STAIRS    = 4,
	SI_TYPE_TERMINAL  = 5,
	SI_TYPE_COMPUTER  = 6,
	SI_TYPE_OXYGEN    = 7,
	SI_TYPE_FUEL      = 8,
	SI_TYPE_FUELTANK  = 9,
	SI_TYPE_MEDICINE  = 10,
	SI_TYPE_BOTS      = 11,
	SI_TYPE_TELEPORT  = 12,
	SI_TYPE_TELEMETRY = 13
}

public enum CrewState {
	CREW_STATE_NONE       = 0,
	CREW_STATE_STAY       = 1,
	CREW_STATE_GO         = 2,
	CREW_STATE_WORK       = 3,
	CREW_STATE_UPSTAIRS   = 4,
	CREW_STATE_DOWNSTAIRS = 5
}

public enum CrewMemberType {
	CM_TYPE_NORMAL        = 0,
	CM_TYPE_HERO          = 1,
	CM_TYPE_RANDOM        = 2
}

public enum ConditionType {
	COND_TYPE_HERODIE       = 0,
	COND_TYPE_QUESTCOMPLETE = 1,
	COND_TYPE_ITEMCOLLECTED = 2
}

public struct Point {
	public int x, y;
}

public class BaseClass {
	public uint id;
}

public class BaseNamedClass : BaseClass {
	public LangString name;
	public BaseNamedClass() {
		name = new LangString ();
	}
}

public class Language : BaseClass {
	public string name;
	public string code;
}

public class Sector : BaseNamedClass {
	public LangString description;
	public bool visible;
	public Point coords;
	public uint backgroundTextureId;
	public uint climateId;
	public uint fractionId;
	public List<uint> locationsId;

	public Sector() {
		description = new LangString ();
		locationsId = new List<uint> ();
	}
}

public class Location : BaseNamedClass {
	public LangString description;
	public bool visible;
	public bool isBackgroundRandom;
	public uint fixedBackgroundTextureId;
	public uint firstPhraseNpcId;
	public uint firstPhraseId;
	public uint shipId;

	public Location() {
		description = new LangString ();
	}
}

public class Climate : BaseNamedClass {
	public Point oceanCurrentDir;
	public uint waterTemperature;
	public uint pressure;
	public uint visibility;
	public uint radiation;
}

public class Fraction : BaseNamedClass {
	public LangString description;

	public Fraction() {
		description = new LangString ();
	}
}

public class TextureImage : BaseClass {
	public string name;
}

public class Item : BaseNamedClass {
	public LangString description;
	public ItemType itemType;
	public List<string> parameters;

	public Item() {
		description = new LangString ();
		parameters = new List<string>();
	}
}

public class Race : BaseNamedClass {
	public LangString description;

	public Race() {
		description = new LangString ();
	}
}

public class SubQuest : BaseNamedClass {
	public int currentValue;
	public int totalValue;
}

public class Quest : BaseNamedClass {
	public LangString description;
	public uint questGroupId;
	public int daysToFail;
	public QuestStatus status;
	public Dictionary<uint, SubQuest> subQuests;

	public Quest() {
		description = new LangString ();
		subQuests = new Dictionary<uint, SubQuest> ();
	}
}

public class QuestGroup : BaseNamedClass {
	//Empty
}

public class QuestAction {
	public ActionType actionType;
	public List<string> actionVariables;
	public bool isCounterAction;

	public QuestAction() {
		actionVariables = new List<string> ();
		isCounterAction = false;
	}

	public QuestAction(QuestAction qa) {
		this.actionType = qa.actionType;
		this.actionVariables = new List<string>(qa.actionVariables);
		this.isCounterAction = qa.isCounterAction;
	}
}

public class Answer : BaseClass {
	public LangString text;
	public bool active;
	public bool autohide;
	public int counter;
	public uint nextPhraseId;
	public List<QuestAction> actions;

	public Answer() {
		text = new LangString ();
		actions = new List<QuestAction> ();
		counter = 1;
		active = true;
		autohide = false;
		nextPhraseId = 0;
	}
}

public class Phrase : BaseClass {
	public LangString npcText;
	public bool isRoot;
	public SortedDictionary<uint, Answer> answers;

	public Phrase() {
		npcText = new LangString ();
		answers = new SortedDictionary<uint, Answer> ();
		isRoot = false;
	}
}

public class NPC : BaseNamedClass {
	public uint fractionId;
	public Dictionary<uint, Phrase> phrases;

	public NPC() {
		phrases = new Dictionary<uint, Phrase> ();
	}
}

public class ShipCell {
	public ShipCellType CellType;
	public ShipInterierType InterType;
	public CrewState CrewState;
}

public class Ship : BaseNamedClass {
	public LangString description;
	public List<List<ShipCell>> grid;
	public Point backOffset;
	public Point inwardOffset;
	public uint textureId;

	public Ship() {
		description = new LangString();
		grid = new List<List<ShipCell>> ();
	}
}

public class Achivement : BaseNamedClass {
	public LangString description;

	public Achivement() {
		description = new LangString();
	}
}

public class CrewMember : BaseNamedClass {
	public LangString description;
	public uint raceId;
	public CrewMemberType crewMemberType;

	public CrewMember() {
		description = new LangString();
	}
}

public class EndingCondition {
	public ConditionType cType;
	public bool cState;
	public string cValue;
}

public class Ending : BaseNamedClass {
	public LangString description;
	public List<EndingCondition> conditions;
	public Ending() {
		description = new LangString();
		conditions = new List<EndingCondition>();
	}
}

public struct StartParameters {
	public uint sectorID;
	public uint locationID;
}

public class LevelData  {
	private const string IDATTR_NAME = "id";
	private const string LANGATTR_NAME = "lang";
	private const string NAMENODE_NAME = "name";
	private const string DESCNODE_NAME = "description";
	private const string CODENODE_NAME = "code";
	private const string INFONODE_NAME = "info";
	private const string MISSING_STR = "<missing string>";
	private const string DEF_LANG_CODE = "ru";
	
	public StartParameters startParameters;
	public Dictionary<uint, BaseClass> 	languages = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	sectors = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	locations = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	climates = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	fractions = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	backgrounds = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	items = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	races = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	quests = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	questGroups = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	npcs = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	ships = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	achivements = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	crewMembers = new Dictionary<uint, BaseClass> ();
	public Dictionary<uint, BaseClass> 	endings = new Dictionary<uint, BaseClass> ();

	private struct LangStringPair {
		public LangString langString;
		public String nodeName;

		public LangStringPair(LangString langString, String nodeName) {
			this.langString = langString;
			this.nodeName = nodeName;
		}
	}

	//--------------------------------------------------------------------------------

	private XmlNode FindFirstChildNode(XmlNode parentNode, string childNodeName) {
		foreach (XmlNode childNode in parentNode.ChildNodes) 
			if (childNode.LocalName == childNodeName) return childNode;
		Debug.Log ("Parent Node [" + parentNode.LocalName + "]: Not found child node: " + childNodeName);
		return null;
	}

	private string SafeGetNodeValue(XmlNode node, string defaultValue) {
		if (node == null) {
			Debug.Log ("SafeGetNodeValue: NULL node");
			return defaultValue;
		}
		return node.InnerText;
	}

	private Language GetLanguageByCode(string code){
		foreach (Language lang in languages.Values)
			if (lang.code == code) return lang;
		Debug.Log ("GetLanguageByCode: Language not found " + code);
		return null;
	}

	private List<uint> StringToUintList(string inString) {
		List<uint> outList = new List<uint>();
		String[] strArray = inString.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
		foreach (string strItem in strArray)
			outList.Add (uint.Parse (strItem));
		return outList;
	}

	private List<string> StringToStringList(string inString) {
		List<string> outList = new List<string>();
		String[] strArray = inString.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
		outList.AddRange (strArray);
		return outList;
	}

	private void ReadLangStringFromNode(XmlNode node, params LangStringPair[] langStringPair) {
		foreach (XmlNode langChildNode in node.ChildNodes) {
			if (langChildNode.LocalName != INFONODE_NAME) continue;
			Language lang = GetLanguageByCode(langChildNode.Attributes[LANGATTR_NAME].Value);
			foreach (LangStringPair lsp in langStringPair)
				lsp.langString.Add (lang.code, SafeGetNodeValue (FindFirstChildNode (langChildNode, lsp.nodeName), MISSING_STR));
		}
		foreach (LangStringPair lsp in langStringPair)
			if (lsp.langString.Count == 0)
				lsp.langString.Add (DEF_LANG_CODE, MISSING_STR);
	}

	//--------------------------------------------------------------------------------

	private void ReadLanguagesFromNode (XmlNode node) {
		if (node == null) {
			Debug.Log ("ReadLanguagesFromNode: NULL node");
			return;
		}
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "language") continue;
			Language newLanguage = new Language();
			newLanguage.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);
			newLanguage.name = SafeGetNodeValue(FindFirstChildNode(childNode, NAMENODE_NAME), MISSING_STR);
			newLanguage.code = SafeGetNodeValue(FindFirstChildNode(childNode, CODENODE_NAME), MISSING_STR);
			languages.Add(newLanguage.id, newLanguage);
			Debug.Log ("Language Added :" + newLanguage.code); 
		}
	}

	private void ReadTexturesFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "texture") continue;
			TextureImage newTexture = new TextureImage();
			newTexture.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);
			newTexture.name = SafeGetNodeValue(FindFirstChildNode(childNode, NAMENODE_NAME), MISSING_STR);
			backgrounds.Add(newTexture.id, newTexture);
		}
	}

	private void ReadQuestGroupsFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "questgroup") continue;
			QuestGroup newQuestGroup = new QuestGroup();
			newQuestGroup.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);

			ReadLangStringFromNode (childNode, new LangStringPair(newQuestGroup.name, NAMENODE_NAME));

			questGroups.Add(newQuestGroup.id, newQuestGroup);
		}
	}

	private void ReadFractionsFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "fraction") continue;
			Fraction newFraction = new Fraction();
			newFraction.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);

			ReadLangStringFromNode (childNode, new LangStringPair(newFraction.name, NAMENODE_NAME),
				new LangStringPair(newFraction.description, DESCNODE_NAME));

			fractions.Add(newFraction.id, newFraction);
		}
	}

	private void ReadRacesFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "race") continue;
			Race newRace = new Race();
			newRace.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);

			ReadLangStringFromNode (childNode, new LangStringPair(newRace.name, NAMENODE_NAME),
				new LangStringPair(newRace.description, DESCNODE_NAME));
			
			races.Add(newRace.id, newRace);
		}
	}

	private void ReadCrewMembersFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "crewmember") continue;
			CrewMember newCrewMember = new CrewMember();
			newCrewMember.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);
			newCrewMember.raceId = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "raceid"), "0"));
			newCrewMember.crewMemberType = (CrewMemberType)int.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "cmtype"), "0"));

			ReadLangStringFromNode (childNode, new LangStringPair(newCrewMember.name, NAMENODE_NAME),
				new LangStringPair(newCrewMember.description, DESCNODE_NAME));

			crewMembers.Add(newCrewMember.id, newCrewMember);
		}
	}

	private void ReadClimatesFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "climate") continue;
			Climate newClimate = new Climate();
			newClimate.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);
			newClimate.pressure = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "pressure"), "0"));
			newClimate.radiation = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "radiation"), "0"));
			newClimate.visibility = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "visibility"), "0"));
			newClimate.waterTemperature = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "watertemp"), "0"));
			newClimate.oceanCurrentDir.x = int.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "dir_x"), "0"));
			newClimate.oceanCurrentDir.y = int.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "dir_y"), "0"));
		
			ReadLangStringFromNode (childNode, new LangStringPair(newClimate.name, NAMENODE_NAME));

			climates.Add(newClimate.id, newClimate);
		}
	}

	private void ReadAchivementsFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "achivement") continue;
			Achivement newAchivement = new Achivement();
			newAchivement.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);

			ReadLangStringFromNode (childNode, new LangStringPair(newAchivement.name, NAMENODE_NAME),
				new LangStringPair(newAchivement.description, DESCNODE_NAME));

			achivements.Add(newAchivement.id, newAchivement);
		}
	}

	private void ReadItemsFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "item") continue;
			Item newItem = new Item();
			newItem.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);
			newItem.itemType = (ItemType)int.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "type"), "0")); 
			newItem.parameters = StringToStringList(SafeGetNodeValue(FindFirstChildNode(childNode, "vars"), ""));

			ReadLangStringFromNode (childNode, new LangStringPair(newItem.name, NAMENODE_NAME),
				new LangStringPair(newItem.description, DESCNODE_NAME));

			items.Add(newItem.id, newItem);
		}
	}

	private void ReadQuestsFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "quest") continue;
			Quest newQuest = new Quest();
			newQuest.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);
			newQuest.status = (QuestStatus) int.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "state"), "0"));
			newQuest.questGroupId = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "questgroup_id"), "0"));;
			newQuest.daysToFail = int.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "daystofail"), "0"));

			ReadLangStringFromNode (childNode, new LangStringPair(newQuest.name, NAMENODE_NAME),
				new LangStringPair(newQuest.description, DESCNODE_NAME));

			foreach (XmlNode subQuestNode in childNode.ChildNodes) {
				if (childNode.LocalName != "subquest") continue;
				SubQuest newSubQuest = new SubQuest();
				newSubQuest.id = uint.Parse(subQuestNode.Attributes[IDATTR_NAME].Value);
				newSubQuest.currentValue = 0;
				newSubQuest.totalValue = int.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "total"), "0"));

				ReadLangStringFromNode (childNode, new LangStringPair(newSubQuest.name, NAMENODE_NAME));

				newQuest.subQuests.Add(newSubQuest.id, newSubQuest);
			}
			quests.Add(newQuest.id, newQuest);
		}
	}

	private void ReadLocationsFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "location") continue;
			Location newLocation = new Location();
			newLocation.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);
			newLocation.visible = bool.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "visible"), "true"));
			newLocation.fixedBackgroundTextureId = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "fixbgid"), "0"));
			newLocation.isBackgroundRandom = bool.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "isbgrand"), "false"));
			newLocation.firstPhraseNpcId = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "firstphnpc_id"), "0"));
			newLocation.firstPhraseId = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "firstph_id"), "0"));
			newLocation.shipId = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "ship_id"), "0"));

			ReadLangStringFromNode (childNode, new LangStringPair(newLocation.name, NAMENODE_NAME),
				new LangStringPair(newLocation.description, DESCNODE_NAME));
			
			locations.Add(newLocation.id, newLocation);
		}
	}

	private void ReadSectorsFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "sector") continue;
			Sector newSector = new Sector();
			newSector.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);
			newSector.coords.x = int.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "pos_x"), "0"));
			newSector.coords.y = int.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "pos_y"), "0"));
			newSector.backgroundTextureId = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "bg_id"), "0")); 
			newSector.climateId = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "climate_id"), "0")); 
			newSector.fractionId = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "fract_id"), "0")); 
			newSector.visible = bool.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "visible"), "true"));
			newSector.locationsId = StringToUintList(SafeGetNodeValue(FindFirstChildNode(childNode, "locations"), ""));

			ReadLangStringFromNode (childNode, new LangStringPair(newSector.name, NAMENODE_NAME),
				new LangStringPair(newSector.description, DESCNODE_NAME));
			
			sectors.Add(newSector.id, newSector);
		}
	}

	private void ReadNpcsFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "npc") continue;
			NPC newNpc = new NPC();
			newNpc.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);
			newNpc.fractionId = uint.Parse(SafeGetNodeValue(FindFirstChildNode(childNode, "fract_id"), "0")); 

			ReadLangStringFromNode(childNode, new LangStringPair(newNpc.name, NAMENODE_NAME));
			//Make NPC Type
							
			foreach (XmlNode phraseNode in childNode.ChildNodes) {
				if (phraseNode.LocalName != "phrase") continue;
				Phrase newPhrase = new Phrase ();
				newPhrase.id = uint.Parse(phraseNode.Attributes[IDATTR_NAME].Value);
				newPhrase.isRoot = bool.Parse(SafeGetNodeValue(FindFirstChildNode(phraseNode, "is_root"), "false"));

				ReadLangStringFromNode(phraseNode, new LangStringPair(newPhrase.npcText, "ph_text"));

				foreach (XmlNode answerNode in phraseNode.ChildNodes) {
					if (answerNode.LocalName != "answer") continue;
					Answer newAnswer = new Answer ();
					newAnswer.id = uint.Parse(answerNode.Attributes[IDATTR_NAME].Value);
					newAnswer.active = bool.Parse(SafeGetNodeValue(FindFirstChildNode(answerNode, "active"), "true"));
					newAnswer.autohide = bool.Parse(SafeGetNodeValue(FindFirstChildNode(answerNode, "autohide"), "false"));
					newAnswer.nextPhraseId = uint.Parse(SafeGetNodeValue(FindFirstChildNode(answerNode, "nextphrid"), "0"));
					newAnswer.counter = int.Parse(SafeGetNodeValue(FindFirstChildNode(answerNode, "counter"), "1"));

					ReadLangStringFromNode(answerNode, new LangStringPair(newAnswer.text, "ans_text"));

					foreach (XmlNode actionNode in answerNode.ChildNodes) {
						if (actionNode.LocalName != "action") continue;
						QuestAction qa = new QuestAction ();
						qa.actionType = (ActionType) int.Parse(SafeGetNodeValue(FindFirstChildNode(actionNode, "type"), "0"));
						qa.actionVariables = StringToStringList(SafeGetNodeValue(FindFirstChildNode(actionNode, "vars"), ""));
						qa.isCounterAction = bool.Parse(SafeGetNodeValue(FindFirstChildNode(actionNode, "iscounteraction"), "false"));
						newAnswer.actions.Add (qa);
					}
					newPhrase.answers.Add (newAnswer.id, newAnswer);
				}
				newNpc.phrases.Add(newPhrase.id, newPhrase);
			}
			npcs.Add(newNpc.id, newNpc);
		}
	}

	private void ReadStartSetFromNode (XmlNode node) {
		if (node == null) return;
		startParameters.sectorID = uint.Parse(SafeGetNodeValue(FindFirstChildNode(node, "sect_id"), "0"));
		startParameters.locationID = uint.Parse(SafeGetNodeValue(FindFirstChildNode(node, "loc_id"), "0"));
	}

	private void ReadEndingsFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "endings") continue;
			Ending newEnding = new Ending();
			newEnding.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);

			ReadLangStringFromNode (childNode, new LangStringPair(newEnding.name, NAMENODE_NAME),
				new LangStringPair(newEnding.description, DESCNODE_NAME));

			foreach (XmlNode condChildNode in childNode.ChildNodes) {
				if (condChildNode.LocalName != "condition") continue;
				EndingCondition newCondition = new EndingCondition();
				newCondition.cState = bool.Parse(SafeGetNodeValue(FindFirstChildNode(node, "type"), "false"));
				newCondition.cType = (ConditionType)int.Parse(SafeGetNodeValue(FindFirstChildNode(node, "state"), "0"));
				newCondition.cValue = SafeGetNodeValue(FindFirstChildNode(node, "value"), "0");
				newEnding.conditions.Add (newCondition);
			}

			endings.Add(newEnding.id, newEnding);
		}
	}

	private void ReadShipsFromNode (XmlNode node) {
		if (node == null) return;
		foreach (XmlNode childNode in node.ChildNodes) {
			if (childNode.LocalName != "ships") continue;
			Ship newShip = new Ship();
			newShip.id = uint.Parse(childNode.Attributes[IDATTR_NAME].Value);

			ReadLangStringFromNode (childNode, new LangStringPair(newShip.name, NAMENODE_NAME),
				new LangStringPair(newShip.description, DESCNODE_NAME));

			newShip.backOffset.x = int.Parse(SafeGetNodeValue(FindFirstChildNode(node, "backoffs_x"), "0"));
			newShip.backOffset.y = int.Parse(SafeGetNodeValue(FindFirstChildNode(node, "backoffs_y"), "0"));
			newShip.inwardOffset.x = int.Parse(SafeGetNodeValue(FindFirstChildNode(node, "inwoffs_x"), "20"));
			newShip.inwardOffset.y = int.Parse(SafeGetNodeValue(FindFirstChildNode(node, "inwoffs_y"), "20"));
			newShip.textureId = uint.Parse(SafeGetNodeValue(FindFirstChildNode(node, "tex_id"), "0"));

			XmlNode gridNode = FindFirstChildNode(node, "grid");
			List<ShipCell> cellLine = null; 
			foreach (XmlNode rowNode in gridNode.ChildNodes) {
				switch (rowNode.LocalName) {
				case "row_L0":{
					cellLine = new List<ShipCell>();
					List<uint> valuesList = StringToUintList(SafeGetNodeValue(rowNode, ""));
					foreach (uint value in valuesList){
						ShipCell shipCell = new ShipCell();
						shipCell.CellType = (ShipCellType) value;
						cellLine.Add(shipCell);
					}
					} break;
				case "row_L1":{
					if (cellLine == null) {
						Debug.Log ("Row L1: cellLine is NULL");
						break;
					}
					List<uint> valuesList = StringToUintList(SafeGetNodeValue(rowNode, ""));
					for (int valueInd = 0; valueInd < valuesList.Count; valueInd++) {
						cellLine[valueInd].InterType = (ShipInterierType) valueInd;
						cellLine[valueInd].CrewState = CrewState.CREW_STATE_NONE;
					}
					newShip.grid.Add(cellLine);
					cellLine = null;
					} break;
				}
			}

			ships.Add(newShip.id, newShip);
		}
	}

	//--------------------------------------------------------------------------------

	public void Clear() {
		languages.Clear ();
		sectors.Clear ();
		locations.Clear ();
		climates.Clear ();
		fractions.Clear ();
		backgrounds.Clear ();
		items.Clear ();
		races.Clear ();
		quests.Clear ();
		questGroups.Clear ();
		npcs.Clear ();
		ships.Clear ();
		achivements.Clear ();
		crewMembers.Clear ();
		endings.Clear ();
	}

	public void LoadFromXML(string filepath) {
		XmlDocument xmlLevelData = new XmlDocument ();
		if (File.Exists (filepath + ".xml")) {
			string xmlData = File.ReadAllText (filepath + ".xml");
			xmlLevelData.LoadXml (xmlData);
		} else {
			TextAsset xmlDataAsset = Resources.Load (filepath) as TextAsset;
			if (xmlDataAsset == null) {
				Debug.Log ("XML load failed: xmlDataAsset = NULL");
				return;
			}
			xmlLevelData.LoadXml (xmlDataAsset.text);
		}

		XmlNode rootNode = null;
		foreach (XmlNode childNode in xmlLevelData.ChildNodes)
			if (childNode.Name == "panthadata") {
				rootNode = childNode;
				break;
			}

		if (rootNode == null) return;

		foreach (XmlNode childNode in rootNode.ChildNodes) {
			switch (childNode.LocalName) {
			case "languages" : ReadLanguagesFromNode(childNode); break;
			case "textures" : ReadTexturesFromNode(childNode); break;
			case "questgroups" : ReadQuestGroupsFromNode(childNode); break;
			case "fractions" : ReadFractionsFromNode(childNode); break;
			case "races" : ReadRacesFromNode(childNode); break;
			case "crewmembers" : ReadCrewMembersFromNode(childNode); break;
			case "climates" : ReadClimatesFromNode(childNode); break;
			case "achivements" : ReadAchivementsFromNode(childNode); break;
			case "items" : ReadItemsFromNode(childNode); break;
			case "quests" : ReadQuestsFromNode(childNode); break;
			case "locations" : ReadLocationsFromNode(childNode); break;
			case "sectors" : ReadSectorsFromNode(childNode); break;
			case "npcs" : ReadNpcsFromNode(childNode); break;
			case "startset" : ReadStartSetFromNode(childNode); break;
			case "endings" : ReadEndingsFromNode(childNode); break;
			case "ships" : ReadShipsFromNode(childNode); break;
			}
		}
		Debug.Log("XML loaded");
	}
}
