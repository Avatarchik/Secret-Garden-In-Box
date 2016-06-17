using UnityEngine;
using System.Collections;
using OpenCVForUnity;

public class ARPipeline {

    CameraCalibration m_calibration;
    public Pattern m_pattern = new Pattern();
    public PatternTrackingInfo m_patternInfo;// = new PatternTrackingInfo();
    public PatternDetector m_patternDetector = new PatternDetector();

    public ARPipeline(Mat patternImage, CameraCalibration calibration)
    {
        m_calibration = calibration;
        m_patternDetector.BuildPatternFromImage(patternImage, m_pattern);
        m_patternDetector.Train(m_pattern);

    }

    public bool ProcessFrame(Mat inputFrame)
    {
        Debug.Log("ProcessFrame ... ");
        bool isFound = m_patternDetector.FindPattern(inputFrame, out m_patternInfo);

        if (isFound)
        {
           // Debug.Log("patternFound is true.");
            m_patternInfo.ComputePose(m_pattern, m_calibration);
            //m_patternInfo.Draw2dContour(inputFrame, new Scalar(255, 0, 0, 255));
        }
        return isFound;
    }

    public Matrix4x4 GetPatternLocation() {
        return m_patternInfo.pose3d;
    }


    
}
