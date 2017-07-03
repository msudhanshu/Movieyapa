using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Storage;
using System.Text;
using System.Threading.Tasks;

public class SgFirebase : Manager<SgFirebase> {
	public static string MyStorageBucket = "gs://unityfirebasetest-5c0bc.appspot.com";
	private string firebaseStorageLocation;
	private string fileContents;
	private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

	override public void PopulateDependencies() {}

	override public void StartInit() {
		InitFirebase ();
	}

	// When the app starts, check to make sure that we have
	// the required dependencies to use Firebase, and if not,
	// add them if possible.
	private void InitFirebase() {
		dependencyStatus = FirebaseApp.CheckDependencies ();
		if (dependencyStatus != DependencyStatus.Available) {
			FirebaseApp.FixDependenciesAsync ().ContinueWith (task => {
				dependencyStatus = FirebaseApp.CheckDependencies ();
				if (dependencyStatus != DependencyStatus.Available) {
					Debug.LogError (
						"Could not resolve all Firebase dependencies: " + dependencyStatus);
				}
			});
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
