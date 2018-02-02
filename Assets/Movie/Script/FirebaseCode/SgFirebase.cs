using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Storage;
using System.Text;
using System.Threading.Tasks;


/**
 *
 Config and API has been abstracted to its base class, so that in future it can be easily replaced with other services
 So MovieGameManager will have its reference, and our game code will refer to that only... being totally unware of firebase existance

Where as database is verify much connected to firebase realtime database... So we will refer that through firebase only. And it will have 
direct utility functions to interact with firebase database data's.


Our game will have first dependency to SgFirebaseManager only. Once this is initialised , we will Initialise MovieGameManager and futher codes...
( we can think of a loading bar till sgfirbasemanager initilises as game won't be able to show anything anyway... )

Order in which element of firebase should be initilised
database
config
auth

api
datainitialiser
storage/cdn

?? can we use Task for this
 * 
 */

public class SgFirebase : Manager<SgFirebase> {
	public static string MyStorageBucket = "gs://unityfirebasetest-5c0bc.appspot.com";


	public SgFirebaseDatabase database;
	public SgFirebaseRemoteConfig config;
	public SgFirebaseUserAuth auth;

	public SgFirebaseRestAPI api;

	public SgFirebaseDataInitialiser dataInitialiser;

	private string firebaseStorageLocation;
	private string fileContents;
	private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

	override public void PopulateDependencies() {}

	override public void StartInit() {
		StartFireBaseInitialisation ();
	}

	// When the app starts, check to make sure that we have
	// the required dependencies to use Firebase, and if not,
	// add them if possible.
	private void StartFireBaseInitialisation() {
		dependencyStatus = FirebaseApp.CheckDependencies ();
		if (dependencyStatus != DependencyStatus.Available) {
			FirebaseApp.FixDependenciesAsync ().ContinueWith (task => {
				dependencyStatus = FirebaseApp.CheckDependencies ();
				if (dependencyStatus == Firebase.DependencyStatus.Available) {
					InitializeFirebase();
				} else {
					Debug.LogError (
						"Could not resolve all Firebase dependencies: " + dependencyStatus);
				}
			});
		} else {
			InitializeFirebase();
		}
	}

	private void Finished() {
		Debug.Log("FireBaseSdk initialisation Finished");
		transform.parent.gameObject.BroadcastMessage("DependencyCompleted", ManagerDependency.FIREBASE);
	}

	// Initialize the Firebase database:
	void InitializeFirebase() {
		List<Task> tasks = new List<Task>();

		//1 database
		database = new SgFirebaseDatabase ();

		//2 config
		config = new SgFirebaseRemoteConfig();
		tasks.Add(config.FetchData());

		//3 auth
		auth = new SgFirebaseUserAuth();
		tasks.Add(auth.InitializeFirebaseAuth());


        Task.WhenAll(tasks).ContinueWith(_ =>         {             dataInitialiser = new SgFirebaseDataInitialiser();             dataInitialiser.InitializeFirebase()             .ContinueWith(t =>             {                 //dataInitialiser.InitializeFirebase ().ContinueWith( t2 => {                 Finished();                 //});             });         });  
        /*
		//4 datainit
		dataInitialiser = new SgFirebaseDataInitialiser ();
		tasks.Add (dataInitialiser.InitializeFirebase ());

	    Task.WhenAll (tasks).ContinueWith (t => {
			//dataInitialiser.InitializeFirebase ().ContinueWith( t2 => {
				Finished();
			//});
		});
        */

//		Debug.Log ("gold 2 before = " + dataInitialiser.userGoldResource);
//		Task returnTask = Task.Factory.ContinueWhenAll (
//			new[] { dataInitialiser.InitializeFirebase() },
//			t => {
//				Debug.Log ("gold 2 after = " + dataInitialiser.userGoldResource);
//				Finished();
//			}
//		);
//		Debug.Log ("gold before 2 after = " + dataInitialiser.userGoldResource);
//
	}
		

//	public IEnumerator DownloadFromFirebaseStorageURL(string cloudURL, System.Action<T> resultCallback) {
//		StorageReference reference = FirebaseStorage.DefaultInstance.GetReferenceFromUrl(cloudURL);
//		reference.GetDownloadUrlAsync().ContinueWith((Task task) => {
//			if (!task.IsFaulted && !task.IsCanceled) {
//				Debug.Log("Download URL: " + task.Result());
//				// ... now download the file via WWW or UnityWebRequest.
//			}
//		});
//
////		var task = reference.GetBytesAsync(1024 * 1024);
////		yield return new WaitUntil(() => task.IsCompleted);
////		if (task.IsFaulted) {
////			Debug.Log(task.Exception.ToString());
////		} else {
////			fileContents = Encoding.UTF8.GetString(task.Result);
////			Debug.Log("Finished downloading...");
////			Debug.Log("Contents=" + fileContents);
////		}
//	}

	IEnumerator DownloadFromFirebaseStorage() {
		StorageReference reference = FirebaseStorage.DefaultInstance.GetReferenceFromUrl(firebaseStorageLocation);
		var task = reference.GetBytesAsync(1024 * 1024);
		yield return new WaitUntil(() => task.IsCompleted);
		if (task.IsFaulted) {
			Debug.Log(task.Exception.ToString());
		} else {
			fileContents = Encoding.UTF8.GetString(task.Result);
			Debug.Log("Finished downloading...");
			Debug.Log("Contents=" + fileContents);
		}
	}
}
