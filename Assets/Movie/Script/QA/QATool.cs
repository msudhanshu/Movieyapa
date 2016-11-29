#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

//[RequireComponent(typeof(QAssetFramer))]
using SgUnity;


public class QATool : MonoBehaviour
{
    public string questionId;

    public void ReloadEffect() {
        QuestionAnswerManager.GetInstance().GenerateNextQuestionById(questionId);
    }
}
#endif