using System;

namespace BSB.Microservices.NServiceBus
{
    public static class StringExtensions
    {
        public static int GetLevenshteinDistance(this string input, string compare)
        {
            int expectedLength = input.Length;
            int actualLength = compare.Length;

            var distances = new int[expectedLength + 1, actualLength + 1];

            for (var i = 0; i <= expectedLength; distances[i, 0] = i++) ;
            for (var j = 0; j <= actualLength; distances[0, j] = j++) ;

            for (var i = 1; i <= expectedLength; i++)
            {
                for (var j = 1; j <= actualLength; j++)
                {
                    var cost = compare[j - 1] == input[i - 1] ? 0 : 1;

                    distances[i, j] = Math.Min(Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1), distances[i - 1, j - 1] + cost);
                }
            }

            return distances[expectedLength, actualLength];
        }
    }
}
