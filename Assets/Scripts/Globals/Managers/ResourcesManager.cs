using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using System.IO;

public abstract class StateObject {
	public string rootNodeName;
	public abstract XElement ToXmlNode ();
	public abstract void FromXmlNode (XElement element);

	protected const string SAVE_ID_ELEMENT_NAME = "id";

	public static XElement SaveNodeGroup<T>(string name, Dictionary<uint, StateObject> itemsList) where T : StateObject {
		XElement groupElement = new XElement (name);

		foreach (T item in itemsList.Values)
			groupElement.Add(item.ToXmlNode ());
		return groupElement;
	}

	public static void ReadNodeGroup<T>(XElement groupElement) where T : StateObject {
		foreach (XElement element in groupElement.Nodes()) {
			foreach (XElement subElement in element.Nodes()) {
				if (subElement.Name.LocalName == SAVE_ID_ELEMENT_NAME)
					GlobalData.resourcesManager.GetById<T>(uint.Parse(subElement.Value)).FromXmlNode (element);
			}
		}
	}
}

public class LocationState : StateObject {
	private Location location;
	public bool isVisible {get;set;}
	public uint firstPhraseNpcId { get; set; }
	public uint firstPhraseId { get; set; }

	private const string SAVE_ISVISIBLE_ELEMENT_NAME = "isvisible";
	private const string SAVE_FIRSTPHRASENPCID_ELEMENT_NAME = "firstphrasenpcid";
	private const string SAVE_FIRSTPHRASEID_ELEMENT_NAME = "firstphraseid";

	public LocationState(Location location) {
		this.location = location;
		isVisible = location.visible;
		firstPhraseNpcId = location.firstPhraseNpcId;
		firstPhraseId = location.firstPhraseId;
		this.rootNodeName = "location";
	}

	public String GetSectorDescription() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return location.description[langCode];
	}

	public String GetName() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return location.name[langCode];
	}

	public bool IsBackgroundRandom() {return location.isBackgroundRandom;}
	public uint GetFixedBackgroundId() {return location.fixedBackgroundTextureId;}

	public NPCState GetFirstPhraseNpc() {
		return GlobalData.resourcesManager.GetById<NPCState> (location.firstPhraseNpcId);
	}

	public uint GetShipId() {
		return location.shipId;
	}

	public uint GetId() {
		return location.id;
	}

	public override string ToString()  {
		return "LocationState : [ Name : " + GetName () + 
			" ID : " + location.id +
			" Visible : " + isVisible + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement (SAVE_ID_ELEMENT_NAME, location.id),
			new XElement (SAVE_ISVISIBLE_ELEMENT_NAME, isVisible),
			new XElement (SAVE_FIRSTPHRASENPCID_ELEMENT_NAME, firstPhraseNpcId),
			new XElement (SAVE_FIRSTPHRASEID_ELEMENT_NAME, firstPhraseId));
	}

	public override void FromXmlNode (XElement element) {
		foreach (XElement subElement in element.Nodes()) {
			if (subElement.Name.LocalName == SAVE_ISVISIBLE_ELEMENT_NAME) 
				isVisible = bool.Parse(subElement.Value);
			if (subElement.Name.LocalName == SAVE_FIRSTPHRASENPCID_ELEMENT_NAME) 
				firstPhraseNpcId = uint.Parse(subElement.Value);
			if (subElement.Name.LocalName == SAVE_FIRSTPHRASEID_ELEMENT_NAME) 
				firstPhraseId = uint.Parse(subElement.Value);
		}
	}
}


public class SectorState : StateObject {
	private Sector sector;
	public bool isVisible {get;set;}

	private const string SAVE_ISVISIBLE_ELEMENT_NAME = "isvisible";

	public SectorState(Sector sector) {
		this.sector = sector;
		isVisible = sector.visible;
		this.rootNodeName = "sector";
	}

	public String GetSectorDescription() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return sector.description[langCode];
	}

	public String GetName() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return sector.name[langCode];
	}
		
	public Point GetCoords() {return sector.coords;}
	public Vector2 GetNormalCoords() {return new Vector2(sector.coords.x/10000.0f, 1.0f - sector.coords.y/10000.0f);}
	public uint GetBackgroundId() {return sector.backgroundTextureId;}
	public uint GetClimateId() {return sector.climateId;}
	public uint GetFractionId() {return sector.fractionId;}
	public int GetLocationsCount() {return sector.locationsId.Count;}

	public LocationState GetLocation(int index) {
		if ((index < 0) || (index >= sector.locationsId.Count))
			return null;
		return GlobalData.resourcesManager.GetById<LocationState> (sector.locationsId [index]);
	}

	public uint GetId() {
		return sector.id;
	}

	public override string ToString()  {
		return "SectorState : [ Name : " + GetName () + 
			" ID : " + sector.id +
			" Visible : " + isVisible + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement (SAVE_ID_ELEMENT_NAME, sector.id),
			new XElement (SAVE_ISVISIBLE_ELEMENT_NAME, isVisible));
	}

	public override void FromXmlNode (XElement element) {
		foreach (XElement subElement in element.Nodes()) {
			if (subElement.Name.LocalName == SAVE_ISVISIBLE_ELEMENT_NAME) 
				isVisible = bool.Parse(subElement.Value);
		}
	}
}

public class AnswerState : StateObject {
	private Answer answer;
	public List<QuestAction> actionsList;
	public bool isActive {get;set;}
	private int counter;

	private const string SAVE_ISACTIVE_ELEMENT_NAME = "isactive";
	private const string SAVE_COUNTER_ELEMENT_NAME = "counter";

	public AnswerState(Answer answer) {
		this.answer = answer;
		this.isActive = answer.active;
		this.counter = answer.counter;
		this.rootNodeName = "answer";
		actionsList = new List<QuestAction> ();
		foreach (QuestAction action in answer.actions) {
			actionsList.Add(new QuestAction(action));
		}
	}

	public uint GetNextPhraseId() {
		return answer.nextPhraseId;
	}
		
	public String GetText() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return answer.text[langCode];
	}

	public bool IsAutohide() {
		return answer.autohide;
	}

	public int GetCounter() {
		return counter;
	}

	public void DecreaseCounter() {
		counter -= 1;
		if (counter < 0)
			counter = 0;
	}

	public override string ToString()  {
		return "AnswerState : [ Text : " + GetText () + 
								" ID : " + answer.id +
								" NextID : " + answer.nextPhraseId + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement (SAVE_ID_ELEMENT_NAME, answer.id),
			new XElement (SAVE_ISACTIVE_ELEMENT_NAME, isActive),
			new XElement (SAVE_COUNTER_ELEMENT_NAME, counter));
	}

	public override void FromXmlNode (XElement element) {
		foreach (XElement subElement in element.Nodes()) {
			if (subElement.Name.LocalName == SAVE_ISACTIVE_ELEMENT_NAME)
				isActive = bool.Parse (subElement.Value);
			if (subElement.Name.LocalName == SAVE_COUNTER_ELEMENT_NAME)
				counter = int.Parse (subElement.Value);
		}
	}
}

public class PhraseState : StateObject {
	private Phrase phrase;
	private Dictionary<uint, StateObject> answers = new Dictionary<uint, StateObject> ();

	private const string SAVE_ANSWERS_ELEMENT_NAME = "answers";

	public PhraseState(Phrase phrase) {
		this.phrase = phrase;
		this.rootNodeName = "phrase";
		foreach (Answer answer in phrase.answers.Values)
			answers.Add (answer.id, new AnswerState (answer));
	}

	public bool IsRoot() {
		return phrase.isRoot;
	}

	public String GetText() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return phrase.npcText[langCode];
	}

	public List<AnswerState> GetAnswers() {
		List<AnswerState> resultList = new List<AnswerState> ();
		foreach (StateObject answer in answers.Values)
			resultList.Add ((AnswerState)answer);
		return resultList;
	}

	public AnswerState GetAnswer(uint id) {
		if (answers.ContainsKey (id))
			return (AnswerState)answers[id];
		return null;
	}

	public override string ToString()  {
		return "PhraseState : [ Text : " + GetText () + 
								" ID : " + phrase.id +
								" isRoot : " + phrase.isRoot + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement (SAVE_ID_ELEMENT_NAME, phrase.id),
			SaveNodeGroup<AnswerState>(SAVE_ANSWERS_ELEMENT_NAME, answers));
	}

	public override void FromXmlNode (XElement element) {
		foreach (XElement subElement in element.Nodes()) {
			if (subElement.Name.LocalName == SAVE_ANSWERS_ELEMENT_NAME)
				foreach (XElement answerElement in subElement.Nodes()) {
					foreach (XElement answerProp in answerElement.Nodes()) {
						if (answerProp.Name.LocalName == SAVE_ID_ELEMENT_NAME)
							GetAnswer(uint.Parse(answerProp.Value)).FromXmlNode (answerElement);
					}
				}
		}
	}
}

public class NPCState : StateObject {
	private NPC npc;
	private Dictionary<uint, StateObject> phrases = new Dictionary<uint, StateObject> ();

	private const string SAVE_PHRASES_ELEMENT_NAME = "phrases";

	public NPCState(NPC npc) {
		this.npc = npc;
		this.rootNodeName = "npc";
	}

	public String GetName() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return npc.name[langCode];
	}

	public uint GetFractionId() {return npc.fractionId;}

	public PhraseState GetPhrase(uint id) {
		if (phrases.ContainsKey (id)) {
			return (PhraseState)phrases[id];
		} else {
			if (!npc.phrases.ContainsKey(id))
				return null;
			PhraseState phrase = new PhraseState (npc.phrases[id]);;
			phrases.Add (id, phrase);
			return phrase;
		}
	}

	public override string ToString()  {
		return "NPCState : [ Name : " + GetName () + 
							" ID : " + npc.id +
							" Fraction ID : " + npc.fractionId + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement (SAVE_ID_ELEMENT_NAME, npc.id),
			SaveNodeGroup<PhraseState>(SAVE_PHRASES_ELEMENT_NAME, phrases));
	}
		
	public override void FromXmlNode (XElement element) {
		foreach (XElement subElement in element.Nodes()) {
			if (subElement.Name.LocalName == SAVE_PHRASES_ELEMENT_NAME)
				foreach (XElement phraseElement in subElement.Nodes()) {
					foreach (XElement phraseProp in phraseElement.Nodes()) {
						if (phraseProp.Name.LocalName == SAVE_ID_ELEMENT_NAME)
							GetPhrase(uint.Parse(phraseProp.Value)).FromXmlNode (phraseElement);
					}
				}
		}
	}
}

public class QuestState : StateObject {
	private Quest quest;
	private int startDay;

	private const string SAVE_STARTDAY_ELEMENT_NAME = "startDay";

	public QuestStatus status { get; set;}

	public QuestState(Quest quest) {
		this.quest = quest;
		this.status = quest.status;
		this.rootNodeName = "quest";
		//quest.questGroupId
		//quest.subQuests
	}

	public String GetName() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return quest.name [langCode];
	}

	public String GetDescription() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return quest.description[langCode];
	}
		
	public void StartQuest(int currentDay) {
		startDay = currentDay;
		Debug.LogWarning ("Make quest start!");
	}
		
	public int GetDaysToFail(int currentDay) {
		int daysLeft = currentDay - startDay;
		if (daysLeft > quest.daysToFail)
			return -1;
		else
			return quest.daysToFail - daysLeft;
	}
		
	public override string ToString()  {
		return "QuestState : [ Name : " + GetName () + 
							" ID : " + quest.id +
							" Status : " + status + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement (SAVE_ID_ELEMENT_NAME, quest.id),
			new XElement (SAVE_STARTDAY_ELEMENT_NAME, startDay));
	}
		
	public override void FromXmlNode (XElement element) {
		foreach (XElement subElement in element.Nodes()) {
			if (subElement.Name.LocalName == SAVE_STARTDAY_ELEMENT_NAME)
				startDay = int.Parse (subElement.Value);
		}
	}
}

public class CrewMemberState : StateObject {
	private CrewMember crewMember;

	public CrewMemberState(CrewMember crewMember) {
		this.crewMember = crewMember;
		this.rootNodeName = "crewmember";
	}

	public String GetName() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return crewMember.name [langCode];
	}

	public String GetDescription() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return crewMember.description [langCode];
	}

	public override string ToString()  {
		return "CrewMemberState : [ Name : " + GetName () + 
			" ID : " + crewMember.id + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement (SAVE_ID_ELEMENT_NAME, crewMember.id));
	}
		
	public override void FromXmlNode (XElement element) {
		// Пока пусто
	}
}
	
public class ResourcesManager
{
	private LevelData levelData = null;
	private const string SAVE_ROOT_ELEMENT_NAME = "savedata";
	private const string SAVE_SECTORS_ELEMENT_NAME = "sectors";
	private const string SAVE_LOCATIONS_ELEMENT_NAME = "locations";
	private const string SAVE_NPCS_ELEMENT_NAME = "npcs";
	private const string SAVE_QUESTS_ELEMENT_NAME = "quests";
	private const string SAVE_CREWS_ELEMENT_NAME = "crewmembers";

	private Dictionary<uint, StateObject> sectors = new Dictionary<uint, StateObject> ();
	private Dictionary<uint, StateObject> locations = new Dictionary<uint, StateObject> ();
	private Dictionary<uint, StateObject> quests = new Dictionary<uint, StateObject> ();
	private Dictionary<uint, StateObject> npcs = new Dictionary<uint, StateObject> ();
	//private Dictionary<uint, Achivement> 	achivements = new Dictionary<uint, Achivement> ();
	private Dictionary<uint, StateObject> crewMembers = new Dictionary<uint, StateObject> ();

	public ResourcesManager(LevelData levelData) {
		this.levelData = levelData;
	}

	public void ResetResources() {
		sectors.Clear ();
		locations.Clear ();
		quests.Clear ();
		npcs.Clear ();
		crewMembers.Clear ();
	}

	public void SaveResourcesToFile(string filename) {
		XElement rootElement = new XElement(SAVE_ROOT_ELEMENT_NAME);
		XDocument xmlLevelData = new XDocument (rootElement);

		rootElement.Add(StateObject.SaveNodeGroup<SectorState> (SAVE_SECTORS_ELEMENT_NAME, sectors));
		rootElement.Add(StateObject.SaveNodeGroup<LocationState> (SAVE_LOCATIONS_ELEMENT_NAME, locations));
		rootElement.Add(StateObject.SaveNodeGroup<NPCState> (SAVE_NPCS_ELEMENT_NAME, npcs));
		rootElement.Add(StateObject.SaveNodeGroup<QuestState> (SAVE_QUESTS_ELEMENT_NAME, quests));
		rootElement.Add(StateObject.SaveNodeGroup<CrewMemberState> (SAVE_CREWS_ELEMENT_NAME, crewMembers));

		Directory.CreateDirectory (Directory.GetParent(filename).FullName);
		xmlLevelData.Save (filename);
	}

	public void LoadResourcesFromFile(string filename) {
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

		foreach (XElement groupElement in rootElement.Nodes()) {
			switch (groupElement.Name.LocalName) {
			case SAVE_SECTORS_ELEMENT_NAME: {
					StateObject.ReadNodeGroup<SectorState> (groupElement);
					break;
				}
			case SAVE_LOCATIONS_ELEMENT_NAME: {
					StateObject.ReadNodeGroup<LocationState> (groupElement);
					break;
				}
			case SAVE_NPCS_ELEMENT_NAME: {
					StateObject.ReadNodeGroup<NPCState> (groupElement);
					break;
				}
			case SAVE_QUESTS_ELEMENT_NAME: {
					StateObject.ReadNodeGroup<QuestState> (groupElement);
					break;
				}
			case SAVE_CREWS_ELEMENT_NAME: {
					StateObject.ReadNodeGroup<CrewMemberState> (groupElement);
					break;
				}
			}
		}
	}

	private void LoadSectorToCache(uint id) {
		if (sectors.ContainsKey (id) || (!levelData.sectors.ContainsKey(id)))
			return;	
		SectorState sector = new SectorState ((Sector)levelData.sectors[id]);
		sectors.Add (id, sector);
	}

	public List<SectorState> GetSectors() {
		List<SectorState> resultList = new List<SectorState> ();
		foreach (Sector sector in levelData.sectors.Values) {
			LoadSectorToCache (sector.id);
			resultList.Add ((SectorState)sectors [sector.id]);
		}
		return resultList;
	}

	public T GetById<T>(uint id) where T : StateObject {
		Dictionary<uint, StateObject> cacheContainer = null;
		Dictionary<uint, BaseClass> levelContainer = null;
		if (typeof(T) == typeof(SectorState)) { 
			cacheContainer = sectors;
			levelContainer = levelData.sectors;
		} else if (typeof(T) == typeof(NPCState)) { 
			cacheContainer = npcs;
			levelContainer = levelData.npcs;
		} else if (typeof(T) == typeof(LocationState)) { 
			cacheContainer = locations;
			levelContainer = levelData.locations;
		} else if (typeof(T) == typeof(QuestState)) { 
			cacheContainer = quests;
			levelContainer = levelData.quests;
		} else if (typeof(T) == typeof(CrewMemberState)) { 
			cacheContainer = crewMembers;
			levelContainer = levelData.crewMembers;
		} else {
			return null;
		}

		if (cacheContainer.ContainsKey (id)) {
			return (T)cacheContainer[id];
		} else {
			if (!levelContainer.ContainsKey(id))
				return null;
			StateObject item = (StateObject) Activator.CreateInstance(typeof(T), new object[] { levelContainer[id] });
			cacheContainer.Add (id, item);
			return (T)item;
		}
	}
}


