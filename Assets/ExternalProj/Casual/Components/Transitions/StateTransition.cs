using UnityEngine;
using System.Collections.Generic;

/**
 * Generic class for an activity which is something
 * done over time that can be sped up with gold.
 */ 
[System.Serializable]
public class StateTransition
{
	/**
	 * Type of activity.
	 */
	public string Type {
		get; set;
	}

	public AssetState State {
		get; set;
	}
	
	/**
	 * Time activity commenced.
	 */ 
	public long StartTime {
		get; set;
	}

	/**
	 * Time activity will finish.
	 */ 
	public long EndTime {
		get; set;
	}

	/**
	 * Duration of activity in seconds.
	 */ 
	public int DurationInSeconds {
		get; set;
	}

	/**
	 * Supporting id, the first supporting id or null if none.
	 */
	[System.Xml.Serialization.XmlIgnore]
	public string SupportingId {
		get {
			if (SupportingIds != null && SupportingIds.Count > 0) return SupportingIds[0];
			return null;
		}
	}

	/**
	 * Supporting ids
	 */
	public List<string> SupportingIds {
		get; set;
	}
	
	/**
	 * Implementation that checks time based on activity type.
	 */ 
	[System.Xml.Serialization.XmlIgnore]
	public float PercentageComplete {
		get {
			float elapsedSeconds = (int)(Utility.GetServerTime() - StartTime);
			//Debug.LogError("Now " + Utility.GetServerTime() + " start " + StartTime);
			float percentageComplete = elapsedSeconds / (float)DurationInSeconds;
			if (percentageComplete > 1.0f) percentageComplete = 1.0f;
			return percentageComplete;
		}
	}

	/**
	 * Time left before this activity completes. 
	 */
	[System.Xml.Serialization.XmlIgnore]
	public double RemainingTime {
		get {
			double span = EndTime - Utility.GetServerTime();
			if (span < 0) return 0;
			return span;
		}
	}

	/**
	 * Is this an auto activity?
	 */ 
	public bool IsAutoActivity {
		get {
			try {
				return (State.HasAutoActivity());
			} catch (System.NullReferenceException ex) {
				return false;
			}
		}
	}

	public StateTransition () {
		
	}

	public StateTransition(AssetState state, long startTime) {
		State = state;
		DurationInSeconds = State.activityDuration;
		StartTime = startTime;
		EndTime = startTime + DurationInSeconds;
	}
	
	/**
	 * Create a new activity and populate with the supplied values.
	 */ 
	public StateTransition(string type, int durationInSeconds, System.DateTime startTime, string supportingId) {
		Type = type;
		DurationInSeconds = durationInSeconds;
		StartTime = Utility.ToUnixTime(startTime);
		EndTime = StartTime + DurationInSeconds;
		SupportingIds = new List<string>();
		SupportingIds.Add(supportingId);
	}

	/**
	 * Create a new activity and populate with the supplied values.
	 */ 
	public StateTransition(string type, int durationInSeconds, System.DateTime startTime, List<string> supportingIds) {
		Type = type;
		DurationInSeconds = durationInSeconds;
		StartTime = Utility.ToUnixTime(startTime);
		EndTime = StartTime + DurationInSeconds;
		SupportingIds = new List<string>();
		SupportingIds.AddRange(supportingIds);
	}
}


public class StateTransitionType {
	public const string NONE = "NONE";
	public const string CONSTRUCT = "construct";
	public const string GATHER = "GATHER";
	public const string AUTOMATIC = "AUTOMATIC";
	public const string CLEAR = "CLEAR";
	public const string RECRUIT = "RECRUIT";
	public const string ATTACK = "ATTACK";
}
