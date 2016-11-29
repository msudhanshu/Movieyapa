/**
 * Modified MIT License
 * 
 * Copyright 2015 OneSignal
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * 1. The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * 2. All copies of substantial portions of the Software may only be used in connection
 * with services provided by OneSignal.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;


public class UnityAnalyticsController : MonoBehaviour {
    public bool DebugMode = false;

    void Start() {
        if(DebugMode) {
            TestAnalytics();
            TestUserData();
        }
    }
        
    public void TestAnalytics()
    {
        Analytics.CustomEvent("gameStarted", new Dictionary<string, object>
            {
                { "userId", Config.USER_ID },
                { "userLevel", "no level" }
          });
        
    }


    //FIRST ADD GOOGLE PLAY PUBLIC KEY IN DASHBOARD
    //https://analytics.cloud.unity3d.com/integration/f673dc58-60dd-416e-820a-daf180657d45?unity_version=
    public void TestMonatization()
    {
        // // Use this call for each and every place that a player triggers a monetization event
//        Analytics.Transaction(string productId, decimal price,
//            string currency, string receipt,
//            string signature);
        
        Analytics.Transaction("12345abcde", 0.99m, "USD", null, null);
    }



  

    public void TestUserData() {
//        // Use this call to designate the user gender
//        Analytics.SetUserGender(Gender gender);
//
//        // Use this call to designate the user birth year
//        Analytics.SetUserBirthYear(int birthYear);

        Gender gender = Gender.Female;
        Analytics.SetUserGender(gender);

        int birthYear = 2014;
        Analytics.SetUserBirthYear(birthYear);
    }
   
  
}
