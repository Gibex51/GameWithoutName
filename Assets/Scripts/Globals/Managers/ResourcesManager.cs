using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using System.IO;

public abstract class StateObject {
	public string rootNodeName;
	public abstract XElement ToXmlNode ();
	public abstract void FromXmlNode (XElement element);

	public static XElement saveNodeGroup<T>(string name, Dictionary<uint, StateObject> itemsList) where T : StateObject {
		XElement groupElement = new XElement (name);

		foreach (T item in itemsList.Values)
			groupElement.Add(item.ToXmlNode ());
		return groupElement;
	}

	public static void readNodeGroup<T>(XElement groupElement) where T : StateObject {
		foreach (XElement element in groupElement.Nodes()) {
			foreach (XElement subElement in element.Nodes()) {
				if (subElement.Name.LocalName == "id")
					GlobalData.resourcesManager.getById<T>(uint.Parse(subElement.Value)).FromXmlNode (element);
			}
		}
	}
}

public class LocationState : StateObject {
	private Location location;
	public bool isVisible {get;set;}
	public uint firstPhraseNpcId { get; set; }
	public uint firstPhraseId { get; set; }

	public LocationState(Location location) {
		this.location = location;
		isVisible = location.visible;
		firstPhraseNpcId = location.firstPhraseNpcId;
		firstPhraseId = location.firstPhraseId;
		this.rootNodeName = "location";
	}

	public String getSectorDescription() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return location.description[langCode];
	}

	public String getName() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return location.name[langCode];
	}

	public bool isBackgroundRandom() {return location.isBackgroundRandom;}
	public uint getFixedBackgroundId() {return location.fixedBackgroundTextureId;}
	public NPCState getFirstPhraseNpc() {
		return GlobalData.resourcesManager.getById<NPCState> (location.firstPhraseNpcId);
	}

	public uint getShipId() {
		return location.shipId;
	}

	public uint getId() {
		return location.id;
	}

	public override string ToString()  {
		return "LocationState : [ Name : " + getName () + 
			" ID : " + location.id +
			" Visible : " + isVisible + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement ("id", location.id),
			new XElement ("isvisible", isVisible),
			new XElement ("firstphrasenpcid", firstPhraseNpcId),
			new XElement ("firstphraseid", firstPhraseId));
	}

	public override void FromXmlNode (XElement element) {
		foreach (XElement subElement in element.Nodes()) {
			if (subElement.Name.LocalName == "isvisible") 
				isVisible = bool.Parse(subElement.Value);
			if (subElement.Name.LocalName == "firstphrasenpcid") 
				firstPhraseNpcId = uint.Parse(subElement.Value);
			if (subElement.Name.LocalName == "firstphraseid") 
				firstPhraseId = uint.Parse(subElement.Value);
		}
	}
}


public class SectorState : StateObject {
	private Sector sector;
	public bool isVisible {get;set;}

	public SectorState(Sector sector) {
		this.sector = sector;
		isVisible = sector.visible;
		this.rootNodeName = "sector";
	}

	public String getSectorDescription() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return sector.description[langCode];
	}

	public String getName() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return sector.name[langCode];
	}
		
	public Point getCoords() {return sector.coords;}
	public Vector2 getNormalCoords() {return new Vector2(sector.coords.x/10000.0f, 1.0f - sector.coords.y/10000.0f);}
	public uint getBackgroundId() {return sector.backgroundTextureId;}
	public uint getClimateId() {return sector.climateId;}
	public uint getFractionId() {return sector.fractionId;}
	public int getLocationsCount() {return sector.locationsId.Count;}

	public LocationState getLocation(int index) {
		if ((index < 0) || (index >= sector.locationsId.Count))
			return null;
		return GlobalData.resourcesManager.getById<LocationState> (sector.locationsId [index]);
	}

	public uint getId() {
		return sector.id;
	}

	public override string ToString()  {
		return "SectorState : [ Name : " + getName () + 
			" ID : " + sector.id +
			" Visible : " + isVisible + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement ("id", sector.id),
			new XElement ("isvisible", isVisible));
	}

	public override void FromXmlNode (XElement element) {
		foreach (XElement subElement in element.Nodes()) {
			if (subElement.Name.LocalName == "isvisible") 
				isVisible = bool.Parse(subElement.Value);
		}
	}
}

public class AnswerState : StateObject {
	private Answer answer;
	public List<QuestAction> actionsList;
	public bool isActive {get;set;}
	private int counter;

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

	public uint getNextPhraseId() {
		return answer.nextPhraseId;
	}
		
	public String getText() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return answer.text[langCode];
	}

	public bool isAutohide() {
		return answer.autohide;
	}

	public int getCounter() {
		return counter;
	}

	public void decreaseCounter() {
		counter -= 1;
		if (counter < 0)
			counter = 0;
	}

	public override string ToString()  {
		return "AnswerState : [ Text : " + getText () + 
								" ID : " + answer.id +
								" NextID : " + answer.nextPhraseId + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement ("id", answer.id),
			new XElement ("isactive", isActive),
			new XElement ("counter", counter));
	}

	public override void FromXmlNode (XElement element) {
		foreach (XElement subElement in element.Nodes()) {
			if (subElement.Name.LocalName == "isactive")
				isActive = bool.Parse (subElement.Value);
			if (subElement.Name.LocalName == "counter")
				counter = int.Parse (subElement.Value);
		}
	}
}

public class PhraseState : StateObject {
	private Phrase phrase;
	private Dictionary<uint, StateObject> answers = new Dictionary<uint, StateObject> ();

	public PhraseState(Phrase phrase) {
		this.phrase = phrase;
		this.rootNodeName = "phrase";
		foreach (Answer answer in phrase.answers.Values)
			answers.Add (answer.id, new AnswerState (answer));
	}

	public bool isRoot() {
		return phrase.isRoot;
	}

	public String getText() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return phrase.npcText[langCode];
	}

	public List<AnswerState> getAnswers() {
		List<AnswerState> resultList = new List<AnswerState> ();
		foreach (StateObject answer in answers.Values)
			resultList.Add ((AnswerState)answer);
		return resultList;
	}

	public AnswerState getAnswer(uint id) {
		if (answers.ContainsKey (id))
			return (AnswerState)answers[id];
		return null;
	}

	public override string ToString()  {
		return "PhraseState : [ Text : " + getText () + 
								" ID : " + phrase.id +
								" isRoot : " + phrase.isRoot + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement ("id", phrase.id),
			saveNodeGroup<AnswerState>("answers", answers));
	}

	public override void FromXmlNode (XElement element) {
		foreach (XElement subElement in element.Nodes()) {
			if (subElement.Name.LocalName == "answers")
				foreach (XElement answerElement in subElement.Nodes()) {
					foreach (XElement answerProp in answerElement.Nodes()) {
						if (answerProp.Name.LocalName == "id")
							getAnswer(uint.Parse(answerProp.Value)).FromXmlNode (answerElement);
					}
				}
		}
	}
}

public class NPCState : StateObject {
	private NPC npc;
	private Dictionary<uint, StateObject> phrases = new Dictionary<uint, StateObject> ();

	public NPCState(NPC npc) {
		this.npc = npc;
		this.rootNodeName = "npc";
	}

	public String getName() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return npc.name[langCode];
	}

	public uint getFractionId() {return npc.fractionId;}

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
		return "NPCState : [ Name : " + getName () + 
							" ID : " + npc.id +
							" Fraction ID : " + npc.fractionId + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement ("id", npc.id),
			saveNodeGroup<PhraseState>("phrases", phrases));
	}
		
	public override void FromXmlNode (XElement element) {
		foreach (XElement subElement in element.Nodes()) {
			if (subElement.Name.LocalName == "phrases")
				foreach (XElement phraseElement in subElement.Nodes()) {
					foreach (XElement phraseProp in phraseElement.Nodes()) {
						if (phraseProp.Name.LocalName == "id")
							GetPhrase(uint.Parse(phraseProp.Value)).FromXmlNode (phraseElement);
					}
				}
		}
	}
}

public class QuestState : StateObject {
	private Quest quest;
	private int startDay;

	public QuestStatus status { get; set;}

	public QuestState(Quest quest) {
		this.quest = quest;
		this.status = quest.status;
		this.rootNodeName = "quest";
		//quest.questGroupId
		//quest.subQuests
	}

	public String getName() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return quest.name [langCode];
	}

	public String getDescription() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return quest.description[langCode];
	}

	/* Доделать */
	public void startQuest(int currentDay) {
		startDay = currentDay;
	}

	/* Доделать */
	public int getDaysToFail(int currentDay) {
		int daysLeft = currentDay - startDay;
		if (daysLeft > quest.daysToFail)
			return -1;
		else
			return quest.daysToFail - daysLeft;
	}
		
	public override string ToString()  {
		return "QuestState : [ Name : " + getName () + 
							" ID : " + quest.id +
							" Status : " + status + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement ("id", quest.id),
			new XElement ("startDay", startDay));
	}
		
	public override void FromXmlNode (XElement element) {
		foreach (XElement subElement in element.Nodes()) {
			if (subElement.Name.LocalName == "startDay")
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

	public String getName() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return crewMember.name [langCode];
	}

	public String getDescription() {
		string langCode = GlobalData.gameSettingsManager.GetSelectedLanguageCode ();
		return crewMember.description [langCode];
	}

	public override string ToString()  {
		return "CrewMemberState : [ Name : " + getName () + 
			" ID : " + crewMember.id + "]";
	}

	public override XElement ToXmlNode() {
		return new XElement (rootNodeName, 
			new XElement ("id", crewMember.id));
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

	public void resetResources() {
		sectors.Clear ();
		locations.Clear ();
		quests.Clear ();
		npcs.Clear ();
		crewMembers.Clear ();
	}

	public void saveResourcesToFile(string filename) {
		XElement rootElement = new XElement(SAVE_ROOT_ELEMENT_NAME);
		XDocument xmlLevelData = new XDocument (rootElement);

		rootElement.Add(StateObject.saveNodeGroup<SectorState> (SAVE_SECTORS_ELEMENT_NAME, sectors));
		rootElement.Add(StateObject.saveNodeGroup<LocationState> (SAVE_LOCATIONS_ELEMENT_NAME, locations));
		rootElement.Add(StateObject.saveNodeGroup<NPCState> (SAVE_NPCS_ELEMENT_NAME, npcs));
		rootElement.Add(StateObject.saveNodeGroup<QuestState> (SAVE_QUESTS_ELEMENT_NAME, quests));
		rootElement.Add(StateObject.saveNodeGroup<CrewMemberState> (SAVE_CREWS_ELEMENT_NAME, crewMembers));

		Directory.CreateDirectory (Directory.GetParent(filename).FullName);
		xmlLevelData.Save (filename);
	}

	public void loadResourcesFromFile(string filename) {
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
					StateObject.readNodeGroup<SectorState> (groupElement);
					break;
				}
			case SAVE_LOCATIONS_ELEMENT_NAME: {
					StateObject.readNodeGroup<LocationState> (groupElement);
					break;
				}
			case SAVE_NPCS_ELEMENT_NAME: {
					StateObject.readNodeGroup<NPCState> (groupElement);
					break;
				}
			case SAVE_QUESTS_ELEMENT_NAME: {
					StateObject.readNodeGroup<QuestState> (groupElement);
					break;
				}
			case SAVE_CREWS_ELEMENT_NAME: {
					StateObject.readNodeGroup<CrewMemberState> (groupElement);
					break;
				}
			}
		}
	}

	private void loadSectorToCache(uint id) {
		if (sectors.ContainsKey (id) || (!levelData.sectors.ContainsKey(id)))
			return;	
		SectorState sector = new SectorState ((Sector)levelData.sectors[id]);
		sectors.Add (id, sector);
	}

	public List<SectorState> getSectors() {
		List<SectorState> resultList = new List<SectorState> ();
		foreach (Sector sector in levelData.sectors.Values) {
			loadSectorToCache (sector.id);
			resultList.Add ((SectorState)sectors [sector.id]);
		}
		return resultList;
	}

	public T getById<T>(uint id) where T : StateObject {
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


