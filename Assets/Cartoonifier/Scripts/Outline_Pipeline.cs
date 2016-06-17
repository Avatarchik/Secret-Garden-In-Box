using UnityEngine;
using System.Collections;
using OpenCVForUnity;
using System;

public class Outline_Pipeline : BasicPipeline
{ 
    
    Mat grayMat, laplacianMat, maskMat;
    float[] laplacianPixels;
    byte[] maskPixels;

    const int MEDIAN_BLUR_FILLTER_SIZE = 7;
    const int LAPLACIAN_FILTER_SIZE = 5;
    const int EDGES_THRESHOLD = 4;
    
    Color32[] outputColors;
    byte[] outputPixels;

    //int mRows, mCols;

    public Outline_Pipeline(int rows, int cols) : base(rows, cols)
    {
    }

    protected override void HandleOnInit(int rows, int cols)
    {
        /*inputMat = new Mat(rows, cols, CvType.CV_8UC3);
        outputMat = new Mat(rows, cols, CvType.CV_8UC4);
        imgColors = new Color32[rows * cols];*/

        base.HandleOnInit(rows, cols);

        grayMat = new Mat(rows, cols, CvType.CV_8UC1);
        laplacianMat = new Mat(rows, cols, CvType.CV_32FC1);
        maskMat = new Mat(rows, cols, CvType.CV_8UC1);

        //laplacianPixels = new float[rows * cols * laplacianMat.channels()];
        maskPixels = new byte[rows * cols * maskMat.channels()];

        //outputTex = new Texture2D(cols, rows, TextureFormat.RGBA32, false);
        outputPixels = new byte[rows * cols * outputMat.channels()];
        outputColors = new Color32[rows * cols];
    }

    protected override void HandleOnProcess()
    {
        Imgproc.cvtColor(inputMat, grayMat, Imgproc.COLOR_BGR2GRAY);

        Imgproc.medianBlur(grayMat, grayMat, MEDIAN_BLUR_FILLTER_SIZE);
        //Imgproc.GaussianBlur(grayMat, grayMat, new Size(3, 3), 0);

        Imgproc.Laplacian(grayMat, laplacianMat, LAPLACIAN_FILTER_SIZE);

        //Imgproc.threshold(laplacianMat, laplacianMat, EDGES_THRESHOLD, 255, Imgproc.THRESH_BINARY_INV);

        //laplacianMat.convertTo(outputMat, CvType.CV_8U);

        OnUnitTest(EDGES_THRESHOLD);
    }

    void OnUnitTest(float threshold = 1.0f)
    {
        for (int i = 0; i < 8; i++)
        {
            Imgproc.dilate(laplacianMat, laplacianMat, new Mat());
            Imgproc.erode(laplacianMat, laplacianMat, new Mat());
        }

        laplacianPixels = new float[inputMat.rows() * inputMat.cols() * laplacianMat.channels()];
        

        Core.MinMaxLocResult res = Core.minMaxLoc(laplacianMat);
        var scale = 255 / res.maxVal;// Math.Max(-res.minVal, res.maxVal);

        //laplacianMat.convertTo(laplacianMat, CvType.CV_32F, scale, 128);
        Debug.Log(laplacianMat);

        laplacianMat.get(0, 0, laplacianPixels);

        Debug.Log(string.Format("Mat : min : {0}; max : {1}; scale : {2}", res.minVal, res.maxVal, scale));

        /*Debug.Log("output length : " + outputPixels.Length);

        Debug.Log("laplacian length : " + laplacianPixels.Length);*/

        //threshold *= -1.0f;
        float min = 0;
        float max = 0;
        for (int i = inputMat.cols(); i < laplacianPixels.Length - inputMat.cols(); i++)
        {
            outputPixels[4 * i + 3] = 0;
            //Debug.Log(outputPixels[i]);
            var value = laplacianPixels[i];
            if (min > value) min = value;
            if (max < value) max = value;

            //var value = Math.Abs(laplacianPixels[i]);
            
            var current = laplacianPixels[i];
            var left = laplacianPixels[i - 1];
            var up = laplacianPixels[i - inputMat.cols()];

            if(value > threshold)
            {
                outputPixels[4 * i + 3] = (byte)(value * scale);// 0xFF;
//                 if(value < -threshold)
/*                     outputPixels[4 * i ] = outputPixels[4 * i + 1] = outputPixels[4 * i + 2] = 255;*/
            }
            
        }
        Debug.Log(string.Format("Picels : min : {0}; max : {1}", min, max));
        outputMat.put(0, 0, outputPixels);

        

    }

    
}
