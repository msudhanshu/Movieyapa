#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[RequireComponent(typeof(QAssetFramer))]
public class EffectTool : MonoBehaviour
{
    public EffectEnum effectType;
	public static bool isDebug = true;
    public void ReloadEffect() {
        Effect.CleanEffects(this.gameObject);
        IEffectData effectData = new QEffectModel(effectType);
        Effect e = Effect.AddEffect(this.gameObject,effectData.getEffectType());
        e.effectData = effectData;
 //       e.ReloadEffect();

        QAssetFramer qf = GetComponent<QAssetFramer>();

        if((e is TextureEffect) && (qf is QTextureFramer) ) {
            TextureEffect ie = e as TextureEffect;
            QTextureFramer qif = qf as QTextureFramer;
            //ie.mainTexture = t;
            ie.ReloadEffect<Texture>(qif.mainTexture);
        } else if( (e is MusicEffect) && (qf is QMusicFramer) ) {
            MusicEffect me = e as MusicEffect;
            QMusicFramer mif = qf as QMusicFramer;
            me.ReloadEffect<AudioClip>(mif.audioClip);
        } 
        else {
            Debug.LogError("This Effect does not have matching Effect class and assetFramer class:="+effectData.getName());
        }
//

//        if(qAssetFramer !=null && qAssetFramer.mainTexture!=null) {
//            e.mainTexture = qAssetFramer.mainTexture;
//        }
//
//        e.ReloadEffect();


    }
}
#endif