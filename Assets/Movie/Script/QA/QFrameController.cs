using UnityEngine;
using System.Collections;

public class QFrameController : MonoBehaviour
{
    public QImageFramer imageFrame;
    public QTextFramer textFrame;
    public QSlideShowGifFramer slideShowGifFrame;
    public QGifFramer gifFrame;
    public QSpriteGifFramer spriteGifFrame;
    public QMusicFramer musicFrame;

    public void DestoryQFrame()
    {
        spriteGifFrame.DestoryFrame();
        gifFrame.DestoryFrame();
        slideShowGifFrame.DestoryFrame();
        musicFrame.DestoryFrame();
        imageFrame.DestoryFrame();
        textFrame.DestoryFrame();
        DeactivateAllFrame();
    }

    public void SetQFrame(QuestionModel questionData)
    {
        AssetType qType = questionData.questionAsset.questionType;
        SetQFrameActive(qType);
        switch (qType)
        {
            case AssetType.IMAGE:
                imageFrame.SetFrame(questionData);
                break;
            case AssetType.GIF:
                gifFrame.SetFrame(questionData);
                break;
            case AssetType.SLIDESHOW:
                slideShowGifFrame.SetFrame(questionData);
                break;
            case AssetType.SPRITE:
                spriteGifFrame.SetFrame(questionData);
                break;
            case AssetType.MUSIC:
                musicFrame.SetFrame(questionData);
                break;
            case AssetType.TEXT:
                textFrame.SetFrame(questionData);
                break;
        }
    }

    private void DeactivateAllFrame()
    {
        spriteGifFrame.gameObject.SetActive(false);
        gifFrame.gameObject.SetActive(false);
        slideShowGifFrame.gameObject.SetActive(false);
        musicFrame.gameObject.SetActive(false);
        imageFrame.gameObject.SetActive(false);
        textFrame.gameObject.SetActive(false);
    }

    private void SetQFrameActive(AssetType qType)
    {
        DeactivateAllFrame();
        switch (qType)
        {
            case AssetType.IMAGE:
                imageFrame.gameObject.SetActive(true);
                break;
            case AssetType.GIF:
                gifFrame.gameObject.SetActive(true);
                break;
            case AssetType.SLIDESHOW:
                slideShowGifFrame.gameObject.SetActive(true);
                break;
            case AssetType.SPRITE:
                spriteGifFrame.gameObject.SetActive(true);
                break;
            case AssetType.MUSIC:
                musicFrame.gameObject.SetActive(true);
                break;
            case AssetType.TEXT:
                textFrame.gameObject.SetActive(true);
                break;
        }
    }
}
