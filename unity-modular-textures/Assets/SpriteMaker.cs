using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteMaker : MonoBehaviour {
    SpriteRenderer rend;

    Texture2D tex;

    public Texture2D[] TextureArray;
    public Color[] ColorArray;

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
        Color[][] adjustedLayers = new Color[layers.Length][];

        //populate array with cropped or expanded layer arrays
        for (int i = 0; i < layers.Length; i++) {
            if (i == 0 || layers[i].width == newTexture.width && layers[i].height == newTexture.height) {
                adjustedLayers[i] = layers[i].GetPixels();
            } else {
                int getX, getWidth, setX, setWidth;

                getX = layers[i].width > newTexture.width ? (layers[i].width - newTexture.width) / 2 : 0;
                getWidth = layers[i].width > newTexture.width ? newTexture.width : layers[i].width;
                setX = layers[i].width < newTexture.width ? (newTexture.width - layers[i].width) / 2 : 0;
                setWidth = layers[i].width < newTexture.width ? layers[i].width : newTexture.width;

                int getY, getHeight, setY, setHeight;

                getY = layers[i].height > newTexture.height ? (layers[i].height - newTexture.height) / 2 : 0;
                getHeight = layers[i].height > newTexture.height ? newTexture.height : layers[i].height;
                setY = layers[i].height < newTexture.height ? (newTexture.height - layers[i].height) / 2 : 0;
                setHeight = layers[i].height < newTexture.height ? layers[i].height : newTexture.height;

                Color[] getPixels = layers[i].GetPixels(getX, getY, getWidth, getHeight);
                if (layers[i].width >= newTexture.width && layers[i].height >= newTexture.height) {
                    adjustedLayers[i] = getPixels;
                } else {
                    Texture2D sizedLayer = clearTexture(newTexture.width, newTexture.height);
                    sizedLayer.SetPixels(setX, setY, setWidth, setHeight, getPixels);
                    adjustedLayers[i] = sizedLayer.GetPixels();
                }
            }
        }

        //set each color layer to alpha 100% if it isn't already
        for (int i = 0; i < layerColors.Length; i++) {
            if (layerColors[i].a != 1) {
                layerColors[i] = new Color(layerColors[i].r, layerColors[i].g, layerColors[i].b, 1f);
            }
        }

        //iterate through each pixel, copying the source index to the destination index
        for (int x = 0; x < newTexture.width; x++) {
            for (int y = 0; y < newTexture.height; y++) {
                int pixelIndex = x + y * newTexture.width;
                for (int i = 0; i < layers.Length; i++) {
                    Color srcPixel = adjustedLayers[i][pixelIndex];

                    //APPLY LAYER COLOR IF NECESSARY
                    if (srcPixel.r != 0 && srcPixel.a != 0 && i < layerColors.Length) {
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

    Texture2D clearTexture(int width, int height) {
        Texture2D clearTexture = new Texture2D(width, height);
        Color[] clearPixels = new Color[width * height];
        clearTexture.SetPixels(clearPixels);
        return clearTexture;
    }
}
