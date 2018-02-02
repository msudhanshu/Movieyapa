using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class SgFirebaseDatabase
{
    public static string DATABASE_URL = "https://unityfirebasetest-5c0bc.firebaseio.com/";
    private string email = "";
    private string password = "";
    private string displayName = "";
    public static string MARKET_DATA_ROOT = "zoopmoviequiz";
    public static string USER_DEATAIL = "Users";
    public static string USER_DATA_ROOT = "user_data";
    public static string USER_GAME_DATA = "game_data";
    public SgFirebaseDatabase()
    {
        InitializeFirebase();
    }

    // Initialize the Firebase database:
    void InitializeFirebase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        app.SetEditorDatabaseUrl(DATABASE_URL);
        if (app.Options.DatabaseUrl != null) app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
    }

    protected void DebugLog(string message)
    {
        Debug.Log(message);
    }

    private string GetUserId()
    {
        return SgFirebase.GetInstance().auth.userId;
    }

    private string GetFilmType()
    {
        return FilmIndustryType.GetActiveFilmTypeName();
    }

    //market data
    public DatabaseReference GetEffectLevelRef()
    {
        return GetActiveFilmTypeMarketDataRef().Child("effects");
    }

    public DatabaseReference GetQuestionLevelRef()
    {
        return GetActiveFilmTypeMarketDataRef().Child("questionslevel");
    }

    public DatabaseReference GetQuestionsRef()
    {
        //DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        //return reference.Child("zoopQ/questions");
        return GetActiveFilmTypeMarketDataRef().Child("questions");
    }

    public DatabaseReference GetMarketDataRef()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        return reference.Child(MARKET_DATA_ROOT);
    }

    public DatabaseReference GetFilmTypeMarketDataRef(string filmType)
    {
        if (Utility.StringEmpty(filmType))
        {
            return GetMarketDataRef();
        }
        else
            return GetMarketDataRef().Child(filmType);
    }

    public DatabaseReference GetActiveFilmTypeMarketDataRef()
    {
        return GetFilmTypeMarketDataRef("");
        //return GetFilmTypeMarketDataRef(GetFilmType()); //TEMP HOTFIX
    }



    //USER DATA
    public void SetResourceValue(string res, int value)
    {
        Debug.Log("Currency " + res + " added to database value = " + value);
        GetResourceRef().Child(res).SetValueAsync(value);
    }

    //USER DATA REF
    public DatabaseReference GetGoldResourceRef()
    {
        return GetGameUserDataRef().Child(GetUserId()).Child("gold");
    }

    public DatabaseReference GetResourceRef()
    {
        return GetActiveFilmTypeUserDataRef().Child(GetUserId()).Child("resources");
    }

    public DatabaseReference GetUserSolvedRef()
    {
        return GetActiveFilmTypeUserDataRef().Child(GetUserId()).Child("qsolved");
    }

    public DatabaseReference GetUserDataRef()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        return reference.Child(USER_DATA_ROOT);
    }

    public DatabaseReference GetGameUserDataRef()
    {
        return GetUserDataRef().Child(USER_GAME_DATA);
    }

    public DatabaseReference GetFilmTypeUserDataRef(string filmType)
    {
        if (Utility.StringEmpty(filmType))
        {
            return GetUserDataRef();
        }
        else
            return GetUserDataRef().Child(filmType);
    }

    public DatabaseReference GetActiveFilmTypeUserDataRef()
    {
        return GetFilmTypeUserDataRef("");
        //return GetFilmTypeUserDataRef(GetFilmType()); //TEMP HOTFIX
    }



    //Packages
    public DatabaseReference GetPackages()
    {
        return GetActiveFilmTypeMarketDataRef().Child("packages");
    }

    public DatabaseReference GetUserPackages()
    {
        return GetActiveFilmTypeUserDataRef().Child("userpackages");
    }

    //	public int GetResourceValue(string res) {
    //		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    ////		var task = reference.Child ("Users").Child (GetUserId ()).Child ("resources").GetValueAsync (res);
    ////		task.Wait ();
    ////		task.Result.Value;
    //		return 0;
    //	}

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
