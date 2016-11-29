using UnityEngine;
using SgUnity;

namespace SgUnity {
    public interface IQuestionAnsweredCallback {
	    void OnCorrectAnswer();
        void OnWrongAnswer();
    }
}