using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SgUnityConfig;
using Firebase.Database;

namespace SgUnity
{
   
	public class FirebaseDbRemoteAction {

		private SgFirebaseDatabase db {
			get {
				return SgFirebase.Instance.database;
			}
		}

		protected void DebugError(string message) {
			Debug.LogError("Firebase db RemoteAction Failed: "+message);
		}

		// callback with parsed value ??
        public void GetQuestionByIdAction(int question_id, ServerNotifier notifier) {
			db.GetQuestionsRef ().Child (""+question_id).GetValueAsync ().ContinueWith (task => {
				if(task.IsFaulted) {
					DebugError(task.Exception.ToString());
					notifier.OnFailure(task.Exception.ToString());
				} else if(task.IsCompleted) {
					DataSnapshot d =  task.Result;
					QuestionModel resmodel = JsonUtility.FromJson<QuestionModel> (d.GetRawJsonValue());
					string response = d.GetRawJsonValue();
					GameResponse gameResponse = new GameResponse(new Action(task.ToString()), response, false);
					notifier.OnSuccess(gameResponse);
				}
			});
        }

		public void UpdateCurrencyAction(Dictionary<ICapitalCurrency, int> diffResources, ServerNotifier notifier){
			foreach( ICapitalCurrency gameCurrency in diffResources.Keys) {
				if (gameCurrency.isGlobalCurrency()) {
					db.GetGameUserDataRef ().Child (gameCurrency.getId ()).SetValueAsync (diffResources [gameCurrency]);
				} else {
					db.GetResourceRef ().Child (gameCurrency.getId ()).SetValueAsync (diffResources [gameCurrency]);
				}
			}
    	}

//        public void AddCurrencyAction(Dictionary<ICapitalCurrency, int> diffResources, Boolean sync = false){
//            if (! ServerConfig.SERVER_ENABLED) return;
//			String url= GetRestAPI(ActionEnum.CURRENCY_ADD) +"user_id="+Config.USER_ID;
//            url += CapitalManager.GetInstance().GetCurrencyString(diffResources);   
//            MakeCallToServer(ActionEnum.CURRENCY_ADD, ServerSyncManager.GetInstance().serverNotifier, url, null, false);
//        }


        public void UpdateUserPackageAction(UserPackage userPackage, Boolean sync = false){
			//db.GetUserPackages().
        }

		public void CarrierQSolved (QSolvedItemModel s, Boolean sync = false){
			db.GetUserSolvedRef().Child(""+s.id).SetValueAsync(s.level);
		}
			
}
}
