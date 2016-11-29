using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using KiwiCommonDatabase;

[System.Serializable]
public class MarketDataWrapper {


	public List<LevelSceneData> levelSceneData;

	public int version;


	public static bool IsDbTable(FieldInfo field) {
		foreach (object attribute in field.GetCustomAttributes(true))
		{
			if (attribute is DbTableAttribute) return true;
		}
		return false;
	}

	public void InsertIntoDatabase() {
		DatabaseManager dbManager = DatabaseManager.GetInstance();
		dbManager.InsertMarketTable<LevelSceneData>(levelSceneData);
		dbManager.UpdateMarketVersion (version);
	}

}
