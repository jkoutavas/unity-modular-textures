using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteMaker : MonoBehaviour {
    public Texture2D[] layers;
    SpriteRenderer rend;

    Texture2D tex;


    void Start() {
        //making a texture
        makeTexture();

        //making a sprite from that texture
        makeSprite();
    }

    void makeTexture() {
        //create a texture
        rend = GetComponent<SpriteRenderer>();

        //create a texture
        tex = new Texture2D(layers[0].width, layers[0].height);
        Color[] colorArray = new Color[tex.width * tex.height];

        Color[][] srcArray = new Color[layers.Length][];

        //populate source array with layer arrays
        for (int i = 0; i < layers.Length; i++) {
            srcArray[i] = layers[i].GetPixels();
        }

        for (int x = 0; x < tex.width; x++) {
            for (int y = 0; y < tex.height; y++) {
                int pixelIndex = x + y * tex.width;
                for (int i = 0; i < layers.Length; i++) {
                    Color srcPixel = srcArray[i][pixelIndex];
                    if (srcPixel.a == 1) {
                        colorArray[pixelIndex] = srcPixel;
                    } else if (srcPixel.a > 0) {
                        colorArray[pixelIndex] = normalBlend(colorArray[pixelIndex], srcPixel);
                    }
                }
            }
        }
        tex.SetPixels(colorArray);
        tex.Apply();

        //confirm any settings
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;
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
}
