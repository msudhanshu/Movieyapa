using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SgUnity;

public class UIQuestionController : UIGamePanel
{
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

    public void Init(QuestionModel questData)
    {
        this.questionData = questData;
        SetQuestion();
        //WE CAN SET CALL BACK TO SETQUEISTION->SETQFRAME , THEN ON callback set options
        title.text = questionData.title;
        option1.text = questionData.answer.options[0];
        option2.text = questionData.answer.options[1];
        option3.text = questionData.answer.options[2];
        option4.text = questionData.answer.options[3];
    }

    private void SetQuestion()
    {
        qCameraController.SetQCamera(questionData.questionAsset.questionType);
        qFrameController.SetQFrame(questionData);
    }

    private List<string> answeredOptions = new List<string>();
    public void AnswerSelected(int i)
    {
        qFrameController.DestoryQFrame();
        answeredOptions.Add(questionData.answer.options[Math.Max(0, i - 1)]);
        //TEMP FOR SINGLE CORRECT , TRIGGER DONE AS SOON AS ONE QUESTION IS ANSWERED
        OnQuestionAnswered();
    }


    //TODO FIXME
    // in case of more than one correct
    public void OnQuestionAnswered()
    {
        int score = 0;
        if (answeredOptions != null && answeredOptions.Count > 0)
        {
            foreach (string ao in answeredOptions)
            {
                if (Utility.StringEquals(questionData.answer.ans, ao))
                {
                    score++;
                }
            }
        }
        answeredOptions.Clear();
        QuestionAnswerManager.GetInstance().OnQuestionAnswered(questionData, score);
    }


}
