using UnityEngine;
using SgUnity;

namespace SgUnity {
    public interface IQuestionAnsweredCallback {
		void OnCorrectAnswer(QuestionModel q);
		void OnWrongAnswer(QuestionModel q);
    }
}