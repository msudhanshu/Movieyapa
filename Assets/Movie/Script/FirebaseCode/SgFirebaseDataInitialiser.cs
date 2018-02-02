using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Linq;

public class SgFirebaseDataInitialiser : IBaseDataInitialiser
{
    SgFirebaseDatabase db;
    public int userGoldResource { get; set; }
    public UserCurrency userResources { get; set; }
    public List<QSolvedItemModel> userSolvedQuestions { get; set; }

    public List<QLevelItemModel> questionsWithLevel { get; set; }
    public List<QLevelItemModel> unSolvedQuestionsWithLevel { get; set; }


    public List<QEffectItemModel> qEffectLevel { get; set; }
    public List<PackageModel> packages { get; set; }
    public List<UserPackage> userPackages { get; set; }


    /*
	 	public List<QSolvedItemModel> userSolvedQuestions = new List<QSolvedItemModel>();

	public List<QLevelItemModel> questionsWithLevel = new List<QLevelItemModel>();
	public List<QLevelItemModel> unSolvedQuestionsWithLevel = new  List<QLevelItemModel> ();

	public List<QEffectItemModel> qEffectLevel = new List<QEffectItemModel>();
	public List<PackageModel> packages = new List<PackageModel>();
	public List<UserPackage> userPackages = new List<UserPackage>();
	 */


    // Initialize the Firebase database:
    public Task InitializeFirebase()
    {
        List<Task> tasks = new List<Task>();
        Task returnTask;
        db = SgFirebase.GetInstance().database;

        tasks.Add(InitPackages());
        tasks.Add(InitUserPackages());
        tasks.Add(InitGoldResources());
        tasks.Add(InitResources());
        tasks.Add(InitEffectLevels());
        tasks.Add(InitQuestionLevels());
        tasks.Add(InitUserSolvedQuestions());
        Debug.Log("gold before = " + userGoldResource);

        //IMPORTANT : The advantage of this over Task.WhenAll().ContinueWith() is that it will work on .Net 4.0 too. 
        //		returnTask = Task.Factory.ContinueWhenAll (
        //			new[] { InitGoldResources(), InitResources() },
        //			t => {
        //				Debug.Log ("gold after = " + userGoldResource);
        //				userGoldResource = 50;
        //			}
        //		);

        returnTask = Task.WhenAll(tasks).ContinueWith(t =>
        {
            InitUnsolvedQuestionList();
            Debug.Log("gold after = " + userGoldResource);
            //userGoldResource = 50;
        });

        Debug.Log("gold before after = " + userGoldResource);
        //userGoldResource = 25;

        return returnTask;
    }

    public Task InitGoldResources()
    {
        return db.GetGoldResourceRef().GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                DebugError(task.Exception.ToString());
            }
            else if (task.IsCompleted)
            {
                DataSnapshot d = task.Result;
                userGoldResource = Convert.ToInt32(d.GetValue(false));
                Debug.Log("gold in callback= " + userGoldResource);
            }
        });
    }

    public Task InitResources()
    {
        return db.GetResourceRef().GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                DebugError(task.Exception.ToString());
            }
            else if (task.IsCompleted)
            {
                DataSnapshot d = task.Result;
                UserCurrency resmodel = JsonUtility.FromJson<UserCurrency>(d.GetRawJsonValue());
                userResources = resmodel;
                //SgResourceManager.GetInstance().Load(resmodel);
            }
        });
    }

    private Task InitEffectLevels()
    {
        return db.GetEffectLevelRef().GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                DebugError(task.Exception.ToString());
            }
            else if (task.IsCompleted)
            {
                DataSnapshot d = task.Result;
                var resdic = d.Value as Dictionary<string, object>;
                foreach (var item in resdic)
                {
                    Debug.Log(item.Key + ", v=" + item.Value); // Kdq6...
                    QEffectItemModel qe = JsonUtility.FromJson<QEffectItemModel>(JsonUtility.ToJson(item.Value));
                    qe._id = item.Key;
                    //QEffectItemModel qe = new QEffectItemModel(item.Key,item.Value.ToString());
                    qEffectLevel.Add(qe);
                }
            }
        });
    }


    ////// QUETSTION //////////
    /// solved unsolved Question with level init //////
    /*
	private Task InitQuestionLevels() {
		return db.GetQuestionLevelRef().GetValueAsync().ContinueWith( task => {
			if (task.IsFaulted) {
				// Handle the error...
				DebugError(task.Exception.ToString());
			} else if (task.IsCompleted) {
				questionsWithLevel= new List<QLevelItemModel>();
				DataSnapshot d =  task.Result;
				var resdic = d.Value as Dictionary<string, object>;
				//List<QLevelItemModel> tquestionsWithLevel = new List<QLevelItemModel>();
				if(resdic != null ) {
					foreach (var item in resdic)
					{
						Debug.Log(item.Key+", v="+item.Value); // Kdq6...
						//QLevelItemModel res = JsonUtility.FromJson<QLevelItemModel> ( JsonUtility.ToJson(item.Value) );
						//res.id = (int)Convert.ChangeType(item.Key,typeof(int));
						QLevelItemModel res = new QLevelItemModel(item.Key,item.Value.ToString());
						questionsWithLevel.Add(res);
					}
				} else {
					var resarr = d.Value as List<object>;
					var count = resarr.Count;
					for(int i =0 ; i < count ; i++ ) {
					//foreach ( var item in resarr) {
						var item = resarr[i];
						if(item != null) {
							//QLevelItemModel res = JsonUtility.FromJson<QLevelItemModel> ( JsonUtility.ToJson(item) );
							//res.id = (int)Convert.ChangeType(""+i,typeof(int));
							QLevelItemModel res = new QLevelItemModel(i,Convert.ToInt32(item));
							questionsWithLevel.Add(res);
						}
					}
				}
			}
		});
	}
*/

    private Task InitQuestionLevels()
    {
        return db.GetQuestionLevelRef().GetValueAsync().ContinueWith(task =>
        {
            questionsWithLevel = new List<QLevelItemModel>();
            List<QLevelItemModel> l = GetFromArrayOfKeyValue<QLevelItemModel, int, int>(task);
            questionsWithLevel.AddRange(l);
        });
    }

    public Task InitUserSolvedQuestions()
    {
        return db.GetUserSolvedRef().GetValueAsync().ContinueWith(task =>
        {
            userSolvedQuestions = new List<QSolvedItemModel>();
            userSolvedQuestions.AddRange(GetFromArrayOfKeyValue<QSolvedItemModel, int, int>(task));
        });
    }

    private void InitUnsolvedQuestionList()
    {
        unSolvedQuestionsWithLevel = questionsWithLevel.FindAll(qLevelItemModel => !(userSolvedQuestions.Any(qSolvedItemModel => (qSolvedItemModel.id == qLevelItemModel.id))));
        unSolvedQuestionsWithLevel.Sort();
    }


    /// <summary>
    /// PACKAGES 
    /// </summary>
    /// <returns>The packages.</returns>
    private Task InitPackages()
    {
        return db.GetPackages().GetValueAsync().ContinueWith(task =>
        {
            packages = new List<PackageModel>();
            packages.AddRange(GetFromArrayOfObject<PackageModel, string>(task));
        });
    }

    private Task InitUserPackages()
    {
        return db.GetUserPackages().GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                DebugError(task.Exception.ToString());
            }
            else if (task.IsCompleted)
            {
                DataSnapshot d = task.Result;
                var resdic = d.Value as Dictionary<string, object>;
                foreach (var item in resdic)
                {
                    //Debug.Log(item.Key+", v="+item.Value); // Kdq6...
                    UserPackage qe = JsonUtility.FromJson<UserPackage>(JsonUtility.ToJson(item.Value));
                    //					qe.name = item.Key;
                    //QEffectItemModel qe = new QEffectItemModel(item.Key,item.Value.ToString());
                    userPackages.Add(qe);
                }
            }
        });
    }

    protected void DebugLog(string message)
    {
        Debug.Log("Firebase db initialisation: " + message);
    }

    protected void DebugError(string message)
    {
        Debug.LogError("Firebase db initialisation Failed: " + message);
    }

    private string GetUserId()
    {
        return SgFirebase.GetInstance().auth.userId;
    }

    public int GetResourceValue(string res)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        //		var task = reference.Child ("Users").Child (GetUserId ()).Child ("resources").GetValueAsync (res);
        //		task.Wait ();
        //		task.Result.Value;
        return 0;
    }

    #region Generic_DbObject_deserialize
    public interface IFireBaseModel<TID>
    {
        TID itemId { get; set; }
    }


    //https://thaiunitydev.weebly.com/36103607358836233634361736263635362736193633361036093633358536143633360236093634364835853617/-firebase-unity-3d
    // if in firebase db keys are continues int value then it is taken as array, not dict.
    private List<R> GetFromArrayOfObject<R, TIDType>(Task<DataSnapshot> task) where R : IFireBaseModel<TIDType>
    {
        List<R> parsedItemList = new List<R>();
        if (task.IsFaulted)
        {
            // Handle the error...
            DebugError(task.Exception.ToString());
        }
        else if (task.IsCompleted)
        {
            DataSnapshot d = task.Result;
            foreach (DataSnapshot c in d.Children)
            {
                string j = c.GetRawJsonValue();
                R i = JsonUtility.FromJson<R>(j);
                parsedItemList.Add(i);
                //qe.itemId = (TIDType)Convert.ChangeType(item.Key, typeof(TIDType));
                //PackageModel ttss2t = JsonUtility.FromJson<PackageModel>(JsonUtility.ToJson(c.Value));
            }

            return parsedItemList;





            /////////////////////
            List<PackageModel> ttt = JsonUtility.FromJson<List<PackageModel>>(d.GetRawJsonValue());
            string json = d.GetRawJsonValue();
            PackageModels tt2t = JsonUtility.FromJson<PackageModels>(json);

            string json2 = "[" + json + "]";
            //  PackageModels ttw2t = JsonUtility.FromJson<PackageModels>(json2);

            var resdic = d.Value as Dictionary<string, string>;
            if (resdic != null)
            {
                foreach (var item in resdic)
                {
                    DebugLog(item.Key + ",json obj v=" + item.Value); // Kdq6...
                    R qe = JsonUtility.FromJson<R>(JsonUtility.ToJson(item.Value));

                    String a = JsonUtility.ToJson(item.Value);

                    String gd = JsonUtility.ToJson(item.ToString());
                    String bd = JsonUtility.ToJson(item.Value.ToString());

                    String b = JsonUtility.ToJson(item);

                    String gd2 = JsonUtility.ToJson(item.ToString());

                    qe = JsonUtility.FromJson<R>(item.Value.ToString());
                    //if (typeof(TIDType) == typeof(string)) {
                    qe.itemId = (TIDType)Convert.ChangeType(item.Key, typeof(TIDType));
                    //} else {

                    //}
                    //qe.id = item.Key;
                    parsedItemList.Add(qe);
                }
            }
        }
        return parsedItemList;
    }

    private List<R> GetFromArrayOfKeyValue<R, TKey, TValue>(Task<DataSnapshot> task) where R : new()
    {
        List<R> parsedItemList = new List<R>();
        if (task.IsFaulted)
        {
            // Handle the error...
            DebugError(task.Exception.ToString());
        }
        else if (task.IsCompleted)
        {
            DataSnapshot d = task.Result;
            var resdic = d.Value as Dictionary<string, object>;
            if (resdic != null)
            {  // if in firebase db keys are continues int value then it is taken as array, not dict.
                foreach (var item in resdic)
                {
                    //DebugLog(item.Key+", v="+item.Value);
                    var reskey = (TKey)Convert.ChangeType(item.Key, typeof(TKey));
                    var resvalue = (TValue)Convert.ChangeType(item.Value, typeof(TValue));
                    R res = (R)Activator.CreateInstance(typeof(R), new object[] { reskey, resvalue });
                    parsedItemList.Add(res);
                }
            }
            else
            {
                var resarr = d.Value as List<object>;
                var count = resarr.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = resarr[i];
                    if (item != null)
                    {
                        var reskey = (TKey)Convert.ChangeType(i, typeof(TKey));
                        var resvalue = (TValue)Convert.ChangeType(item, typeof(TValue));
                        //Debug.Log("k= "+reskey+", v= "+resvalue);
                        R res = (R)Activator.CreateInstance(typeof(R), new object[] { reskey, resvalue });
                        parsedItemList.Add(res);
                    }
                }
            }
        }
        return parsedItemList;
    }
    #endregion
}
