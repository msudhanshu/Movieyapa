using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//data
using KiwiCommonDatabase;
using SimpleSQL;


[Serializable]
public class QuestionModel : BaseDbModel
{
	[PrimaryKey]
	public string _id {get; set;}
	public string title {get; set;}
    public List<string> hints;
    public string level {get; set;}
    public QEffectModel qEffect;
    public QuestionAsset questionAsset;
    public QAnswerModel answer;
    public List<string> tags;

    public string GetImageUrl() {
        //if on game server
        //if on packed local apk
        //if on cdn
        return ServerConfig.BASE_URL+questionAsset.assetUrl;
    }

    public static QuestionModel GetQuestionData(string _id) {
        QuestionModel q = KiwiCommonDatabase.DataHandler.wrapper.questions.Find(x => x._id.Equals(_id));
        //return DatabaseManager.GetInstance ().GetDbHelper ().QueryObjectById<QuestionModel> (_id);
        return q;
    }

    static int i=-1;
    public static QuestionModel GetNextQuestionData() {
        List<QuestionModel> qd = GetAllQuestionData();
        if(qd.Count>i+1)
            i++;
        return GetAllQuestionData()[i];
    }

    public static List<QuestionModel> GetAllQuestionData() {
        return DatabaseManager.GetInstance ().GetDbHelper ().QueryForAll<QuestionModel> ();
    }
}


public enum QuestionType {
    IMAGE,
    MUSIC,
    GIF,
    TEXT,
    SPRITE,
    DEFAULT
}

public enum CdnType {
    UNKNOWN,
    RESOURCE,
    APK,
    SDCARD,
    SERVER,
    CDN
}

public class QuestionAsset //: BaseDbModel 
{
  //  [PrimaryKey]
  //  public string _id {get; set;}
    public string mime {get; set;}
    public string assetUrl {get; set;}
    public string cdn {get; set;}
    public int level {get; set;}
    public string frameCount {get; set;}
    public int fps {get; set;}
    public List<string> fx {get; set;}
    public List<string> antifx {get; set;}

    private int tgifFrameCount;
    private int tgifSpriteColumn;
    private int tgifSpriteRow;
    //private bool tisGifSprite;
    private bool isInit = false;

    private void ParseFrameCount() {
        if(isInit) return;
        if(questionType == QuestionType.SPRITE) {
            try{
            String[] s = frameCount.Split('x');
            tgifSpriteColumn = Int32.Parse(s[0]);
            tgifSpriteRow = Int32.Parse(s[1]);
            }catch(Exception e) {
                tgifSpriteColumn = 1;
                tgifSpriteRow = 1;
            }
        } else {
            tgifFrameCount = Int32.Parse(frameCount);
        }
        isInit = true;
    }

    public bool isGifSprite {
        get {
            return questionType==QuestionType.SPRITE;
        }
    }

    public int gifFrameCount {
        get {
            ParseFrameCount();
            return tgifFrameCount;
        }
    }

    public int gifSpriteColumn {
        get {
            ParseFrameCount();
            return tgifSpriteColumn;
        }
    }

    public int gifSpriteRow {
        get {
            ParseFrameCount();
            return tgifSpriteRow;
        }
    }

    public QuestionType questionType {
        get{
            switch(mime) {
            case "image":
                return QuestionType.IMAGE;
                break;
            case "music":
                return QuestionType.MUSIC;
                break;
            case "gif":
                return QuestionType.GIF;
                break;
            case "sprite":
                return QuestionType.SPRITE;
                break;
            case "text":
                return QuestionType.TEXT;
                break;
            default:
                return QuestionType.IMAGE;
            }
        }
    }

    public CdnType cdnType {
        get{
            switch(cdn) {
            case "res":
                return CdnType.RESOURCE;
                break;
            case "apk":
                return CdnType.APK;
                break;
            case "sd":
                return CdnType.SDCARD;
                break;
            case "server":
                return CdnType.SERVER;
                break;
            case "cdn":
                return CdnType.CDN;
                break;
            default:
                return CdnType.UNKNOWN;
            }
        }
    }
}

public class QAnswerModel //: BaseDbModel 
{
    //  [PrimaryKey]
    //  public string _id {get; set;}

    public string ans {get; set;}
    public bool oneChoice {get; set;}
    public List<string> options {get; set;}

    public bool multiChoice {get; set;}
    public List<string> extraAns  {get; set;}

    //time segment
    public bool ts_flag {get; set;}
    public int ts_options {get; set;}
    public int ts_ans {get; set;}
}

[Serializable]
public class QEffectModel : IEffectData
{
    public string effect_id {get; set;}
    //override param
    public string param1 {get; set;}


    // EffectModel has static data , constact min max limit, default value
    // QEffectModel has question based param , which overrides the EffectModel values
    // from effect_id get EffectEnum .. 

    private EffectModel teffectModel = null;
    public EffectModel effectModel {
        get {
            if(teffectModel == null) {
                teffectModel = EffectModel.GetEffectModelById(effect_id);
            }
            return teffectModel;
        }
 
    }

    public QEffectModel(string effect_id) {
        this.effect_id = effect_id;
    }

    public QEffectModel(EffectEnum effect_enum) {
        this.effect_id = SEnum.GetStringValue(effect_enum);
    }

    public EffectEnum getEffectType() {
        return Effect.StringToEffectType(effect_id);
    }

    public string getShader() {
        return  effectModel.shaderName;
    }

    public bool isMusic(){
        return false;
    }

    public string getName() {
        return effectModel.name;
    }
}