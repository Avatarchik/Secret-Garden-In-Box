using UnityEngine;
using System.Collections;
using OpenCVForUnity;

public class CameraCalibration {

    public CameraCalibration() { }
    
    public CameraCalibration(float _fx, float _fy, float _cx, float _cy) {

        m_intrinsic = Mat.zeros(3, 3, CvType.CV_64FC1);

        fx = _fx;
        fy = _fy;
        cx = _cx;
        cy = _cy;
        m_intrinsic.put(2, 2, 1.0f);

        //double[] distortions = new double[5];
        m_distortion = new MatOfDouble(0,0,0,0);

    }
    public CameraCalibration(float _fx, float _fy, float _cx, float _cy, float[] distorsionCoeff) {
        m_intrinsic = Mat.zeros(3, 3, CvType.CV_64FC1);

        fx = _fx;
        fy = _fy;
        cx = _cx;
        cy = _cy;
        m_intrinsic.put(2, 2, 1.0f);

        double[] distortions = new double[5];
        for(int i = 0; i<5; i++)
        {
            distortions[i] = distorsionCoeff[i];
        }

        m_distortion = new MatOfDouble(distortions);

    }

    public void GetMatrix34(float[][] cparam) { }

    public Mat GetIntrinsic() { return m_intrinsic; }
    public MatOfDouble GetDistorsion() { return m_distortion; }

    public double fx
    {
        set { m_intrinsic.put(1, 1, value); }
        get { return m_intrinsic.get(1, 1)[0]; }
    }

    public double fy
    {
        set { m_intrinsic.put(0, 0, value); }
        get { return m_intrinsic.get(0, 0)[0]; }
    }

    public double cx
    {
        set { m_intrinsic.put(0, 2, value); }
        get { return m_intrinsic.get(0, 2)[0]; }
    }

    public double cy
    {
        set { m_intrinsic.put(1, 2, value); }
        get { return m_intrinsic.get(1, 2)[0]; }
    }

    Mat m_intrinsic;
    MatOfDouble m_distortion;

}
