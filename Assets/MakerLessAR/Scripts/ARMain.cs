using UnityEngine;
using System.Collections;
using OpenCVForUnity;

public class ARMain
{

    GameObject Canvas;
    Camera[] ARCameras;

    CameraCalibration cameraCalibration;

    ARDrawingContext drawingCtx;
    ARPipeline pipeline;

    public ARMain(GameObject canvas, params Camera[] arCameras)
    {
        Canvas = canvas;
        ARCameras = arCameras;
    }
    
    public void CalibrateCamera(Mat patternMat, Mat inputMat)
    {
         int width = inputMat.cols();
         int height = inputMat.rows();

        int max_d = Mathf.Max(width, height);
        float cx = width / 2.0f;
        float cy = height / 2.0f;

        cameraCalibration = new CameraCalibration(max_d, max_d, cx, cy);

        drawingCtx = new ARDrawingContext(width, height, cameraCalibration.GetIntrinsic());

        pipeline = new ARPipeline(patternMat, cameraCalibration);

        Canvas.transform.localScale = new Vector3(width, height, 1);
        Camera.main.orthographicSize = height * 0.5f;

    }
    
    public void ProcessSingleImage(Mat patternMat, Mat inputMat)
    {

        //Debug.Log("ProcessSingleImage ");
        
        CalibrateCamera(patternMat, inputMat);
        
        ProcessSingleFrame(inputMat);
        
        //Debug.Log("Done ");

    }
    
    public void ProcessSingleFrame(Mat captureMat)
    {
        
        drawingCtx.isPatternPresent = pipeline.ProcessFrame(captureMat);

        drawingCtx.patternPose = pipeline.GetPatternLocation();

        /*pipeline.m_patternInfo.Draw2dContour(captureMat, new Scalar(255, 0, 0, 255));

        drawingCtx.UpdateBackground(Canvas, captureMat);*/

        drawingCtx.UpdateARCamera(ARCameras[0]);
    }
    
        
}
