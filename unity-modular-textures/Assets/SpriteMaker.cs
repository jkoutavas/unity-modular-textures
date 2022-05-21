using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteMaker : MonoBehaviour {

    SpriteRenderer rend;

    public Texture2D[] TextureArray;
    public Color[] ColorArray;

    Texture2D tex;

    // Use this for initialization
    void Start() {
        rend = GetComponent<SpriteRenderer>();

        //making a texture
        tex = MakeTexture(TextureArray, ColorArray);

        //assign our procedural sprite to rend sprite
        rend.sprite = MakeSprite(tex);
    }

    public Texture2D MakeTexture(Texture2D[] layers, Color[] layerColors) {
        //BUG CHECK: If only one or no image layers present
        if (layers.Length == 0) {
            Debug.LogError("No image layer information in array");
            return Texture2D.whiteTexture;
        } else if (layers.Length == 1) {
            Debug.Log("Only one image layer present. Are you sure you need to make a texture?");
            return layers[0];
        }

        //create a texture
        Texture2D newTexture = new Texture2D(layers[0].width, layers[0].height);

        //array to store the destination texture's pixels
        Color[] colorArray = new Color[newTexture.width * newTexture.height];

        //array of colors derived from the source Texture
        Color[][] srcArray = new Color[layers.Length][];

        //populate source array with layer arrays
        for (int i = 0; i < layers.Length; i++) {
            srcArray[i] = layers[i].GetPixels();
        }

        // iterate through each pixel, copying the source index to the destination index
        for (int x = 0; x < newTexture.width; x++) {
            for (int y = 0; y < newTexture.height; y++) {
                int pixelIndex = x + y * newTexture.width;
                for (int i = 0; i < layers.Length; i++) {
                    Color srcPixel = srcArray[i][pixelIndex];

                    //APPLY LAYER COLOR IF NECESSARY
                    if (srcPixel.r != 0 && srcPixel.a != 0) {
                        srcPixel = applyColorToPixel(srcPixel, layerColors[i]);
                    }

                    //NORMAL BLENDING BASED ON ALPHA
                    if (srcPixel.a == 1) {
                        colorArray[pixelIndex] = srcPixel;
                    } else if (srcPixel.a > 0) {
                        colorArray[pixelIndex] = normalBlend(colorArray[pixelIndex], srcPixel);
                    }
                }
            }
        }

        //transfer the array to the texture and apply the pixels
        newTexture.SetPixels(colorArray);
        newTexture.Apply();

        //confirm any settings
        newTexture.wrapMode = TextureWrapMode.Clamp;
        //newTexture.filterMode = FilterMode.Point;

        return newTexture;
    }

    public Sprite MakeSprite(Texture2D texture) {
        //create a sprite from that texture
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }

    Color normalBlend(Color dest, Color src) {
        float srcAlpha = src.a;
        float destAlpha = (1 - srcAlpha) * dest.a;
        Color destLayer = dest * destAlpha;
        Color srcLayer = src * srcAlpha;
        return destLayer + srcLayer;
    }

    Color applyColorToPixel(Color pixel, Color applyColor) {
        if (pixel.r == 1f) {
            return applyColor;
        }
        return pixel * applyColor;
    }
}
