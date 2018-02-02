using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SgUnity;

public class QAssetFramer : MonoBehaviour
{

    protected QuestionModel questionData;
    public GameObject loadingBar;

    virtual public void Awake()
    {

    }

    protected void onLoadSuccess()
    {
        Debug.Log("Asset Load Successfull" + questionData.questionAsset.assetUrl);
        //disable loadingbar
        loadingBar.SetActive(false);

    }

    protected void onLoadFailed()
    {
        Debug.LogError("Asset Load Failed" + questionData.questionAsset.assetUrl);
    }

    virtual protected void ResetOldFrame()
    {

        Material baseMaterial = GetComponentInChildren<Renderer>().material;
        baseMaterial.mainTexture = null;
        //this.mainTexture = t;

    }

    virtual public void DestoryFrame()
    {
        ResetOldFrame();
    }

    virtual public void SetFrame(QuestionModel questionData)
    {
        loadingBar.SetActive(true);
        this.questionData = questionData;
        SetQuestionAsset();
    }

    virtual protected void SetQuestionAsset()
    {

    }
}
