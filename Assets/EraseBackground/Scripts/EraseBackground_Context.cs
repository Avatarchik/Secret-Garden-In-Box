using UnityEngine;
using System.Collections;
using System;

public class EraseBackground_Context : BasicWebCamBuilder
{

    EraseBackground_Pipeline erasePipeline;

    protected override bool HandleOnInit()
    {
        erasePipeline = new EraseBackground_Pipeline(webCamTexture.height, webCamTexture.width);

        return true;
    }

    protected override void HandleOnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SaveToLocal(erasePipeline.HandleOnErase(Input.mousePosition));

        }
    }

    void SaveToLocal(Texture2D tex)
    {

    }

}
