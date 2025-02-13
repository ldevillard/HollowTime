using Microsoft.AspNetCore.Components;

namespace HollowTime.Components
{
    public partial class TimeStats : ComponentBase
    {
        List<KeyValuePair<int, TimeSpan>> currentTimes = new List<KeyValuePair<int, TimeSpan>>();

        TimeSpan lastTime;
        TimeSpan averageOfFive;
        TimeSpan averageOfTwelve;

        public void RecordTime(TimeSpan timeToRecord)
        {
            currentTimes.Add(new KeyValuePair<int, TimeSpan>(currentTimes.Count + 1, timeToRecord));
            lastTime = timeToRecord;
            averageOfFive = computeAverage(5);
            averageOfTwelve = computeAverage(12);
            StateHasChanged();
        }

        string getFormatedTime(TimeSpan timeSpan)
        {
            if (timeSpan.Hours > 0)
            {
                return $"{timeSpan.Hours:00}.{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{(timeSpan.Milliseconds / 10):00}";
            }
            else if (timeSpan.Minutes > 0)
            {
                return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{(timeSpan.Milliseconds / 10):00}";
            }
            else
            {
                return $"{timeSpan.Seconds:0}.{(timeSpan.Milliseconds / 10):00}";
            }
        }

        TimeSpan computeAverage(int averageOfNumber)
        {
            TimeSpan returnedAverage = TimeSpan.Zero;

            if (currentTimes.Count < averageOfNumber)
            {
                return returnedAverage;
            }

            // Take last averageOfNumber times
            List<TimeSpan> times = currentTimes.Select(x => x.Value).ToList();
            List<TimeSpan> lastTimes = times.Skip(times.Count - averageOfNumber).ToList();

            // Sort them to find the best and the worst
            lastTimes.Sort();

            // Remove best and worst times
            lastTimes.RemoveAt(0);
            lastTimes.RemoveAt(lastTimes.Count - 1);

            // Compute the average of the remaining times
            returnedAverage = TimeSpan.FromTicks((long)lastTimes.Average(ts => ts.Ticks));
            return returnedAverage;
        }
    }
}
