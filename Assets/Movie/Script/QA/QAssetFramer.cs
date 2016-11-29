using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SgUnity;

public class QAssetFramer : MonoBehaviour{

    protected QuestionModel questionData;

    virtual public void Awake() {
        
    }

    protected void onLoadSuccess() {
        Debug.Log("Asset Load Successfull" + questionData.questionAsset.assetUrl);
        //disable loadingbar

    }

    protected void onLoadFailed() {
        Debug.LogError("Asset Load Failed" + questionData.questionAsset.assetUrl);
    }

    public void SetFrame(QuestionModel questionData) {
        this.questionData = questionData;
        SetQuestionAsset();
    }

    virtual protected void SetQuestionAsset() {
        
    }
}
