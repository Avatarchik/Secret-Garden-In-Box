using UnityEngine;
using System.Collections;
using OpenCVForUnity;
using System;

public class BasicPipeline {

    protected Mat outputMat;
    protected Mat inputMat;
    protected Color32[] imgColors;

    protected Texture2D outputTex;

    public BasicPipeline(int rows, int cols)
    {
        HandleOnInit(rows, cols);
    }

    protected virtual void HandleOnInit(int rows, int cols)
    {
        inputMat = new Mat(rows, cols, CvType.CV_8UC3);
        outputMat = new Mat(rows, cols, CvType.CV_8UC4);
        imgColors = new Color32[rows * cols];
        outputTex = new Texture2D(cols, rows, TextureFormat.RGBA32, false);
    }

    public Texture2D OnProcess(WebCamTexture srcTexture)
    {
        Utils.WebCamTextureToMat(srcTexture, inputMat, imgColors);
        HandleOnProcess();
        Utils.matToTexture2D(outputMat, outputTex, imgColors);
        return outputTex;
    }

    public Texture2D OnProcess(Texture2D srcTexture)
    {
        Utils.texture2DToMat(srcTexture, inputMat);
        HandleOnProcess();
        Utils.matToTexture2D(outputMat, outputTex, imgColors);
        return outputTex;
    }

    protected virtual void HandleOnProcess()
    {
        outputMat = inputMat.clone();
    }


}
