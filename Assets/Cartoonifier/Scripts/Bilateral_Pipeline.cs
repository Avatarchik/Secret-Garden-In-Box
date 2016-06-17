using UnityEngine;
using System.Collections;
using OpenCVForUnity;

public class Bilateral_Pipeline : BasicPipeline {

    //Mat grayMat, laplacianMat, maskMat;
    //float[] laplacianPixels;
    //byte[] maskPixels;
    
    const int MEDIAN_BLUR_FILLTER_SIZE = 7;
    const int LAPLACIAN_FILTER_SIZE = 5;
    const int EDGES_THRESHOLD = 8;
    
    public Bilateral_Pipeline(int rows, int cols) : base(rows, cols) { }

    protected override void HandleOnInit(int rows, int cols)
    {

        base.HandleOnInit(rows, cols);
        
    }
    
    protected override void HandleOnProcess()
    {
        
        Mat small = OnBilateralFilter(inputMat);

        Imgproc.resize(small, outputMat, inputMat.size(), 0, 0, Imgproc.INTER_LINEAR);

    }

    protected Mat OnBilateralFilter(Mat mask, int level = 1, int ksize = 8, double sigmaColor = 32, double sigmaSpace = 8)
    {
        Size size = inputMat.size();
        Size smallSize = new Size();
        int scale = (int)Mathf.Pow(2, level);
        smallSize.width = size.width / scale;
        smallSize.height = size.height / scale;
        Mat smallMat = new Mat(smallSize, CvType.CV_8UC3);
        Imgproc.resize(inputMat, smallMat, smallSize, 0, 0, Imgproc.INTER_LINEAR);

        Mat temp = new Mat(smallSize, CvType.CV_8UC3);
        int repetitions = 2;

        for (int i = 0; i < repetitions; i++)
        {

            Imgproc.bilateralFilter(smallMat, temp, ksize, sigmaColor, sigmaSpace);
            Imgproc.bilateralFilter(temp, smallMat, ksize, sigmaColor, sigmaSpace);
        }

        return smallMat;

    }

}
