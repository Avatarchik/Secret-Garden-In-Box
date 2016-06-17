using UnityEngine;
using System.Collections.Generic;
using OpenCVForUnity;

public class Pattern {

    public Size size;
    public Mat frame;
    public Mat grayImg;
    
    public MatOfPoint2f points2d;
    public MatOfPoint3f points3d;

    public MatOfKeyPoint keypoints;
    public Mat descriptors;

    public override string ToString()
    {
        return string.Format("Pattern : Size : {0},\n Frame {1},\n Gray {2},\n KeyPoint {3}\n Descri {4}", 
            size.ToString(), frame.ToString(), grayImg.ToString(), keypoints.ToString(), descriptors.ToString());
    }
}

public class PatternTrackingInfo
{
    public Mat homography;
    public MatOfPoint2f points2d = new MatOfPoint2f();
    public Matrix4x4 pose3d = new Matrix4x4();
    //public bool isFound;

    public PatternTrackingInfo()
    {
//         homography = new Mat();
//         points2d = new MatOfPoint2f();
//         pose3d = new Matrix4x4();
    }

    public void Draw2dContour(Mat image, Scalar color)
    {
        Point[] pts = points2d.toArray();

        if (pts.Length != 4)
            return;

        int thickness = 2;

        Core.line(image, pts[0], pts[1], color, thickness, Core.LINE_AA, 0);
        Core.line(image, pts[1], pts[2], color, thickness, Core.LINE_AA, 0);
        Core.line(image, pts[2], pts[3], color, thickness, Core.LINE_AA, 0);
        Core.line(image, pts[3], pts[0], color, thickness, Core.LINE_AA, 0);
        
    }

    public void ComputePose(Pattern pattern, CameraCalibration calibration)
    {
        Mat Rvec = new Mat();
        Mat Tvec = new Mat();
        Mat raux = new Mat();
        Mat taux = new Mat();
        //Calib3d.solvePnP(m_markerCorners3d, new MatOfPoint2f(m.points.toArray()), camMatrix, distCoeffs, raux, taux);

        Debug.Log(string.Format("Compute Pose : {0},\n{1},\n{2},\n{3},\n{4}", 
            pattern.points3d.ToString(), 
            points2d.ToString(), 
            calibration.GetIntrinsic().ToString(), 
            calibration.GetDistorsion().ToString(),
            homography.ToString()));

        Calib3d.solvePnP(pattern.points3d, points2d, calibration.GetIntrinsic(), calibration.GetDistorsion(), raux, taux);

        if (taux == null || taux.empty())
            Debug.Log("taux is null");

        raux.convertTo(Rvec, CvType.CV_32F);
        taux.convertTo(Tvec, CvType.CV_32F);

        Mat rotMat = new Mat(3, 3, CvType.CV_64FC1);
        Calib3d.Rodrigues(Rvec, rotMat);

        if (pose3d == null)
            Debug.Log("pose3d is null");
        if (rotMat == null || rotMat.empty())
            Debug.Log("rotMat is null");
        if (Tvec == null || Tvec.empty())
            Debug.Log("Tvec is null");

        pose3d.SetRow(0, new Vector4((float)rotMat.get(0, 0)[0], (float)rotMat.get(0, 1)[0], (float)rotMat.get(0, 2)[0], (float)Tvec.get(0, 0)[0]));
        pose3d.SetRow(1, new Vector4((float)rotMat.get(1, 0)[0], (float)rotMat.get(1, 1)[0], (float)rotMat.get(1, 2)[0], (float)Tvec.get(1, 0)[0]));
        pose3d.SetRow(2, new Vector4((float)rotMat.get(2, 0)[0], (float)rotMat.get(2, 1)[0], (float)rotMat.get(2, 2)[0], (float)Tvec.get(2, 0)[0]));
        pose3d.SetRow(3, new Vector4(0, 0, 0, 1));

        //pose3d = Matrix4x4.Inverse(pose3d);

        Rvec.Dispose();
        Tvec.Dispose();
        raux.Dispose();
        taux.Dispose();
        rotMat.Dispose();

    }
}
