using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using Unity.Linq; // using LINQ to GameObject


public class Effect : MonoBehaviour {
    public IEffectData effectData;

//	virtual public void ReloadEffect() {
//        
//	}

	virtual public string PrefabName() {
		return "default_effect_prefab";
	}

	//TODO: IT IS FOR CASES WHERE EFFECT IS SO TOUGH , SHOULD BE APPLIED ONLY FOR THE MOVIE/IMAGE WITH MOST POPULAR/KNOWN MOVIE OR SCENE..
	// WE CAN CREATE ENUM TO COVER DIFF TYPE OF MOVIE... LIKE CROWD SCENE ETC.... damsiraj
	//SO EACH MOVIE QUESTION WILL HAVE THIS DETAIL WHAT KIND OF IMAGE IS THAT...... BUT THEN QPROVIDER ALGO WILL CHANGE...
	virtual public bool applyOnlyIfMentioned() {
		return false;
	}

    virtual public void ReloadEffect<T>(T asset) {
		InstantiateEffectPrefab (this.gameObject, PrefabName());
        UpdateEffectTool();
    }

	protected GameObject InstantiateEffectPrefab(GameObject targetgo, string prefabName) {
		GameObject go;
		targetgo.Children ().Destroy (true);
		//Util.DestroyImmediateChildren (targetgo.transform);
		go = Instantiate(Resources.Load(prefabName, typeof(GameObject))) as GameObject;
		go.transform.parent = targetgo.transform;
		//CleanEffects (go);
		return go;
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
		GameObject go;
		CleanEffects(targetgo);
		switch (effectType) {
		case EffectEnum.GRAY:
			return targetgo.AddComponent<IE_Gray>();
		case EffectEnum.EDGE_DETECT:
			return targetgo.AddComponent<IE_EdgeDetection>();
		case EffectEnum.CIRCLE_HOLE:
			return targetgo.AddComponent<IE_CircleHole> ();
		case EffectEnum.RIPPLES:
			return targetgo.AddComponent<IE_Ripples> ();
		case EffectEnum.BLUR:
			return targetgo.AddComponent<IE_Blur> ();
		case EffectEnum.SLIDE_STRIPE:
			return targetgo.AddComponent<IE_SlideStripe> ();
		case EffectEnum.SLIDE_GRID:
			return targetgo.AddComponent<IE_SlideGrid> ();
		case EffectEnum.NEGATIVE:
			return targetgo.AddComponent<IE_Negative> ();
		case EffectEnum.DOUBLE_VISION:
			return targetgo.AddComponent<IE_DoubleVision> ();
		case EffectEnum.BLACKNWHITE:
			return targetgo.AddComponent<IE_BlackNWhite> ();
		case EffectEnum.DEFAULT:
			default:
			//go = InstantiateEffect (targetgo);
			return targetgo.AddComponent<IE_Default>();
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
