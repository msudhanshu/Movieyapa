using UnityEngine;
using System.Collections;

public enum QuestStatus {
	NONINITIALIZED,
	INITIALIZED, // Not All the pre-requisite quests are completed, but few are complete
	ACTIVATED, // All the pre-requisite quests are completed
	COMPLETED,

	/*
	 *  LE Quest will be in pre limited state Until gold is used to gold_activate the quest or
	 * User should wait until user_start_time.
	 */
	PRE_ACTUAL_START, // LE Quest, before Actual Start Time 

	PRE_ACTIVATED, // LE Quest is in between actual_start and user_start time
	//Expired but can be restored by paying gold
	USER_EXPIRED,
	//Quest Actual Expiry Time counter starts based on User Action
	READY_FOR_ACTUAL_EXPIRY,
	// Force Activated using gold or some other resource
	FORCE_ACTIVATED,
	//Expired and cannot be restored
	ACTUAL_EXPIRED,
	//For FB locked quests
	HIBERNATE
}


