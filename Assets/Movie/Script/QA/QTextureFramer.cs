using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SgUnity;

public class QTextureFramer : QAssetFramer{
    [HideInInspector]
    public Texture mainTexture;
          
    protected void SetImageEffect(Texture t) {

        this.mainTexture = t;
        IEffectData effectData = questionData.qEffect;
        Effect e = Effect.AddEffect(gameObject,effectData.getEffectType());
        e.effectData = effectData;
        e.ReloadEffect(t);

//        if(e is TextureEffect) {
//            TextureEffect ie = e as TextureEffect;
//            ie.mainTexture = t;
//            ie.ReloadEffect(t);
//        } else {
//            Debug.LogError("This Effect is not a Image effect.="+effectData.getName());
//        }
    }
}
