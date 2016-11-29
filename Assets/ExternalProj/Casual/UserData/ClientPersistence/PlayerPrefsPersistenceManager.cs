using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
#if ENABLE_JSON
using Newtonsoft.Json;
#endif

/**
 * Saves and loads data from PlayerPrefs
 */ 
public class PlayerPrefsPersistenceManager : PersistenceManager {
	
	public string playerPrefName = "SAVED_GAME";
	public bool useJson = true;
	
	protected bool willSave;

	protected System.Xml.Serialization.XmlSerializer serializer;
	
	void LateUpdate() {
		// Only save once per frame at the end of the frame to avoid race conditions
		if (willSave) {
			DoSave();	
			willSave = false;
		}
	}
	
	/**
	 * Used to determine if there is a game that should be loaded.
	 */ 
	override public bool SaveGameExists() {
		if (PlayerPrefs.GetString(GetPrefName(), "").Length > 0) return true;
		return false;
	}
	
	override public void Save() {
		willSave = true;	
	}
	
	protected void DoSaveXML() {
		InitTypes();
		if (serializer == null) serializer = new System.Xml.Serialization.XmlSerializer(typeof(SaveGameData), types);

		SaveGameData dataToSave = GetSaveGameData();
		if (dataToSave != null) {
			try 
			{
        		using (var stream = new StringWriter()) {
					serializer.Serialize(stream, dataToSave);
					PlayerPrefs.SetString(GetPrefName(), stream.ToString());
				}
			}
			catch (System.IO.IOException e)
			{
				Debug.LogError("Error saving buildings:" + e.Message);
			}	
		} else {
			Debug.LogWarning("Nothing to save");	
		}
	}


	protected void DoSaveJSON() {
#if ENABLE_JSON
	SaveGameData dataToSave = GetSaveGameData();

		if (dataToSave != null) {
			try 
			{
				PlayerPrefs.SetString(GetPrefName(), JsonConvert.SerializeObject(dataToSave));

			}
			catch (System.IO.IOException e)
			{
				Debug.LogError("Error saving buildings:" + e.Message);
			}	
		} else {
			Debug.LogWarning("Nothing to save");	
		}
#endif
	}

	virtual protected void DoSave() {
		if (useJson) 
			DoSaveJSON();
		else 
			DoSaveXML();
	}

	public SaveGameData LoadXML() {
		SaveGameData result;
		InitTypes();
		System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SaveGameData), types);
		String savedGame = PlayerPrefs.GetString(GetPrefName(), "");
		if (savedGame.Length < 1) {
			Debug.LogError ("No saved game data present.");	
			return null;
		} 
		using (StringReader stream = new StringReader(savedGame)) {
			result = (SaveGameData)serializer.Deserialize(stream);
		}
		return result;
	}


	public SaveGameData LoadJSON() {
#if ENABLE_JSON
		SaveGameData result;
		String savedGame = PlayerPrefs.GetString(GetPrefName(), "");

		if (savedGame.Length < 1) {
			Debug.LogError ("No saved game data present.");	
			return null;
		} 
		result = JsonConvert.DeserializeObject<SaveGameData>(savedGame);
		return result;
#endif
		return null;
	}

	override public SaveGameData Load() {
		if (useJson) {
			return LoadJSON();
		} else { 
			return LoadXML();
		}
	}

	public string GetPrefName() {
		return useJson ? playerPrefName + "json" : playerPrefName;
	}

	public string GetUserIdPref() {
		return Config.USER_ID_KEY;
	}
}
