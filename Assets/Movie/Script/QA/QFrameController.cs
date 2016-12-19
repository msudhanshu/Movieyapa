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
        AssetType  qType =  questionData.questionAsset.questionType;
        SetQFrameActive(qType);
        switch(qType) {
            case AssetType.IMAGE:
                 imageFrame.SetFrame(questionData);
                break;
            case AssetType.GIF:
                 gifFrame.SetFrame(questionData);
                 break;
            case AssetType.SPRITE:
                spriteGifFrame.SetFrame(questionData);
                break;
            case AssetType.MUSIC:
                musicFrame.SetFrame(questionData);
                break;
        }
    }

    public void SetQFrameActive(AssetType qType)
    {
        spriteGifFrame.gameObject.SetActive(false);
        gifFrame.gameObject.SetActive(false);
        musicFrame.gameObject.SetActive(false);
        imageFrame.gameObject.SetActive(false);

        switch(qType) {
        case AssetType.IMAGE:
            imageFrame.gameObject.SetActive(true);
            break;
        case AssetType.GIF:
            gifFrame.gameObject.SetActive(true);
            break;
        case AssetType.SPRITE:
            spriteGifFrame.gameObject.SetActive(true);
            break;
        case AssetType.MUSIC:
            musicFrame.gameObject.SetActive(true);
            break;
        }
    }
}
