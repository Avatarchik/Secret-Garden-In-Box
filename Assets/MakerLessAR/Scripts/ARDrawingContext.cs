using UnityEngine;
using System.Collections;
using OpenCVForUnity;

public class ARDrawingContext {

    public bool isPatternPresent { set; get; }

    public Matrix4x4 patternPose { set; private get; }
    
    public float FieldOfView { private set; get; }

    public Texture2D BackgroundTexture { protected set; get; }

    public ARDrawingContext(int width, int height, Mat cameraMatrix)
    {
        lazyLookAtMatrix = new Lazy<Matrix4x4>(GetLookAtMatrix);
        lazyInvertZMatrix = new Lazy<Matrix4x4>(GetInvertZAxis);

        CalculateFOV(new Size(width, height), cameraMatrix);

        BackgroundTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

    }

    public void UpdateBackground(GameObject Canvas, Mat imageMat)
    {
        Utils.matToTexture2D(imageMat, BackgroundTexture);
        Canvas.GetComponent<Renderer>().material.mainTexture = BackgroundTexture;

    }

    public bool UpdateARCamera(Camera arCamera)
    {
        if (isPatternPresent)
        {
            arCamera.fieldOfView = FieldOfView;
            arCamera.worldToCameraMatrix = WorldToCameraMatrix;
        }

        return isPatternPresent;
    }

    void CalculateFOV(Size size, Mat cameraMatrix)
    {
        double apertureWidth = 0;
        double apertureHeight = 0;
        double[] fovx = new double[1];
        double[] fovy = new double[1];
        double[] focalLength = new double[1];
        Point principalPoint = new Point();
        double[] aspectratio = new double[1];
        
        Calib3d.calibrationMatrixValues(cameraMatrix, size, apertureWidth, apertureHeight, fovx, fovy, focalLength, principalPoint, aspectratio);

//         Debug.Log("imageSize " + size.ToString());
//         Debug.Log("apertureWidth " + apertureWidth);
//         Debug.Log("apertureHeight " + apertureHeight);
//         Debug.Log("fovx " + fovx[0]);
//         Debug.Log("fovy " + fovy[0]);
//         Debug.Log("focalLength " + focalLength[0]);
//         Debug.Log("principalPoint " + principalPoint.ToString());
//         Debug.Log("aspectratio " + aspectratio[0]);

        FieldOfView = (float)fovy[0];
    }
    
    Matrix4x4 WorldToCameraMatrix
    {
        get{

            return LookAtMatrix * patternPose * InvertZMatrix;
        }
    }

    Lazy<Matrix4x4> lazyLookAtMatrix;
    Lazy<Matrix4x4> lazyInvertZMatrix;
    
    Matrix4x4 LookAtMatrix { get { return lazyLookAtMatrix.Value; } }
    Matrix4x4 InvertZMatrix { get { return lazyInvertZMatrix.Value; } }

    //Marker Coordinate Initial Matrix
    Matrix4x4 GetLookAtMatrix()
    {

        Vector3 pos = new Vector3(0, 0, 0);
        Vector3 target = new Vector3(0, 0, 1);
        Vector3 up = new Vector3(0, -1, 0);

        Vector3 z = Vector3.Normalize(pos - target);
        Vector3 x = Vector3.Normalize(Vector3.Cross(up, z));
        Vector3 y = Vector3.Normalize(Vector3.Cross(z, x));

        Matrix4x4 result = new Matrix4x4();
        result.SetRow(0, new Vector4(x.x, x.y, x.z, -(Vector3.Dot(pos, x))));
        result.SetRow(1, new Vector4(y.x, y.y, y.z, -(Vector3.Dot(pos, y))));
        result.SetRow(2, new Vector4(z.x, z.y, z.z, -(Vector3.Dot(pos, z))));
        result.SetRow(3, new Vector4(0, 0, 0, 1));

        return result;
    }

    //OpenGL to Unity Coordinate System Convert Matrix
    //http://docs.unity3d.com/ScriptReference/Camera-worldToCameraMatrix.html 
    //that camera space matches OpenGL convention: camera's forward is the negative Z axis. 
    //This is different from Unity's convention, where forward is the positive Z axis. 
    Matrix4x4 GetInvertZAxis()
    {
        return Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
    }



}
