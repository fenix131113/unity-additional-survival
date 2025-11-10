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
        
        public static float RandomExceptRange(int minInclusive, int maxInclusive, int excludeMin, int excludeMax)
        {
            var leftLength = excludeMin - minInclusive;
            var rightLength = maxInclusive - excludeMax;
            var totalLength = leftLength + rightLength;

            if (totalLength <= 0)
                return Random.Range(minInclusive, maxInclusive);

            var randomIndex = Random.Range(0, totalLength);

            if (randomIndex < leftLength)
                return minInclusive + randomIndex;

            return excludeMax + (randomIndex - leftLength);
        }
    }
}