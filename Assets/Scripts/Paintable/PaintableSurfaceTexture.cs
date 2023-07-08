using UnityEngine;
using Unity.Mathematics;
using System;

[Serializable]
public struct PaintSurfaceInfo
{
    public float2 WorldPos;
    public int Radius;
    public Color Color;
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class PaintableSurfaceTexture : MonoBehaviour
{
    [Header("Surface Settings")]
    public LayerMask DrawingLayers;

    public Texture2D DefaultTexture;

    [Header("Debug Settings")]
    public bool ActivateDebug = true;

    public int DebugPaintSize = 20;
    public Color DebugPaintColor = Color.red;

    private Sprite sprite;
    private Color32[] resetColorArray;

    private Texture2D texture => sprite.texture;
    private float spriteWidth => sprite.rect.width;
    private float spriteHeight => sprite.rect.height;

    // Public API
    public void PaintSurface(PaintSurfaceInfo data)
    {
        var toPaint = texture.GetPixels32();
        PaintCircle(WorldToPixelPos(data.WorldPos), data.Radius, data.Color, ref toPaint);
        ApplyPixelArray(toPaint);
    }

    public void PaintSurface(PaintSurfaceInfo[] data)
    {
        var toPaint = texture.GetPixels32();

        for (int i = 0; i < data.Length; i++)
        {
            PaintCircle(WorldToPixelPos(data[i].WorldPos), data[i].Radius, data[i].Color, ref toPaint);
        }

        ApplyPixelArray(toPaint);
    }

    public void ClearSurface()
    {
        ApplyPixelArray(resetColorArray);
    }

    public Color32[] Pixels32()
    {
        return texture.GetPixels32();
    }
    private bool SameColor(Color32 a, Color32 b)
    {
        return a.r == b.r && a.g == b.g && a.b == b.b;
    }

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;

        // Initialize clean pixels to use
        resetColorArray = (Color32[])DefaultTexture.GetPixels32().Clone();

        ClearSurface();
    }

    // This is where the magic happens. Detects when user is left clicking, which then call the
    // appropriate function
    private void Update()
    {
        if (!ActivateDebug) return;


        // Convert mouse coordinates to world coordinates
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos, DrawingLayers.value);
        if (hit != null && hit.transform != null)
        {
            var toPaint = texture.GetPixels32();
            PaintCircle(WorldToPixelPos(mousePos), DebugPaintSize, DebugPaintColor, ref toPaint);
            ApplyPixelArray(toPaint);
        }
    }

    // Pos is in PixelPosition
    private void PaintCircle(float2 pos, int radius, Color color, ref Color32[] toPaint)
    {
        int2 centerPos = (int2)pos;
        for (int x = centerPos.x - radius; x <= centerPos.x + radius; x++)
        {
            // Check if the X wraps around the image, so we don't draw pixels on the other side of
            // the image
            if (x >= (int)spriteWidth || x < 0)
                continue;

            for (int y = centerPos.y - radius; y <= centerPos.y + radius; y++)
            {
                // Need to transform x and y coordinates to flat coordinates of array
                var colorArrayIdx = ((y * (int)spriteWidth) + x);

                // Check if this is a valid position
                if (colorArrayIdx > toPaint.Length || colorArrayIdx < 0)
                {
                    continue;
                }

                // Draw circle
                if (math.distance(centerPos, new float2(x, y)) > radius)
                {
                    continue;
                }

                toPaint[colorArrayIdx] = color;
            }
        }
    }

    private void ApplyPixelArray(Color[] colors)
    {
        texture.SetPixels(colors);
        texture.Apply();
    }

    private void ApplyPixelArray(Color32[] colors)
    {
        texture.SetPixels32(colors);
        texture.Apply();
    }

    //@ref FreeDraw
    private float2 WorldToPixelPos(float2 position)
    {
        // Change coordinates to local coordinates of this image
        var localPos = transform.InverseTransformPoint((Vector2)position);

        var unitsToPixels = spriteWidth / sprite.bounds.size.x * transform.localScale.x;

        // Need to center our coordinates
        var centeredX = localPos.x * unitsToPixels + spriteWidth / 2;
        var centeredY = localPos.y * unitsToPixels + spriteHeight / 2;

        return new Vector2(Mathf.RoundToInt(centeredX), Mathf.RoundToInt(centeredY));
    }
}
