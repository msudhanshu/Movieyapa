using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SgUnity;

public class UIQuestionController : UIGamePanel {
    public RectTransform qTextFrame;
    public Text qText;
    public Text title;
	public Text option1;
	public Text option2;
	public Text option3;
	public Text option4;
    public QCameraController qCameraController;
    public QFrameController qFrameController;
	private QuestionModel questionData;

	public void Init(QuestionModel questData) {
		this.questionData  = questData;
		title.text = questionData.title;
        SetQuestion();
		option1.text = questionData.answer.options[0];
		option2.text = questionData.answer.options[1];
        option3.text = questionData.answer.options[2];
        option4.text = questionData.answer.options[3];
	}

    private void SetQuestion() {
        qCameraController.SetQCamera(questionData.questionAsset.questionType);
        if(questionData.questionAsset.questionType == AssetType.TEXT) {
            qTextFrame.gameObject.SetActive(true);
            qText.text = questionData.questionAsset.assetUrl;
        } else 
             qTextFrame.gameObject.SetActive(false);
             qFrameController.SetQFrame(questionData);
//        switch(questionData.questionAsset.questionType) {
//        case QuestionType.IMAGE:
//            //SetQuestionImage();
//            break;
//        case QuestionType.GIF:
//
//            break;
//        case QuestionType.MUSIC:
//
//            break;
//        }
    }


    private void SetQuestionMusic() {
        //questionImage.texture = Resources.Load<Texture>( questionData.questionImage );
        //if(questionImage.texture==null)
        StartCoroutine(CoroutineLoadMusicFromUrl());
    }

    //1,abstractoin at this level , for facebook dowloader api coroutine 
    //2. abstraction for downloading through , www, or resourceload, or facebookdownloadapi
    IEnumerator CoroutineLoadMusicFromUrl() {
        yield return 0;
    }

    private void SetQuestionMovie() {
        //questionImage.texture = Resources.Load<Texture>( questionData.questionImage );
        //if(questionImage.texture==null)
       
        StartCoroutine(CoroutineLoadMovieFromUrl());
    }

    //1,abstractoin at this level , for facebook dowloader api coroutine 
    //2. abstraction for downloading through , www, or resourceload, or facebookdownloadapi
    IEnumerator CoroutineLoadMovieFromUrl() {
        yield return 0;
    }

	private List<string> answeredOptions = new List<string>();
	public void AnswerSelected(int i) {
		answeredOptions.Add(questionData.answer.options[Math.Max(0, i-1)]);
		//TEMP FOR SINGLE CORRECT , TRIGGER DONE AS SOON AS ONE QUESTION IS ANSWERED
		OnQuestionAnswered();
	}


	//TODO FIXME
	// in case of more than one correct
	public void OnQuestionAnswered() {
		int score = 0;
		if(answeredOptions!=null && answeredOptions.Count>0) {
			foreach(string ao in answeredOptions) {
                if(	Utility.StringEquals(questionData.answer.ans, ao ) ) {
					score++;
				}
			}
		}
		answeredOptions.Clear();
		QuestionAnswerManager.GetInstance().OnQuestionAnswered(score);
	}


}
