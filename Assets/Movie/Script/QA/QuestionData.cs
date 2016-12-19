using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//data
using KiwiCommonDatabase;
using SimpleSQL;

//DEPRECATED
[Serializable]
public class QuestionData : BaseDbModel
{
	[PrimaryKey]
	public string questionId {get; set;}
	public string title {get; set;}
	public string image {get; set;}
	public string questType {get; set;}

	public string questionImage {get; set;}
	public string imageURL {get; set;}

//	public string questionDialogue {get; set;}
//	public string questionMusic {get; set;}
//	public string questionMovie {get; set;}

	public string option1 {get; set;}
	public string option2 {get; set;}
	public string option3 {get; set;}
	public string option4 {get; set;}

	public int answer {get; set;}

	public List<string> answers {get; set;}
	public List<string> options {get; set;}

	public AssetType questionType {
		get{
			switch(questType) {
			case "image":
				return AssetType.IMAGE;
				break;
			case "music":
				return AssetType.MUSIC;
				break;
            case "gif":
                return AssetType.GIF;
                break;
			default:
				return AssetType.IMAGE;
			}
		}
	}
	
	public string GetImageUrl() {
		//if on game server
		//if on packed local apk
		//if on cdn
		return ServerConfig.BASE_URL+image;
	}

	public static QuestionModel GetQuestionData(string _id) {
		return DatabaseManager.GetInstance ().GetDbHelper ().QueryObjectById<QuestionModel> (_id);
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