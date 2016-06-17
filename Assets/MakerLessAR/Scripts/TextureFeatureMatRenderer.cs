using UnityEngine;
using System.Collections;
using System;
using OpenCVForUnity;
using System.Linq;

public class TextureFeatureMatRenderer : MatRenderer
{
    

    ARPipeline arPipeline;
    CameraCalibration mCalibration;

    bool isPatternPresent = false;
    public Matrix4x4 patternPose;
    public float fieldOfView;

    Texture2D mPatternImg;
    Mat mPatternMat;
    Texture2D mInputImg;

    public TextureFeatureMatRenderer(Texture2D inputImg)
        : base(inputImg) {
        mInputImg = inputImg;
    }

    public TextureFeatureMatRenderer(Texture2D inputImg, params Action[] callback)
        : base(inputImg, callback) {
        mInputImg = inputImg;
    }

    protected override void OnInit(int width, int height)
    {
        base.OnInit(width, height);

        int max_d = Mathf.Max(width, height);
        float cx = width / 2.0f;
        float cy = height / 2.0f;

        mCalibration = new CameraCalibration(max_d, max_d, cx, cy);

        Size imageSize = new Size(width, height);
        double apertureWidth = 0;
        double apertureHeight = 0;
        double[] fovx = new double[1];
        double[] fovy = new double[1];
        double[] focalLength = new double[1];
        Point principalPoint = new Point();
        double[] aspectratio = new double[1];

        Calib3d.calibrationMatrixValues(mCalibration.GetIntrinsic(), imageSize, apertureWidth, apertureHeight, fovx, fovy, focalLength, principalPoint, aspectratio);

        Debug.Log("imageSize " + imageSize.ToString());
        Debug.Log("apertureWidth " + apertureWidth);
        Debug.Log("apertureHeight " + apertureHeight);
        Debug.Log("fovx " + fovx[0]);
        Debug.Log("fovy " + fovy[0]);
        Debug.Log("focalLength " + focalLength[0]);
        Debug.Log("principalPoint " + principalPoint.ToString());
        Debug.Log("aspectratio " + aspectratio[0]);

        fieldOfView = (float)fovy[0];

        mPatternImg = Resources.Load<Texture2D>("PyramidPattern");//("marker");//
        mPatternMat = new Mat(mPatternImg.height, mPatternImg.width, CvType.CV_8UC4);
        Utils.texture2DToMat(mPatternImg, mPatternMat);

        arPipeline = new ARPipeline(mPatternMat, mCalibration);

        

    }

    protected override void OnPreProcess()
    {
        Utils.texture2DToMat(mInputImg, rgbaMat);

        
    }

    protected override void OnProcess()
    {

        // Convert the image to grayscale
        //Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_BGRA2GRAY);

        //
        isPatternPresent = arPipeline.ProcessFrame(rgbaMat);
        arPipeline.m_patternInfo.Draw2dContour(rgbaMat, new Scalar(255, 0, 0, 255));
        //DrawPoints(rgbaMat, new Scalar(255, 0, 0, 255), arPipeline.m_pattern.keypoints.toArray().Select(k => k.pt));
        //DrawPoints(rgbaMat, new Scalar(0, 0, 255, 255), arPipeline.m_patternDetector.m_queryKeypoints.toArray().Select(k => k.pt));


        //throw new NotImplementedException();

    }

    protected override void OnPostProcess()
    {

        Utils.matToTexture2D(rgbaMat, destTexture);

        if (isPatternPresent)
        {

            patternPose = arPipeline.GetPatternLocation();
            //arPipeline.m_patternInfo.Draw2dContour(rgbaMat, new Scalar(255, 0, 0, 255));

            foreach(var pt in arPipeline.m_patternInfo.points2d.toArray())
            {
                Debug.Log("Point 2D : " + pt.ToString());
            }

            Debug.Log(patternPose);
        }
        //throw new NotImplementedException();
    }

}
