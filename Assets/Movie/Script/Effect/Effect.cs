using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Effect : MonoBehaviour {
    public IEffectData effectData;

//	virtual public void ReloadEffect() {
//        
//	}

    virtual public void ReloadEffect<T>(T asset) {
        UpdateEffectTool();
    }

	virtual public void Init() {

	}

    private void UpdateEffectTool() {
        #if UNITY_EDITOR
        EffectTool et = GetComponent<EffectTool>();
        if(et != null) {
            et.effectType = effectData.getEffectType();
        }
        #endif
    }

    #region STATIC_UTIL_FUNTIONS

	public static Effect AddEffect(GameObject targetgo, EffectEnum effectType) {
		CleanEffects(targetgo);
		switch (effectType) {
			case EffectEnum.DEFAULT:
			case EffectEnum.GRAY:
            return targetgo.AddComponent<IE_Base>();
				break;
			default:
				return targetgo.AddComponent<IE_Base>();
				break;
		}
	}

	public static void CleanEffects(GameObject go) {
		Effect[] e = go.GetComponents<Effect>();
		if(e !=null) {
			foreach(Effect t in e) {
				Destroy(t);
			}
		}
    }

    static Array effectEnumArray = Enum.GetValues(typeof(EffectEnum));
    public static EffectEnum StringToEffectType(string effectType) {
        foreach( EffectEnum val in effectEnumArray )
        {
            if( Utility.StringEquals(SEnum.GetStringValue(val) , effectType) ) return val;
        }
        return EffectEnum.DEFAULT;
    }

    #endregion

}
