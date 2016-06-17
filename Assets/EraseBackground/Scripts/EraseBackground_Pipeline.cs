using UnityEngine;
using System.Collections;
using OpenCVForUnity;

public class EraseBackground_Pipeline : BasicPipeline
{
    public EraseBackground_Pipeline(int rows, int cols) : base(rows, cols) { }

    public Texture2D HandleOnErase(Vector2 position)
    {

        Mat yuv = new Mat(inputMat.size(), CvType.CV_8UC3);
        Imgproc.cvtColor(inputMat, yuv, Imgproc.COLOR_BGR2YCrCb);

        Debug.Log(yuv);

        int cols = inputMat.cols();
        int rows = inputMat.rows();

        Mat maskPlusBotder = Mat.zeros(rows + 2, cols + 2, CvType.CV_8U);
        Mat mask = maskPlusBotder.submat(new OpenCVForUnity.Rect(1, 1, cols, rows));

        const int LOWER_Y = 60;
        const int UPPER_Y = 80;
        const int LOWER_Cr = 25;
        const int UPPER_Cr = 15;
        const int LOWER_Cb = 20;
        const int UPPER_Cb = 15;

        Scalar lowerDiff = new Scalar(LOWER_Y, LOWER_Cr, LOWER_Cb);
        Scalar upperDiff = new Scalar(UPPER_Y, UPPER_Cr, UPPER_Cb);

        const int flags = 4 | Imgproc.FLOODFILL_FIXED_RANGE | Imgproc.FLOODFILL_MASK_ONLY;

        var point = new Point(position.x, position.y);

        Imgproc.floodFill(yuv, maskPlusBotder, point, new Scalar(0), null, lowerDiff, upperDiff, flags);

        Core.add(inputMat, new Scalar(0, 70, 0), outputMat, mask);

        Core.circle(outputMat, point, 8, new Scalar(255, 0, 0));

        Utils.matToTexture2D(outputMat, outputTex, imgColors);

        return outputTex;
    }
}
