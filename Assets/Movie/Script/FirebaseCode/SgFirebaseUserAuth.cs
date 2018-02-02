using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;


//TODO FIXME : DATABASE WITH PROPER RULE DOESN'T WORK UNTIL AUTH FINISHES PROPERLY.
// BUT AUTH WORKES AS CALLBACK... SO CONVERT THIS TO MANAGER

public class SgFirebaseUserAuth
{
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    private string email = "";
    private string password = "";
    private string displayName = "";
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    public string userId;

    //	public delegate void DiffFinishedAction();
    //	public static event DiffFinishedAction OnDiffFinished;
    //
    //
    //	override public void StartInit() {
    //		StartFireBaseInitialisation ();
    //	}
    //
    //	override public void PopulateDependencies() {
    //		//dependencies = new List<ManagerDependency>();
    //		//dependencies.Add(ManagerDependency.DATABASE_INITIALIZED);
    //	}
    //
    //	// When the app starts, check to make sure that we have
    //	// the required dependencies to use Firebase, and if not,
    //	// add them if possible.
    //	void StartFireBaseInitialisation() {
    //		DebugLog("pre - Setting dependency for Firebase Auth");
    //		dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
    //		if (dependencyStatus != Firebase.DependencyStatus.Available) {
    //			Firebase.FirebaseApp.FixDependenciesAsync().ContinueWith(task => {
    //				dependencyStatus = Firebase.FirebaseApp.CheckDependencies();
    //				if (dependencyStatus == Firebase.DependencyStatus.Available) {
    //					InitializeFirebaseAuth();
    //				} else {
    //					Debug.LogError(
    //						"Could not resolve all Firebase dependencies: " + dependencyStatus);
    //				}
    //			});
    //		} else {
    //			InitializeFirebaseAuth();
    //		}
    //	}
    //
    //	private void Finished() {
    //		transform.parent.gameObject.BroadcastMessage("DependencyCompleted", ManagerDependency.AUTH_DONE);
    //		Debug.Log("Diff Finished");
    //		if(OnDiffFinished != null)
    //			OnDiffFinished();
    //	}

    //TODO FIXME : DATABASE WITH PROPER RULE DOESN'T WORK UNTIL AUTH FINISHES PROPERLY.
    // BUT AUTH WORKES AS CALLBACK... SO CONVERT THIS TO MANAGER

    // Initialize auth , and set the default values.
    public Task InitializeFirebaseAuth()
    {
        DebugLog("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;

        string lUserId = PlayerPrefs.GetString(SgConfigValue.USER_ID_KEY);
        if (lUserId == null)
        {
            if (auth.CurrentUser == null)
            {
                return SigninAnonymously();
            }
            else
            {
                Debug.Log("error fireaebase auth : setting authstate directly?");
                AuthStateChanged(this, null);
                updateUserDetail();
            }
        }
        else
        {
            SetUserId(lUserId);
        }
        return Task.Run(() =>
        {
            Debug.Log("Init auth userid = " + userId);
        });

    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                DebugLog("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            SetUserId(user.UserId);
            if (signedIn)
            {
                DebugLog("Signed in " + user.UserId);
                displayName = user.DisplayName ?? "";
                TestDisplayUserInfo(user, 1);
                DebugLog("  Anonymous: " + user.IsAnonymous);
                DebugLog("  Email Verified: " + user.IsEmailVerified);
                var providerDataList = new List<Firebase.Auth.IUserInfo>(user.ProviderData);
                if (providerDataList.Count > 0)
                {
                    DebugLog("  Provider Data:");
                    foreach (var providerData in user.ProviderData)
                    {
                        TestDisplayUserInfo(providerData, 2);
                    }
                }
            }
        }
    }

    private void SetUserId(string lUserId)
    {
        userId = lUserId;
        PlayerPrefs.SetString(SgConfigValue.USER_ID_KEY, userId);
        Debug.Log("Setting user id = " + userId);
    }

    // Attempt to sign in anonymously.
    public Task SigninAnonymously()
    {
        DebugLog("Attempting to sign anonymously...");
        return auth.SignInAnonymouslyAsync().ContinueWith(HandleSigninResult);
    }

    void HandleSigninResult(Task<Firebase.Auth.FirebaseUser> authTask)
    {
        LogTaskCompletion(authTask, "Sign-in");
    }

    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    bool LogTaskCompletion(Task<Firebase.Auth.FirebaseUser> task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            DebugLog(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            DebugLog(operation + " encounted an error.");
            DebugLog(task.Exception.ToString());
        }
        else if (task.IsCompleted)
        {
            DebugLog(operation + " completed");
            user = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                user.DisplayName, user.UserId);
            complete = true;
            updateUserDetail();
            //Finished ();
        }
        return complete;
    }

    // Initialize the Firebase database:
    void updateUserDetail()
    {
        //		FirebaseApp app = FirebaseApp.DefaultInstance;
        //
        //		app.SetEditorDatabaseUrl("https://unityfirebasetest-5c0bc.firebaseio.com/");

        /*
		 //java.lang.NoSuchMethodError: no non-static method "Lcom/google/firebase/FirebaseOptions$Builde
		if (app.Options.DatabaseUrl != null) {
			app.SetEditorDatabaseUrl (app.Options.DatabaseUrl);
		}
		*/

        DebugLog("User Detail is being updated in database");

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("Users").Child(user.UserId).Child("username").SetValueAsync(user.DisplayName);
        reference.Child("Users").Child("userid").Child("name").SetValueAsync("cfssfddesktopsdf");

    }

    //	// Update the user's display name with the currently selected display name.
    //	public void UpdateUserProfile(string newDisplayName = null) {
    //		if (user == null) {
    //			DebugLog("Not signed in, unable to update user profile");
    //			return;
    //		}
    //		displayName = newDisplayName ?? displayName;
    //		DebugLog("Updating user profile");
    //
    //		user.UpdateUserProfileAsync(new Firebase.Auth.UserProfile {
    //			DisplayName = displayName,
    //			PhotoUrl = user.PhotoUrl,
    //		}).ContinueWith(HandleUpdateUserProfile);
    //	}
    //
    //	void HandleUpdateUserProfile(Task authTask) {
    //		
    //		if (LogTaskCompletion(authTask, "User profile")) {
    //			TestDisplayUserInfo(user, 1);
    //		}
    //	}

    protected void DebugLog(string message)
    {
        Debug.Log(message);
    }

    // Display user information.
    void TestDisplayUserInfo(Firebase.Auth.IUserInfo userInfo, int indentLevel)
    {
        string indent = new String(' ', indentLevel * 2);
        var userProperties = new Dictionary<string, string> {
            {"Display Name", userInfo.DisplayName},
            {"Email", userInfo.Email},
            {"Photo URL", userInfo.PhotoUrl != null ? userInfo.PhotoUrl.ToString() : null},
            {"Provider ID", userInfo.ProviderId},
            {"User ID", userInfo.UserId}
        };
        foreach (var property in userProperties)
        {
            if (!String.IsNullOrEmpty(property.Value))
            {
                DebugLog(String.Format("{0}{1}: {2}", indent, property.Key, property.Value));
            }
        }
    }
}
