using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteMaker : MonoBehaviour {
    public Texture2D src;
    SpriteRenderer rend;

    // Start is called before the first frame update
    void Start() {
        rend = GetComponent<SpriteRenderer>();

        // create a texture
        Texture2D tex = new Texture2D(src.width, src.height);
        Color[] colorArray = new Color[tex.width * tex.height];

        Color[] srcArray = src.GetPixels();

        for (int x = 0; x < tex.width; x++) {
            for (int y = 0; y < tex.height; y++) {
                int pixelIndex = x + y * tex.width;
                Color srcPixel = srcArray[pixelIndex];
                colorArray[pixelIndex] = srcPixel;
            }
        }
        tex.SetPixels(colorArray);
        tex.Apply();

        // create a sprite from that texture
        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

        // assign our procedural sprite to rend sprite
        rend.sprite = newSprite;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;
    }

    // Update is called once per frame
    void Update() {

    }
}
