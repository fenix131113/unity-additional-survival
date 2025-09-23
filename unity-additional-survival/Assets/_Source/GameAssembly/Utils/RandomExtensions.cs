using UnityEngine;

namespace Utils
{
    public static class RandomExtensions
    {
        public static float RandomExceptRange(float min, float max, float excludeMin, float excludeMax)
        {
            var leftLength = excludeMin - min;
            var rightLength = max - excludeMax;
            var totalLength = leftLength + rightLength;

            return Random.value < leftLength / totalLength ? Random.Range(min, excludeMin) : Random.Range(excludeMax, max);
        }
    }
}