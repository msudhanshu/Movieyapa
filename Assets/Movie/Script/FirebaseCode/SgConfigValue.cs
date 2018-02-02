using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SgConfigValue
{

    public static bool FirebaseEnabled = true;

    public static string PACKAGE_MINIMUM_LEVEL_LOCK_TAG = "package_min_level_lock";
    public static int PACKAGE_MINIMUM_LEVEL_LOCK_VALUE = 15;

    public static string CARRIER_QCOUNT_IN_ALEVEL_TAG = "carrier_qcount_in_alevel";

    public static string USER_ID_KEY = "user_id_pref_key";

    public static Dictionary<string, object> configMap = new Dictionary<string, object>
    {
        {PACKAGE_MINIMUM_LEVEL_LOCK_TAG, PACKAGE_MINIMUM_LEVEL_LOCK_VALUE},
        {CARRIER_QCOUNT_IN_ALEVEL_TAG, 5}
    };
}
