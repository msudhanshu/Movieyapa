using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public interface IBaseDataInitialiser {
	int userGoldResource { get; set; }
	UserCurrency userResources { get; set; }
	List<QSolvedItemModel> userSolvedQuestions { get; set; }
	List<QLevelItemModel> questionsWithLevel { get; set; }
	List<QEffectItemModel> qEffectLevel { get; set; }

	//TODO : IT SHOULD NOT FETCH ALL THE PACKEGES IN FUTURE... JUST FEW AHEAD OF USER CURRENT LEVEL... FOR NOW ITS OK
	List<PackageModel> packages { get; set; }
	List<UserPackage> userPackages { get; set; }
}
