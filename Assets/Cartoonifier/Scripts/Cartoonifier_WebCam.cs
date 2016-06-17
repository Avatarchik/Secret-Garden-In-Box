using UnityEngine;
using System.Collections;
using System;
using OpenCVForUnity;

public class Cartoonifier_WebCam : BasicWebCamBuilder
{
    public Renderer canvasRenderer;

    Bilateral_Pipeline bilateral_pipeline;

    Outline_Pipeline outline_pipeline;

    FloodFill_Pipeline floodFill_pipeline;

    Vector3 position;// = Vector3.zero;

    protected override bool HandleOnInit()
    {
        bilateral_pipeline = new Bilateral_Pipeline(webCamTexture.height, webCamTexture.width);
        //canvasRenderer.materials[0].mainTexture = pipeline.OnProcess(originalTexture);

        outline_pipeline = new Outline_Pipeline(webCamTexture.height, webCamTexture.width);
        //canvasRenderer.materials[1].mainTexture = putlinePipeline.OnProcess(originalTexture);

        floodFill_pipeline = new FloodFill_Pipeline(webCamTexture.height, webCamTexture.width);
        

        return true;

    }
    
    protected override void HandleOnUpdate()
    {
        //canvasRenderer.materials[0].mainTexture = bilateral_pipeline.OnProcess(webCamTexture);
        canvasRenderer.materials[1].mainTexture = outline_pipeline.OnProcess(webCamTexture);
        //Utils.matToTexture2D(outlineMat, outputTexture, imgColors);

        if (Input.GetMouseButtonDown(0))
        {
            position = Screen2TextureMatrix.MultiplyPoint3x4(Input.mousePosition);
            floodFill_pipeline.OnProcess(webCamTexture);
            canvasRenderer.materials[0].mainTexture = floodFill_pipeline.HandleOnFloodFill(position);
        }
        
    }

    

   

    Mat ChangeFacialSkinColor(Mat smallImgBGR, Mat bigEdges, int debugType)
    {
        // Convert to Y'CrCb color-space, since it is better for skin detection and color adjustment.
        Mat yuv = new Mat(smallImgBGR.size(), CvType.CV_8UC3);
        Imgproc.cvtColor(smallImgBGR, yuv, Imgproc.COLOR_BGR2YCrCb);

        // The floodFill mask has to be 2 pixels wider and 2 pixels taller than the small image.
        // The edge mask is the full src image size, so we will shrink it to the small size,
        // storing into the floodFill mask data.
        int sCols = smallImgBGR.cols();
        int sRows = smallImgBGR.rows();

        Mat maskPlusBorder = Mat.zeros(sRows + 2, sCols + 2, CvType.CV_8U);
        Mat mask = maskPlusBorder.submat(1, sRows, 1, sCols);

        Imgproc.resize(bigEdges, mask, smallImgBGR.size());

        // Make the mask values just 0 or 255, to remove weak edges.
        Imgproc.threshold(mask, mask, 80, 255, Imgproc.THRESH_BINARY);

        // Connect the edges together, if there was a pixel gap between them.

        for(int i=0; i< 2; i++)
        {
            Imgproc.dilate(mask, mask, new Mat());
            Imgproc.erode(mask, mask, new Mat());
        }

        // YCrCb Skin detector and color changer using multiple flood fills into a mask.
        // Apply flood fill on many points around the face, to cover different shades & colors of the face.
        // Note that these values are dependent on the face outline, drawn in drawFaceStickFigure().
        int NUM_SKIN_POINTS = 6;
        Point[] skinPts = new Point[NUM_SKIN_POINTS];
        skinPts[0] = new Point(sCols / 2, sRows / 2 - sRows / 6);
        skinPts[1] = new Point(sCols / 2 - sCols / 11, sRows / 2 - sRows / 6);
        skinPts[2] = new Point(sCols / 2 + sCols / 11, sRows / 2 - sRows / 6);
        skinPts[3] = new Point(sCols / 2, sRows / 2 + sRows / 16);
        skinPts[4] = new Point(sCols / 2 - sCols / 9, sRows / 2 + sRows / 16);
        skinPts[5] = new Point(sCols / 2 + sCols / 9, sRows / 2 + sRows / 16);

        // Skin might be fairly dark, or slightly less colorful.
        // Skin might be very bright, or slightly more colorful but not much more blue.
        const int LOWER_Y = 60;
        const int UPPER_Y = 80;
        const int LOWER_Cr = 25;
        const int UPPER_Cr = 15;
        const int LOWER_Cb = 20;
        const int UPPER_Cb = 15;
        Scalar lowerDiff = new Scalar(LOWER_Y, LOWER_Cr, LOWER_Cb);
        Scalar upperDiff = new Scalar(UPPER_Y, UPPER_Cr, UPPER_Cb);

        // Instead of drawing into the "yuv" image, just draw 1's into the "maskPlusBorder" image, so we can apply it later.
        // The "maskPlusBorder" is initialized with the edges, because floodFill() will not go across non-zero mask pixels.
        Mat edgeMask = mask.clone();    // Keep an duplicate copy of the edge mask.
        const int flags = 4 | Imgproc.FLOODFILL_FIXED_RANGE | Imgproc.FLOODFILL_MASK_ONLY;

        //Imgproc.floodFill(yuv, maskPlusBorder, skinPts[1], new Scalar(0), null, lowerDiff, upperDiff, flags);


        for (int i = 0; i < 2; i++)
        {
            // Use the floodFill() mode that stores to an external mask, instead of the input image.
            Imgproc.floodFill(yuv, maskPlusBorder, skinPts[i], new Scalar(0), null, lowerDiff, upperDiff, flags);
            if (debugType >= 1)
                Core.circle(smallImgBGR, skinPts[i], 1, new Scalar(0, 0, 255), 1);
        }
        //         if (debugType >= 2)
        //             imshow("flood mask", mask * 120); // Draw the edges as white and the skin region as grey.

        //mask -= edgeMask;



        //Core.add(smallImgBGR, new Scalar(0, 70, 0), smallImgBGR, mask);

        return mask;
    }
}
