using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Mathematics
{
    public static class mathx
    {
        /// <summary>
        /// angle
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float angle(float3 from, float3 to)
        {
            return math.degrees(math.acos(math.dot(math.normalize(from), math.normalize(to))));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ProjectOnPlane(in float3 v, in float3 normal)
        {
            return v - math.dot(v, normal) * normal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ProjectOnPlane(in float3 v, in float3 normal, in float3 direction)
        {
            return v - (math.dot(v, normal) / math.dot(normal, direction)) * direction;
        }

        /// <summary>
        /// Returns a tuple with the length & unit vector of given vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (float len, float3 dir) lendir(in float3 v)
        {
            return (math.length(v), math.normalizesafe(v));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 clamplen(float3 v, float max, float min = 0.0f)
        {
            var l = math.length(v);

            if (l <= math.EPSILON)
                return new float3(min, 0, 0);

            return math.clamp(l, min, max) * (v / l);
        }

        /// <summary>
        /// Split given vector on given direction vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (float3 Dir, float3 Other) split(float3 v, float3 dir)
        {
            dir = math.normalizesafe(dir);
            var x = v - dir * math.dot(v, dir);
            return (v - x, x);
        }

        /// <summary>
        /// Returns true if all components of the specified vector are <c>0.0</c> (or within the given epsilon).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool iszero(float3 v, float epsilon = 1E-04f)
        {
            return math.length(v) <= epsilon;
        }

        /// <summary>
        /// safe
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 safe(float3 v)
        {
            return math.any(math.isnan(v)) ? 0.0f : v;
        }
    }
}
