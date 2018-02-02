using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SgUnity;
using UnityExtensions;
using TMPro;

public class QTextFramer : QAssetFramer
{
    TextMeshPro textMesh;

    override public void Awake()
    {
        textMesh = gameObject.GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            textMesh = gameObject.AddComponent<TextMeshPro>();
        }
    }

    override protected void SetQuestionAsset()
    {
        textMesh.SetText(questionData.questionAsset.getAssetUrl());
        //questionImage.texture = Resources.Load<Texture>( questionData.questionImage );
        //if(questionImage.texture==null)
        //QAssetDownloadManager.GetInstance().SetQuestionImage(questionData.questionAsset, this);
        // StartCoroutine(CoroutineLoadFromUrl());
    }


    override protected void ResetOldFrame()
    {
        base.ResetOldFrame();
        if (textMesh != null)
            textMesh.SetText("");
    }

}
