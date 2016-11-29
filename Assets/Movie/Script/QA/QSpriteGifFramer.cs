using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SgUnity;

[RequireComponent(typeof(AnimateTiledTexture))]
public class QSpriteGifFramer : QTextureFramer,IAssetLoadCallback<Texture2D> {

    private AnimateTiledTexture spriteSequencer;

    override public void Awake ()  
    {  
        spriteSequencer =  gameObject.GetComponent<AnimateTiledTexture>();
    }  

    override protected void SetQuestionAsset() {
        spriteSequencer.gameObject.SetActive(true);
        QAssetDownloadManager.GetInstance().SetQuestionGifSprite(questionData.questionAsset,this);
    }

    public void assetLoadSuccess(Texture2D asset) {
        Debug.Log("Asset Load Successfull" + questionData.questionAsset.assetUrl);
        SetQuestionImage(asset);
        this.spriteSequencer._columns = questionData.questionAsset.gifSpriteColumn;
        this.spriteSequencer._rows = questionData.questionAsset.gifSpriteRow;
        this.spriteSequencer._framesPerSecond = questionData.questionAsset.fps;
        this.spriteSequencer.SetupAndPlay();
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
