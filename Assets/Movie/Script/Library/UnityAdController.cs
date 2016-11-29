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
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
using System.Collections;
using System.Collections.Generic;


public class UnityAdController : Manager<UnityAdController> {
    public bool DebugMode = false;

    /* Implement this function */
    override public void StartInit() {
        if(DebugMode)
            UnityAdController.GetInstance().ShowAd();
    }

    /* Implement this function */
    override public void PopulateDependencies(){}

    public void ShowAd()
    {
#if UNITY_ADS
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
		#endif
    }

    public void ShowRewardedAd()
    {
#if UNITY_ADS
        if (Advertisement.IsReady("rewardedVideoZone"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideoZone", options);
        }
		#endif
    }

#if UNITY_ADS
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
        case ShowResult.Finished:
            Debug.Log("The ad was successfully shown.");
            //
            // YOUR CODE TO REWARD THE GAMER
            // Give coins etc.
            break;
        case ShowResult.Skipped:
            Debug.Log("The ad was skipped before reaching the end.");
            break;
        case ShowResult.Failed:
            Debug.LogError("The ad failed to be shown.");
            break;
        }
    }
	#endif

    void OnGUI () {
        if( DebugMode ) {
            GUIStyle customTextSize = new GUIStyle("button");
            customTextSize.fontSize = 30;

            GUIStyle guiBoxStyle = new GUIStyle("box");
            guiBoxStyle.fontSize = 30;

            GUI.Box(new Rect(400, 10, 390, 340), "Test Menu", guiBoxStyle);

            if (GUI.Button (new Rect (400, 80, 300, 60), "ShowAd", customTextSize)) {
                ShowAd();
            }

            if (GUI.Button (new Rect (400, 170, 300, 60), "ShowRewardAd", customTextSize)) {
                ShowRewardedAd() ;  
            }

        }
    }
}
