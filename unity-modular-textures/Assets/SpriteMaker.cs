using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteMaker : MonoBehaviour {

    SpriteRenderer rend;

    public Texture2D[] layers;
    public Color[] layerColors;

    Texture2D tex;


    // Use this for initialization
    void Start() {
        rend = GetComponent<SpriteRenderer>();

        //making a texture
        makeTexture();

        //making a sprite from that texture
        makeSprite();
    }

    void makeTexture() {
        //create a texture
        tex = new Texture2D(layers[0].width, layers[0].height);

        //array to store the destination texture's pixels
        Color[] colorArray = new Color[tex.width * tex.height];

        //array of colors derived from the source Texture
        Color[][] srcArray = new Color[layers.Length][];

        //populate source array with layer arrays
        for (int i = 0; i < layers.Length; i++) {
            srcArray[i] = layers[i].GetPixels();
        }

        // iterate through each pixel, copying the source index to the destination index
        for (int x = 0; x < tex.width; x++) {
            for (int y = 0; y < tex.height; y++) {
                int pixelIndex = x + y * tex.width;
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
        tex.SetPixels(colorArray);
        tex.Apply();

        //confirm any settings
        tex.wrapMode = TextureWrapMode.Clamp;
        //    tex.filterMode = FilterMode.Point;
    }

    void makeSprite() {
        //create a sprite from that texture
        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

        //assign our procedural sprite to rend sprite
        rend.sprite = newSprite;
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
