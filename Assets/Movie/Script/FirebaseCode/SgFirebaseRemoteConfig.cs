using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using System;
using System.Threading.Tasks;

public class SgFirebaseRemoteConfig : BaseConfig {
	Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;



	public SgFirebaseRemoteConfig() {
		InitializeFirebaseRemoteConfig();
	}

	// When the app starts, check to make sure that we have
	// the required dependencies to use Firebase, and if not,
	// add them if possible.
//	override public void Initialise() {
//		base.Initialise ();
//		dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
//		if (dependencyStatus != Firebase.DependencyStatus.Available) {
//			Firebase.FirebaseApp.FixDependenciesAsync().ContinueWith(task => {
//				dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
//				if (dependencyStatus == Firebase.DependencyStatus.Available) {
//					InitializeFirebaseRemoteConfig();
//				} else {
//					Debug.LogError(
//						"Could not resolve all Firebase dependencies: " + dependencyStatus);
//				}
//			});
//		} else {
//			InitializeFirebaseRemoteConfig();
//		}
//	}
//

	override public string GetValue (string key) {
		return GetConfigValue(key).StringValue;
	}


	private ConfigValue GetConfigValue (string key) {
		Debug.Log ("reading config value " + key);
		return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue (key);
	}

	override public string GetStringValue (string key) {
		return GetConfigValue(key).StringValue;
	}

	override public long GetLongValue (string key) {
		return GetConfigValue(key).LongValue;
	}

	override public double GetDoubleValue (string key) {
		return GetConfigValue(key).DoubleValue;
	}

	override public bool GetBooleanValue (string key) {
		return GetConfigValue(key).BooleanValue;
	}



	// Initialize remote config, and set the default values.
	void InitializeFirebaseRemoteConfig() {
		SetDebugMode ();
		Dictionary<string, object> defaults = InitDefaultConfigValues ();
		Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
		DebugLog("RemoteConfig configured and ready!");
		//FetchData ();
	}

	private void SetDebugMode() {
		if (SgConfig.DEBUG) {
			ConfigSettings configSetting = new ConfigSettings ();
			configSetting.IsDeveloperMode = true;
			Firebase.RemoteConfig.FirebaseRemoteConfig.Settings = configSetting;
		}
	}

	// Start a fetch request.
	public Task FetchData() {
		DebugLog("Fetching data...");
		// FetchAsync only fetches new data if the current data is older than the provided
		// timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
		// By default the timespan is 12 hours, and for production apps, this is a good
		// number.  For this example though, it's set to a timespan of zero, so that
		// changes in the console will always show up immediately.
		System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(TimeSpan.Zero);
		return fetchTask.ContinueWith(FetchComplete);
	}

	void FetchComplete(Task fetchTask) {
		if (fetchTask.IsCanceled) {
			DebugLog("Fetch canceled.");
		} else if (fetchTask.IsFaulted) {
			DebugLog("Fetch encountered an error.");
		} else if (fetchTask.IsCompleted) {
			DebugLog("Fetch completed successfully!");
		}

		var info = Firebase.RemoteConfig.FirebaseRemoteConfig.Info;
		switch (info.LastFetchStatus) {
		case Firebase.RemoteConfig.LastFetchStatus.Success:
			Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched ();
			DebugLog (String.Format ("Remote data loaded and ready (last fetch time {0}).",
				info.FetchTime));
			TestDisplayAllKeys ();
			break;
		case Firebase.RemoteConfig.LastFetchStatus.Failure:
			switch (info.LastFetchFailureReason) {
			case Firebase.RemoteConfig.FetchFailureReason.Error:
				DebugLog("Fetch failed for unknown reason");
				break;
			case Firebase.RemoteConfig.FetchFailureReason.Throttled:
				DebugLog("Fetch throttled until " + info.ThrottledEndTime);
				break;
			}
			break;
		case Firebase.RemoteConfig.LastFetchStatus.Pending:
			DebugLog("Latest Fetch call still pending.");
			break;
		}
		CheckUrgentConfigChanges();
	}

	private void CheckUrgentConfigChanges() {
		//if force update
		checkForceUpdate();
	}


	private void checkForceUpdate() {

	}

	public void TestDisplayAllKeys() {
		DebugLog("Current Keys:");
		System.Collections.Generic.IEnumerable<string> keys =
			Firebase.RemoteConfig.FirebaseRemoteConfig.Keys;
		foreach (string key in keys) {
			DebugLog("Key=" + key + ", Value="+Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue (key).StringValue );
		}

//		DebugLog("GetKeysByPrefix(\"config_test_s\"):");
//		keys = Firebase.RemoteConfig.FirebaseRemoteConfig.GetKeysByPrefix(PACKAGE_MINIMUM_LEVEL_LOCK_TAG);
//		foreach (string key in keys) {
//			DebugLog("    " + key);
//		}
	}
}
