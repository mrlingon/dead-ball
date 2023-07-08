using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameField : MonoBehaviour
{
    public PaintableSurfaceTexture Surface { get; private set; }

    void Awake()
    {
        if (GameManager.Instance.GameField != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.Instance.GameField = this;

        Surface = GetComponent<PaintableSurfaceTexture>();
    }

    public void SplatterPaint(float2 pos, float radius, int paintCountMin = 5, int paintCountMax = 10, int paintSizeMin = 1, int paintSizeMax = 5)
    {
        // Randomize a bunch of PaintSurfaceInfo data to form randomized blood splatter in the given radius
        var paintData = new List<PaintSurfaceInfo>();
        var paintCount = UnityEngine.Random.Range(paintCountMin, paintCountMax);

        for (int i = 0; i < paintCount; i++)
        {
            var paintPos = pos + new float2(UnityEngine.Random.Range(-radius, radius), UnityEngine.Random.Range(-radius, radius));
            var paintRadius = UnityEngine.Random.Range(paintSizeMin, paintSizeMax);
            var paintColor = new Color32(255, 0, 0, 255);
            paintData.Add(new PaintSurfaceInfo { WorldPos = paintPos, Radius = paintRadius, Color = paintColor });
        }

        Surface.PaintSurface(paintData.ToArray());
    }
}
