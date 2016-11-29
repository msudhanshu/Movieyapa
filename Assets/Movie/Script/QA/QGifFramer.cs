using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SgUnity;

[RequireComponent(typeof(ImageSequenceTextureArray))]
public class QGifFramer : QTextureFramer,IAssetLoadCallback<List<Texture2D>> {

    private ImageSequenceTextureArray imagesSequencer;

    override public void Awake ()  
    {  
        imagesSequencer =  gameObject.GetComponent<ImageSequenceTextureArray>();
        this.imagesSequencer.textureInitialized = false;
    }  
      
    override protected void SetQuestionAsset() {
        imagesSequencer.gameObject.SetActive(true);
        imagesSequencer.gameObject.SetActive(true);
        QAssetDownloadManager.GetInstance().SetQuestionGif(questionData.questionAsset,this);
    }

    public void assetLoadSuccess(List<Texture2D> asset) {
        Debug.Log("Asset Load Successfull" + questionData.questionAsset.assetUrl);
        int gifFrameSize =  asset.Count;
        this.imagesSequencer.textures = new Texture[gifFrameSize];  
        for(int i=0; i< asset.Count; i++)
        {  
            this.imagesSequencer.textures[i] = (Texture)asset[i];
        }
        SetQuestionImage((Texture)asset[0]);
        this.imagesSequencer.FPS = questionData.questionAsset.fps;
        this.imagesSequencer.textureInitialized = true;
        onLoadSuccess();
    }

    public void assetLoadFailed() {
        Debug.LogError("Asset Load Failed" + questionData.questionAsset.assetUrl);
        onLoadFailed();
    }
     
    private void SetQuestionImage(Texture t) {
        SetImageEffect(t);
    }

}