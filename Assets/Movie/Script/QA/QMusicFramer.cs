using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SgUnity;

//REFACTOR : to show different kind of visualization , it should have many child object with having self sustain prefab

public class QMusicFramer : QAssetFramer,IAssetLoadCallback<AudioClip> {
    AudioSource audioSource;
    public AudioClip audioClip;
	override public void Awake () {
        audioSource = GetComponentInChildren<AudioSource>();
	}
	
    override protected void SetQuestionAsset() {
        QAssetDownloadManager.GetInstance().SetQuestionMusic(questionData.questionAsset,this);
    }

    public void assetLoadSuccess(AudioClip asset) {
        Debug.Log("Asset Load Successfull" + questionData.questionAsset.assetUrl);
        onLoadSuccess();
        audioSource.clip = asset;
        SetAudioEffect(asset);
        audioSource.Play();
    }

    public void assetLoadFailed() {
        Debug.LogError("Asset Load Failed" + questionData.questionAsset.assetUrl);
        onLoadFailed();
    }
        
    public void SetAudioEffect(AudioClip clip) {
        IEffectData effectData = questionData.qEffect;
        this.audioClip = clip;
        Effect e = Effect.AddEffect(gameObject,effectData.getEffectType());
        if(e is MusicEffect) {
            MusicEffect me = e as MusicEffect;
            me.effectData = effectData;
            me.audioClip = clip;
            me.ReloadEffect(clip);
        } else {
            Debug.LogError("This Effect is not a Music effect.="+effectData.getName());
        }
    }
}
