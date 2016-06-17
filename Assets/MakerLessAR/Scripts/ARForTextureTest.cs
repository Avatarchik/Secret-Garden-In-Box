using UnityEngine;
using System.Collections;
using OpenCVForUnity;

public class ARForTextureTest : MonoBehaviour {

    public Camera[] ARCameras;
    public GameObject canvasObj;

    public Texture2D patternTex;
    public Texture2D inputTex;

    Mat patternMat;
    Mat inputMat;
    
	void Start () {

        ARMain main = new ARMain(canvasObj, ARCameras);
        Debug.Log("main ");

        patternMat = new Mat(patternTex.height, patternTex.width, CvType.CV_8UC4);
        Utils.texture2DToMat(patternTex, patternMat);

        inputMat = new Mat(inputTex.height, inputTex.width, CvType.CV_8UC4);
        Utils.texture2DToMat(inputTex, inputMat);

        //renderer.material.mainTexture = inputTex;

        main.ProcessSingleImage(patternMat, inputMat);
    }
    
    // Update is called once per frame
//     void Update () {
// 	
// 	}
}
