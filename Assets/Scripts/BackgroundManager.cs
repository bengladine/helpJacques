using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviourSingleton<BackgroundManager>
{
    #region SpriteObject
    public GameObject DepthSpriteObject;

    // the sprite itself, taken from the GameObject.
    private Sprite _depthSprite;

    // width and height of the sprite in worldspace units 
    private Vector2 _boundsSprite;

    // width and height of the sprite in pixel units
    private Vector2 _rectSprite;

    // left bottom in worldspace coordinates
    private Vector2 _leftBottomCornerSprite;
    #endregion

    protected override void SingletonAwake()
    {
        if (DepthSpriteObject != null)
        {
            _depthSprite = DepthSpriteObject.GetComponent<SpriteRenderer>().sprite;
            _boundsSprite = _depthSprite.bounds.size;
            _rectSprite = _depthSprite.rect.size;
            _leftBottomCornerSprite = new Vector2(DepthSpriteObject.transform.position.x - _boundsSprite.x / 2.0f, DepthSpriteObject.transform.position.y - _boundsSprite.y / 2.0f);
        }
    }

    public float GetAlphaValue(Vector2 targetPos)
    {
        if (_depthSprite != null)
        {
            // position of target relative to the sprite (not the screen as it is now) with (0,0) being the bottom left corner
            var posOnSprite = targetPos - _leftBottomCornerSprite;
            var uvCoordinates = new Vector2(posOnSprite.x / _boundsSprite.x, posOnSprite.y / _boundsSprite.y);

            Debug.Log($"Position on sprite: {posOnSprite}");

            #if DEBUG
            Debug.Log($"UV Coordinates: {uvCoordinates}");
            if (posOnSprite.x < 0.0f || posOnSprite.y < 0.0f)
            {
                Debug.Log($"posOnSprite is negative! Values: {posOnSprite}");
            }
            #endif
            return _depthSprite.texture.GetPixel((int)(uvCoordinates.x * _rectSprite.x), (int)(uvCoordinates.y * _rectSprite.y)).a;
        }



        //if (DepthSpriteObject != null)
        //{
        //    //#if DEBUG
        //    //Debug.Log($"Pixel coordinate at player pos: {DepthSprite.texture.GetPixel((int)screenPos.x, (int)screenPos.y)}");
        //    //#endif
        //    return DepthSpriteObject.texture.GetPixel((int)screenPos.x, (int)screenPos.y).a;
        //}

        //if (DistanceTexture != null)
        //{
        //    #if DEBUG
        //    Debug.Log($"Pixel coordinate at player pos: {DistanceTexture.GetPixel((int)screenPos.x, (int)screenPos.y)}");
        //    #endif
        //    return DistanceTexture.GetPixel((int)screenPos.x, (int)screenPos.y).a;
        //}
        return 1.0f;
    }
}
