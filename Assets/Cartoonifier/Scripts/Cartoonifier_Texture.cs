using UnityEngine;
using System.Collections;
using OpenCVForUnity;

public class Cartoonifier_Texture : MonoBehaviour {


    public Texture2D srcTexture;

    public Transform canvasTransform;
    public Renderer canvasRenderer;

    public Camera orthographicCamera;

    Bilateral_Pipeline bilateralPipeline;

    Outline_Pipeline outlinePipeline;

    FloodFill_Pipeline floodFillPipeline;

    Matrix4x4 Screen2TextureMatrix;

	// Use this for initialization
	void Start () {
        OnUpdateCanvasScale();
        OnUpdateCameraSize();

//         bilateralPipeline = new Bilateral_Pipeline(srcTexture.height, srcTexture.width);
//         canvasRenderer.materials[0].mainTexture = bilateralPipeline.OnProcess(srcTexture);

        outlinePipeline = new Outline_Pipeline(srcTexture.height, srcTexture.width);
        canvasRenderer.materials[1].mainTexture = outlinePipeline.OnProcess(srcTexture);
        
        floodFillPipeline = new FloodFill_Pipeline(srcTexture.height, srcTexture.width);
        canvasRenderer.materials[0].mainTexture = floodFillPipeline.OnProcess(srcTexture);

    }
	
	// Update is called once per frame
	void Update () {

        if (floodFillPipeline == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(Input.mousePosition);
            //var pos = new Vector3(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 0);//Screen.height - 
            var pos = Screen2TextureMatrix.MultiplyPoint3x4(Input.mousePosition); //new Vector3(pos.x + delta_x, pos.y, 0);
            Debug.Log("pos : " + pos);
            canvasRenderer.materials[0].mainTexture = floodFillPipeline.HandleOnFloodFill(pos);
        }
	}

    void OnUpdateCanvasScale()
    {
        if (srcTexture == null)
            return;
        canvasTransform.localRotation = Quaternion.identity;
        canvasTransform.localScale = new Vector3(srcTexture.width, srcTexture.height, 1);

    }

    void OnUpdateCameraSize()
    {
        if (orthographicCamera == null || srcTexture == null)
            return;

        if (!orthographicCamera.orthographic)
            orthographicCamera.orthographic = true;
        float widthScale = (float)Screen.width / srcTexture.width;
        float heightScale = (float)Screen.height / srcTexture.height;

        /*var texture_ratio = srcTexture.height / srcTexture.width;
        var screen_ratio = Screen.height / Screen.width;*/


        float delta_y = 0.5f * (srcTexture.height * widthScale - Screen.height) ;
        float delta_x = 0.5f * (srcTexture.width * heightScale - Screen.width) ;
        
        var trans = Vector3.zero;
        var scale = new Vector3(1, -1, 1);//bottom_left to top_left

        if (widthScale > heightScale)
        {
            
            
            Camera.main.orthographicSize = (srcTexture.width * (float)Screen.height / (float)Screen.width) * 0.5f;

            trans = new Vector3(0, - Screen.height - delta_y, 0);
            scale /= widthScale;
        }
        else
        {
            
            Camera.main.orthographicSize = srcTexture.height / 2;

            trans = new Vector3(delta_x, -Screen.height, 0);
            scale /= heightScale;
        }

        Screen2TextureMatrix = Matrix4x4.Scale(scale);
        Screen2TextureMatrix *= Matrix4x4.TRS(trans, Quaternion.identity, Vector3.one);

    }


    void HandleOnInit()
    {

    }

    void HandleOnUpdate()
    {

    }

}
