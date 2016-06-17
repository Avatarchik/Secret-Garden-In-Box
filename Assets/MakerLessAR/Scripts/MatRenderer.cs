using UnityEngine;
using System.Collections.Generic;
using System;
using OpenCVForUnity;

public abstract class MatRenderer{
    
    protected Mat grayMat;
    protected Mat binaryMat;
    protected Color32[] imgColors;

    protected Texture srcTexture;
   
    protected List<Action> mCallbacks;

    public Mat rgbaMat { protected set; get; }
    public Texture2D destTexture { protected set; get; }

    public int Rows
    {
        get
        {
            return rgbaMat == null ? 0 : rgbaMat.rows();
        }
    }

    public int Cols
    {
        get
        {
            return rgbaMat == null ? 0 : rgbaMat.cols();
        }
    }

    public MatRenderer(Texture src, params Action[] callback) : this(src)
    {
        if (mCallbacks == null)
            mCallbacks = new List<Action>();
        mCallbacks.AddRange(callback);
    }

    public MatRenderer(Texture src)
    {
        OnInit(src.width, src.height);
        srcTexture = src;
    }

   /* public MatRenderer(int width, int height)
    {
        OnInit(width, height);
    }*/

    protected virtual void OnInit(int width, int height)
    {
        destTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        imgColors = new Color32[width * height];

        rgbaMat = new Mat(height, width, CvType.CV_8UC4);

        grayMat = new Mat(height, width, CvType.CV_8UC1);

        binaryMat = new Mat(height, width, CvType.CV_8UC1);

    }

    public virtual void OnDispose()
    {
        if (rgbaMat != null)
            rgbaMat.Dispose();
        if (grayMat != null)
            grayMat.Dispose();
        if (binaryMat != null)
            binaryMat.Dispose();
    }

    public void OnRender()
    {
        /*if (srcTexture == null)
        {
            return;
        }*/

        OnPreProcess();

        OnProcess();

        OnCallback();

        OnPostProcess();
    }

    protected abstract void OnPreProcess();
    protected abstract void OnProcess();
    protected abstract void OnPostProcess();

    void OnCallback()
    {
        if (mCallbacks != null)// && mCallbacks.Count > 0)
        {
            foreach (var act in mCallbacks)
            {
                act();
            }
        }
    }

    public void DrawPoints(Mat canvas, Scalar color, IEnumerable<Point> points)
    {
        foreach (var p in points)
        {

            Core.circle(canvas, new Point(p.x, p.y), 2, color, 2, Core.LINE_AA, 0);

        }
    }

    public void DrawPoints(Mat canvas, params Point[] points)
    {
        foreach (var p in points)
        {

            Core.circle(canvas, new Point(p.x, p.y), 1, new Scalar(255, 0, 0, 255), 2, Core.LINE_AA, 0);

        }
    }

    protected void DrawLines(Mat canvas, params Point[] points)
    {

        for (int i = 0; i < points.Length; i++)
        {

            Core.line(canvas, points[i], points[(i + 1) / points.Length], new Scalar(255, 0, 0, 255));

        }
    }

}
