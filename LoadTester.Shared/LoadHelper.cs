namespace LoadTester.Shared
{
    public static class LoadHelper
    {
        public static int GetDelayDuration(int lowestDuration, int highestDuration, bool randomLoad)
        {
            double middleDuration = (lowestDuration + highestDuration) / 2;
            var duration = (int)Math.Round(middleDuration);

            if (randomLoad)
            {
                var r = new Random();
                duration = r.Next(lowestDuration, highestDuration);
            }

            return duration;
        }
    }
}
