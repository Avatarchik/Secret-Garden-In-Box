using UnityEngine;
using System.Collections;
using System;
using OpenCVForUnity;

public class WebCamFeatureMatRenderer : MatRenderer
{
    WebCamTexture mWebCamTexture;

    public WebCamFeatureMatRenderer(WebCamTexture src) : base(src) {
        mWebCamTexture = src;
    }

    public WebCamFeatureMatRenderer(WebCamTexture src, params Action[] callback):base(src, callback) {
        mWebCamTexture = src;
    }

    protected override void OnPreProcess()
    {
        Utils.WebCamTextureToMat(mWebCamTexture, rgbaMat, imgColors);
    }

    protected override void OnProcess()
    {
        //throw new NotImplementedException();
    }

    protected override void OnPostProcess()
    {
        Utils.matToTexture2D(rgbaMat, destTexture, imgColors);
    }

}
