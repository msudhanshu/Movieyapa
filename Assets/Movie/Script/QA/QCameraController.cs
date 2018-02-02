using UnityEngine;
using System.Collections;

public class QCameraController : Manager<QCameraController>
{

    public Camera main3dmenuCamera;
    public Camera imageCamera;
    public Camera gifCamera;
    public Camera musicCamera;

    override public void PopulateDependencies() { }

    override public void StartInit()
    {

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoToMainMenuCamera()
    {
        gifCamera.gameObject.SetActive(false);
        musicCamera.gameObject.SetActive(false);
        imageCamera.gameObject.SetActive(false);
        main3dmenuCamera.gameObject.SetActive(true);
    }

    public void CloseMainMenuCamera()
    {
        main3dmenuCamera.gameObject.SetActive(false);
    }

    public void SetQCamera(AssetType qType)
    {
        main3dmenuCamera.gameObject.SetActive(false);
        switch (qType)
        {
            case AssetType.IMAGE:
                gifCamera.gameObject.SetActive(false);
                musicCamera.gameObject.SetActive(false);
                imageCamera.gameObject.SetActive(true);
                break;
            case AssetType.GIF:
            case AssetType.SPRITE:
            case AssetType.SLIDESHOW:
                musicCamera.gameObject.SetActive(false);
                imageCamera.gameObject.SetActive(false);
                gifCamera.gameObject.SetActive(true);
                break;
            case AssetType.MUSIC:
                gifCamera.gameObject.SetActive(false);
                imageCamera.gameObject.SetActive(false);
                musicCamera.gameObject.SetActive(true);
                break;
        }
    }
}
