using UnityEngine;
using System.Collections;

public class QFrameController : MonoBehaviour {

    public QImageFramer imageFrame;
    public QGifFramer gifFrame;
    public QSpriteGifFramer spriteGifFrame;
    public QMusicFramer musicFrame;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void SetQFrame(QuestionModel questionData)
    {
        QuestionType  qType =  questionData.questionAsset.questionType;
        SetQFrameActive(qType);
        switch(qType) {
            case QuestionType.IMAGE:
                 imageFrame.SetFrame(questionData);
                break;
            case QuestionType.GIF:
                 gifFrame.SetFrame(questionData);
                 break;
            case QuestionType.SPRITE:
                spriteGifFrame.SetFrame(questionData);
                break;
            case QuestionType.MUSIC:
                musicFrame.SetFrame(questionData);
                break;
        }
    }

    public void SetQFrameActive(QuestionType qType)
    {
        spriteGifFrame.gameObject.SetActive(false);
        gifFrame.gameObject.SetActive(false);
        musicFrame.gameObject.SetActive(false);
        imageFrame.gameObject.SetActive(false);

        switch(qType) {
        case QuestionType.IMAGE:
            imageFrame.gameObject.SetActive(true);
            break;
        case QuestionType.GIF:
            gifFrame.gameObject.SetActive(true);
            break;
        case QuestionType.SPRITE:
            spriteGifFrame.gameObject.SetActive(true);
            break;
        case QuestionType.MUSIC:
            musicFrame.gameObject.SetActive(true);
            break;
        }
    }
}
