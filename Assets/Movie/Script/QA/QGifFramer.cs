using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SgUnity;

[RequireComponent(typeof(ImageSequenceTextureArray))]
public class QGifFramer : QTextureFramer, IAssetLoadCallback<GifTextures>
{
    private QUniGifImage m_uniGifImage;
    private ImageSequenceTextureArray imagesSequencer;

    override public void Awake()
    {

        m_uniGifImage = gameObject.GetComponent<QUniGifImage>();

        imagesSequencer = gameObject.GetComponent<ImageSequenceTextureArray>();
        if (imagesSequencer == null)
        {
            imagesSequencer = gameObject.AddComponent<ImageSequenceTextureArray>();
        }
        this.imagesSequencer.textureInitialized = false;
    }

    override protected void SetQuestionAsset()
    {
        imagesSequencer.gameObject.SetActive(true);
        //StartCoroutine(m_uniGifImage.SetGifFromUrlCoroutine(m_inputField.text));
        QAssetDownloadManager.GetInstance().SetQuestionGif(questionData.questionAsset, this);
    }

    public void assetLoadSuccess(GifTextures asset)
    {
        Debug.Log("Asset Load Successfull" + questionData.questionAsset.assetUrl);
        int gifFrameSize = asset.m_gifTextureList.Count;
        this.imagesSequencer.textures = new Texture[gifFrameSize];
        for (int i = 0; i < asset.m_gifTextureList.Count; i++)
        {
            this.imagesSequencer.textures[i] = (Texture)asset.m_gifTextureList[i].m_texture2d;
        }
        SetQuestionImage((Texture)asset.m_gifTextureList[0].m_texture2d);
        this.imagesSequencer.FPS = questionData.questionAsset.fps;
        this.imagesSequencer.textureInitialized = true;
        onLoadSuccess();
    }

    public void assetLoadFailed()
    {
        Debug.LogError("Asset Load Failed" + questionData.questionAsset.assetUrl);
        onLoadFailed();
    }

    private void SetQuestionImage(Texture t)
    {
        SetImageEffect(t);
    }

    override protected void ResetOldFrame()
    {
        base.ResetOldFrame();
        if (imagesSequencer != null)
        {
            //this.imagesSequencer.textures = null;
            this.imagesSequencer.textureInitialized = false;
            this.imagesSequencer.gameObject.SetActive(false);
        }
    }
}