using UnityEngine;
using System.Collections;
using System;
using OpenCVForUnity;

public class ARForWebCamTest : BasicWebCamBuilder {

    public Camera[] ARCameras;
    public GameObject canvasObj;


    public Texture2D patternTex;

    Mat patternMat;
    Mat captureMat;
    Color32[] imgColors;

    ARMain main;
    protected override bool HandleOnInit()
    {

        patternMat = new Mat(patternTex.height, patternTex.width, CvType.CV_8UC4);
        Utils.texture2DToMat(patternTex, patternMat);

        captureMat = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4);
        imgColors = new Color32[webCamTexture.height * webCamTexture.width];

        main = new ARMain(canvasObj, ARCameras);
        main.CalibrateCamera(patternMat, captureMat);

        Debug.Log("AR Test Mono HandleOnInit");

        //StartCoroutine(HandleOnLoop());

        return true;
    }

    protected override void HandleOnUpdate()
    {
        //canvasObj.renderer.material.mainTexture = webCamTexture;
        //main.ProcessVideo(patternTexture, webCamTexture);

        canvasObj.GetComponent<Renderer>().material.mainTexture = webCamTexture;

        /*Utils.WebCamTextureToMat(webCamTexture, captureMat, imgColors);
         main.ProcessSingleFrame(captureMat);*/
    }

    
}
