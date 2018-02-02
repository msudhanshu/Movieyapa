using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SgUnity;

[RequireComponent(typeof(AnimateTiledTexture))]
public class QSpriteGifFramerBackup : QTextureFramer, IAssetLoadCallback<List<Texture2D>>, IAssetLoadCallback<Texture2D>
{

    private UnityEngine.Object[] objects;
    //private ImageSequenceTextureArray imagesSequencer;
    private AnimateTiledTexture spriteSequencer;

    override public void Awake()
    {
        //imagesSequencer =  gameObject.GetComponent<ImageSequenceTextureArray>();
        spriteSequencer = gameObject.GetComponent<AnimateTiledTexture>();
        //this.imagesSequencer.textureInitialized = false;
        //Load all textures found on the Sequence folder, that is placed inside the resources folder  
    }


    override protected void SetQuestionAsset()
    {
        //questionImage.texture = Resources.Load<Texture>( questionData.questionImage );
        //if(questionImage.texture==null)

        //WHAT KIND OF GIF IS THIS.. SPRITE , DIFF IMAGES ETC
        // IS IT BEING DOWNLOADED FROM LOCAL OR NET, SDCARD OR ASSETS
        // REFACTOR ASSETRESOLVER ?????????

        //imagesSequencer.gameObject.SetActive(true);
        spriteSequencer.gameObject.SetActive(true);

        //bool isSprite = questionData.questionAsset.isGifSprite;

        //   if(isSprite) {
        spriteSequencer.gameObject.SetActive(true);
        //StartCoroutine(CoroutineLoadSpriteFromUrl());
        QAssetDownloadManager.GetInstance().SetQuestionGifSprite(questionData.questionAsset, this);
        //        } else {
        //            imagesSequencer.gameObject.SetActive(true);
        //            //StartCoroutine(CoroutineImagesLoadFromUrl());
        //            QAssetDownloadManager.GetInstance().SetQuestionGif(questionData.questionAsset,this);
        //        }
        //StartCoroutine(CoroutineLoadFromResources("gif1"));
    }

    public void assetLoadSuccess(Texture2D asset)
    {
        Debug.Log("Asset Load Successfull" + questionData.questionAsset.assetUrl);
        SetQuestionImage(asset);
        this.spriteSequencer._columns = questionData.questionAsset.gifSpriteColumn;
        this.spriteSequencer._rows = questionData.questionAsset.gifSpriteRow;
        this.spriteSequencer._framesPerSecond = questionData.questionAsset.fps;
        this.spriteSequencer.SetupAndPlay();
        onLoadSuccess();
    }

    public void assetLoadSuccess(List<Texture2D> asset)
    {
        //        Debug.Log("Asset Load Successfull" + questionData.questionAsset.assetUrl);
        //        int gifFrameSize =  asset.Count;
        //        this.imagesSequencer.textures = new Texture[gifFrameSize];  
        //        for(int i=0; i< asset.Count; i++)
        //        {  
        //            this.imagesSequencer.textures[i] = (Texture)asset[i];
        //        }
        //        SetQuestionImage(null);
        //        this.imagesSequencer.FPS = questionData.questionAsset.fps;
        //        this.imagesSequencer.textureInitialized = true;
        //        onLoadSuccess();
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



#if uncommented_old_imp
    #region commented
       
    public IEnumerator CoroutineLoadSpriteFromUrl() {
        string baseUrl = questionData.GetImageUrl();
        string url = baseUrl;
        Debug.Log("File being loaded: "+url);
        //screen.SetLoading();
        //questionImage.sprite
        WWW www= new WWW (url);
        while(!www.isDone) {
            yield return 0;
        }
        // Wait for download to complete
        //  yield www;
        Texture2D texTmp = new Texture2D(1024, 1024, TextureFormat.DXT5, false); //LoadImageIntoTexture compresses JPGs by DXT1 and PNGs by DXT5    
        if(texTmp!=null) {
            try {
                www.LoadImageIntoTexture(texTmp);
                //screen.SetLoading(false);
                //SetQuestionImage(www.texture);
                //this.sequencer.textures[i] = www.texture ; //(Texture)this.objects[i]; 
                //this.spriteSequencer.SetupAndPlay();
                SetQuestionImage(www.texture);
                Debug.Log(www.text);
                //callback(screen, www.texture);
            } catch (Exception e) {
                Debug.Log(e.ToString());
            }
        }
        this.spriteSequencer.SetupAndPlay();
       // this.sequencer.textureInitialized = true;
        yield break;
    }

    public IEnumerator CoroutineImagesLoadFromUrl() {
        string baseUrl = "http://localhost:9000/externalAssets/";questionData.GetImageUrl();
        int gifFrameSize = 6;
        this.imagesSequencer.textures = new Texture[gifFrameSize];  
        string url;
        for(int i=0; i < gifFrameSize; i++) {
            url = baseUrl + (i) +".png";
            Debug.Log("File being loaded: "+url);
            //screen.SetLoading();
            //questionImage.sprite
            WWW www= new WWW (url);
            while(!www.isDone) {
                yield return 0;
            }
            // Wait for download to complete
            //  yield www;
            Texture2D texTmp = new Texture2D(1024, 1024, TextureFormat.DXT5, false); //LoadImageIntoTexture compresses JPGs by DXT1 and PNGs by DXT5    
            if(texTmp!=null) {
                try {
                    www.LoadImageIntoTexture(texTmp);
                    //screen.SetLoading(false);
                    //SetQuestionImage(www.texture);

                    this.imagesSequencer.textures[i] = www.texture ; //(Texture)this.objects[i];  
                    Debug.Log(www.text);
                    //callback(screen, www.texture);
                } catch (Exception e) {
                    Debug.Log(e.ToString());
                }
            }
            yield return 0;
        }
        SetQuestionImage(null);
        this.imagesSequencer.textureInitialized = true;
        yield break;
    }
        
    public IEnumerator CoroutineLoadFromResources(string seqdir) {
        this.objects = Resources.LoadAll(seqdir, typeof(Texture));  
        yield return 0;

        int gifFrameSize = objects.Length;

        //Initialize the array of textures with the same size as the objects array  
        this.imagesSequencer.textures = new Texture[gifFrameSize];  

        //Cast each Object to Texture and store the result inside the Textures array  
        for(int i=0; i <gifFrameSize; i++)  
        {  
            this.imagesSequencer.textures[i] = (Texture)this.objects[i];  
        }
        this.imagesSequencer.textureInitialized = true;
        yield return 0;
    }

    private void SetQuestionImage(Texture t) {
        SetImageEffect(t);
    }
//        //questionImage.texture  = t;
//        //gameObject.GetComponent<Renderer>().material.mainTexture = t;
//        //todo test
//        EffectEnum effectType = EffectEnum.GRAY;
//        //questionImage3D.GetComponent<Renderer>().material.mainTexture = t;
//        //questionImage3D.AddComponent<Effect.GetEffect(EffectEnum.DEFAULT)>();
//        //Effect e = Effect.AddEffect(questionImage3D,effectType);
//        Effect e = gameObject.GetComponent<Effect>();
//        e.effectData = new EffectData(effectType,t);
//        e.ReloadEffect();
//    }
    #endregion
#endif
}
