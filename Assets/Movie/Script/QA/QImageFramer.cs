using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SgUnity;
using UnityExtensions;

public class QImageFramer : QTextureFramer,IAssetLoadCallback<Texture2D> {

    public static float FRAME_RATIO = 1366.0f/616.0f;

    public void SetFrame(QuestionModel questionData) {
        this.questionData = questionData;
        SetQuestionAsset();
    }

    override protected void SetQuestionAsset() {
        //questionImage.texture = Resources.Load<Texture>( questionData.questionImage );
        //if(questionImage.texture==null)
        QAssetDownloadManager.GetInstance().SetQuestionImage(questionData.questionAsset,this);
        // StartCoroutine(CoroutineLoadFromUrl());
    }

    public void assetLoadSuccess(Texture2D asset) {
        Debug.Log("Asset Load Successfull" + questionData.questionAsset.assetUrl);
        onLoadSuccess();
        //SetSize();
        SetImageEffect(asset);
    }

    public void SetSize() 
    {
        transform.SetScaleZ(1.0f/FRAME_RATIO);
    }

    public void assetLoadFailed() {
        Debug.LogError("Asset Load Failed" + questionData.questionAsset.assetUrl);
        onLoadFailed();
    }

}
