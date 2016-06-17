using UnityEngine;
using System.Collections;

public abstract class BasicWebCamBuilder : MonoBehaviour
{

    protected WebCamTexture webCamTexture;

    public int width = 640;
    public int height = 480;

    protected bool isInitDone = false;

    protected Matrix4x4 Screen2TextureMatrix;

    //MatRenderer matRenderer;

//#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		/// <summary>
		/// The is front.
		/// </summary>
	public bool shouldUseFrontFacing = true;
    //#endif

    // Use this for initialization
    protected void Start()
    {


        StartCoroutine(OnInit());

       
    }

    void OnDisable()
    {
        webCamTexture.Stop();
    }


    protected IEnumerator OnInit()
    {
        if (webCamTexture != null)
        {
//             if (matRenderer != null)
//                 matRenderer.OnDispose();

            webCamTexture.Stop();
            isInitDone = false;

        }

        foreach (var device in WebCamTexture.devices)
        {
//#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
            if(device.isFrontFacing == shouldUseFrontFacing){
//#endif
            webCamTexture = new WebCamTexture(device.name, width, height);
//#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
                break;
            }
//#endif
        }

        if (webCamTexture == null)
        {
            webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, width, height);
        }

        webCamTexture.Play();

        while (true)
        {
            //If you want to use webcamTexture.width and webcamTexture.height on iOS, you have to wait until webcamTexture.didUpdateThisFrame == 1, otherwise these two values will be equal to 16. (http://forum.unity3d.com/threads/webcamtexture-and-error-0x0502.123922/)
#if UNITY_IOS && !UNITY_EDITOR && (UNITY_4_6_3 || UNITY_4_6_4 || UNITY_5_0_0 || UNITY_5_0_1)
			if (webCamTexture.width > 16 && webCamTexture.height > 16) {
#else
            if (webCamTexture.didUpdateThisFrame)
            {
#if UNITY_IOS && !UNITY_EDITOR && UNITY_5_2
			while (webCamTexture.width <= 16) {
				webCamTexture.GetPixels32 ();
				yield return new WaitForEndOfFrame ();
			} 
#endif
#endif
                updateLayout();
                isInitDone = HandleOnInit();// true;
                 
                break;
            }
            else
            {
                Debug.Log("Web cam did not update this frame");
                yield return 0;
            }
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        

        if (!isInitDone)
            return;

        if (webCamTexture.didUpdateThisFrame)
        {

            //matRenderer.OnRender();
            HandleOnUpdate();
        }



    }

    protected abstract bool HandleOnInit();
    protected abstract void HandleOnUpdate();

    //protected abstract IEnumerator HandleOnLoop();

    private void updateLayout()
    {
        gameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
        gameObject.transform.localScale = new Vector3(webCamTexture.width, webCamTexture.height, 1);
        
        if (webCamTexture.videoRotationAngle == 90 || webCamTexture.videoRotationAngle == 270)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, -90);
        }


        float width = 0;
        float height = 0;
        if (webCamTexture.videoRotationAngle == 90 || webCamTexture.videoRotationAngle == 270)
        {
            width = gameObject.transform.localScale.y;
            height = gameObject.transform.localScale.x;
        }
        else if (webCamTexture.videoRotationAngle == 0 || webCamTexture.videoRotationAngle == 180)
        {
            width = gameObject.transform.localScale.x;
            height = gameObject.transform.localScale.y;
        }

        float widthScale = (float)Screen.width / width;
        float heightScale = (float)Screen.height / height;


        float delta_y = 0.5f * (webCamTexture.height * widthScale - Screen.height);
        float delta_x = 0.5f * (webCamTexture.width * heightScale - Screen.width);

        var trans = Vector3.zero;
        var scale = new Vector3(1, -1, 1);//bottom_left to top_left


        if (widthScale > heightScale)
        {
            Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;

            trans = new Vector3(0, -Screen.height - delta_y, 0);
            scale /= widthScale;

        }
        else
        {
            Camera.main.orthographicSize = height / 2;

            trans = new Vector3(delta_x, -Screen.height, 0);
            scale /= heightScale;
        }

        Screen2TextureMatrix = Matrix4x4.Scale(scale);
        Screen2TextureMatrix *= Matrix4x4.TRS(trans, Quaternion.identity, Vector3.one);

    }



}
