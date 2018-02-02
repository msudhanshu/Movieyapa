using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//data
using KiwiCommonDatabase;
using SimpleSQL;


[Serializable]
public class EffectModel : BaseDbModel
{
	[PrimaryKey]
    public string _id {get; set;}
    public string name{get; set;}
    public string shaderName {get;set;}
    public List<string> noiseAsset {get; set;} //other images/sound to blend with
    //  public List<int> ints;
    //  public List<float> floats;
    //  public Texture mainTexture;
    //  public List<Texture> textures;
   
    public EffectModel() {

    }

//    public EffectModel(string _id, string name) {
//        this._id = _id;
//        this.name = name;
//    }

    public EffectModel(string _id, string name, string shaderName) {
        this._id = _id;
        this.name = name;
        this.shaderName = shaderName;
    }

    public static EffectModel GetEffectModelById(string _id) {
		EffectModel q;
		#if UNITY_EDITOR
		if (EffectTool.isDebug) {
			 q = InitSelfNonDiffMarketTable().Find(x => Utility.StringEquals(x._id,_id));

		} else {
		 q = KiwiCommonDatabase.DataHandler.wrapper.effects.Find(x => Utility.StringEquals(x._id,_id));
        //return DatabaseManager.GetInstance ().GetDbHelper ().QueryObjectById<QuestionModel> (_id);
		}
		#else
		 q = KiwiCommonDatabase.DataHandler.wrapper.effects.Find(x => Utility.StringEquals(x._id,_id));

		#endif


        return q;
    }

    public static List<EffectModel> InitSelfNonDiffMarketTable() {
        List<EffectModel> list = new List<EffectModel>();
        list.Add(new EffectModel("DEFAULT","DEFAULT","DEFAULT"));
        list.Add(new EffectModel("gray","gray","EffectGray"));
        list.Add(new EffectModel("EDGE_DETECT","EDGE_DETECT","EffectEdgeDetect"));
       
        return list;
    }

}