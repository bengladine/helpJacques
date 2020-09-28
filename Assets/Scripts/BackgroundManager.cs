using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : SingletonBase<BackgroundManager>
{
    public Texture2D BackgroundTexture;
    public Texture2D DistanceTexture;

    public float GetAlphaValue(Vector2 pos)
    {

        return 0.0f;
        //DistanceTexture.GetPixel()
    }
}
 