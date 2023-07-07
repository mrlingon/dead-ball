using Unity.Mathematics;
using UnityEngine;


public static unsafe class Collider2DExtensions
{
    public static float3 GetRandomPointInsideCollider(this BoxCollider2D boxCollider)
    {
        var extents = boxCollider.size / 2f;
        var point = new float2(UnityEngine.Random.Range(-extents.x, extents.x), UnityEngine.Random.Range(-extents.y, extents.y)) + (float2)boxCollider.offset;

        return ((float3)boxCollider.transform.TransformPoint(new Vector3(point.x, point.y, 0)));
    }
}

