using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SgUnityConfig;

public abstract class BaseConfig {

	protected Dictionary<string, object> InitDefaultConfigValues() {
		return SgConfigValue.configMap;
	}

	virtual public void Initialise() {
	
	}

	abstract public string GetValue (string key);
	abstract public string GetStringValue (string key);
	abstract public long GetLongValue (string key);
	abstract public double GetDoubleValue (string key);
	abstract public bool GetBooleanValue (string key);


	protected void DebugLog(string message) {
		Debug.Log(message);
	}
}
