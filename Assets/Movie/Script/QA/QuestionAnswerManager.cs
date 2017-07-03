using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using SgUnity;
using System.Collections.Generic;
using System;

namespace SgUnity {

    public class QuestionAnswerManager : Manager<QuestionAnswerManager> {

    	public UIQuestionController uiQuestion;
        private IQuestionAnsweredCallback questionAnswered;

        override public void PopulateDependencies() {}
        override public void StartInit() {}


        public void Reset(IQuestionAnsweredCallback questionAnswered) {
            this.questionAnswered = questionAnswered;
        }

    	public void ShowNextCarrierQuestion() {
            GenerateNextCarrierQuestion(CapitalManager.GetInstance().Level,0);
    	}

        public void ShowNextPackageQuestion(string nextQuestionId) {
            
            GenerateNextQuestionById(CapitalManager.GetInstance().Level, nextQuestionId, 0);
        }
        
    	public void OnQuestionAnswered(int score) {
            if(score > 0)
                questionAnswered.OnCorrectAnswer();
            else
                questionAnswered.OnWrongAnswer();
    	}
    	
//    	public virtual void OnSuccess(GameResponse response) {
//            QuestionModel q = JsonConvert.DeserializeObject<QuestionModel>(response.response);
//            SetQuestionToUI(q);
//    	}
//
//    	public virtual void OnFailure(string error) {
//
//    	}

        public void SetQuestionToUI(QuestionModel q ) {
            SetQuestionEffect(q);
            uiQuestion.Init(q);
        }

        //LOGIC TO PUT RANDOM QUESTION EFFECT.... QUESTION GENERATOR ENGINE
        //logic to get question , either by id or random next..
        //try to get locally or else do server call ..
        // this call reply should have qeffect embeden in it. if server then json should have that

        public void GenerateNextCarrierQuestion(int level, int currentQuestionIndex) {
			SgUnity.ServerAction.takeAction(ActionEnum.GET_NEXT_QUESTION,new GetQuestionCallback());
			return;
			/*
			//generate next question id
            try{
                List<QuestionModel> allqs = KiwiCommonDatabase.DataHandler.wrapper.questions;
                QuestionModel nextQuestion = allqs[(int)UnityEngine.Random.Range (0, allqs.Count)];

                if(nextQuestion !=null)  
                    SetQuestionToUI(nextQuestion);
                else {
//                    SgUnity.ServerAction.GetQuestionByIdAction(ActionEnum.GET_QUESTION_BY_ID, nextQuestion, new GetQuestionCallback());
//                else
                    SgUnity.ServerAction.takeAction(ActionEnum.GET_NEXT_QUESTION,new GetQuestionCallback());
                }
            } catch (Exception e) {
                SgUnity.ServerAction.takeAction(ActionEnum.GET_NEXT_QUESTION,new GetQuestionCallback());
            }
			*/
        }

        public void GenerateNextQuestionById(string question_id) {
            GenerateNextQuestionById(CapitalManager.GetInstance().Level,question_id);
        }
        public void GenerateNextQuestionById(int level, string question_id, int currentQuestionIndex=0) {
//            QuestionModel q = QuestionModel.GetQuestionData(question_id);
//            if(q!=null)
//                SetQuestionToUI(q);
//            else 
                SgUnity.ServerAction.GetQuestionByIdAction(ActionEnum.GET_QUESTION_BY_ID, question_id, new GetQuestionCallback());
        }


        private void SetQuestionEffect(QuestionModel q) {
            if(q.qEffect == null) {
                q.qEffect = new QEffectModel(EffectEnum.GRAY);
            }
        }
    }

    public class GetQuestionIdCallback : SgUnity.ServerNotifier {
        
        public virtual void OnSuccess(GameResponse response) {
            QuestionModel q = JsonConvert.DeserializeObject<QuestionModel>(response.response);
            QuestionAnswerManager.GetInstance().SetQuestionToUI(q);
        }

        public virtual void OnFailure(string error) {

        }
    }

    public class GetQuestionCallback : SgUnity.ServerNotifier {
        
        public virtual void OnSuccess(GameResponse response) {
            QuestionModel q = JsonConvert.DeserializeObject<QuestionModel>(response.response);
            QuestionAnswerManager.GetInstance().SetQuestionToUI(q);
        }

        public virtual void OnFailure(string error) {

        }
    }
}