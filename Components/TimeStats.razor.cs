using HollowTime.Data;
using HollowTime.Utility;
using Microsoft.AspNetCore.Components;

namespace HollowTime.Components
{
    public partial class TimeStats : ComponentBase
    {
        List<RecordData> currentTimes = new List<RecordData>();

        TimeSpan lastTime;
        TimeSpan averageOfFive;
        TimeSpan averageOfTwelve;

        public void RecordTime(TimeSpan timeToRecord)
        {
            lastTime = timeToRecord;
            // Compute the averages
            averageOfFive = computeAverage(5, timeToRecord);
            averageOfTwelve = computeAverage(12, timeToRecord);

            // Populate the recordedTime structure with timeToRecord
            RecordData recordedTime = new RecordData
            {
                SolveIndex = currentTimes.Count + 1,
                SingleTime = new TimeRecordData { Type = RecordType.Single, Time = lastTime, BestTime = isBestTime(lastTime, RecordType.Single)},
                AverageOfFive = new TimeRecordData { Type = RecordType.AO5, Time = averageOfFive, BestTime = isBestTime(averageOfFive, RecordType.AO5) },
                AverageOfTwelve = new TimeRecordData { Type = RecordType.AO12, Time = averageOfTwelve, BestTime = isBestTime(averageOfTwelve, RecordType.AO12) }
            };
            // Register the recordedTime to the list
            currentTimes.Add(recordedTime);

            StateHasChanged();
        }

        string getFormatedTime(TimeSpan time)
        {
            if (time.Equal(TimeSpan.Zero))
            {
                return "-";
            }
            
            if (time.Hours > 0)
            {
                return $"{time.Hours:00}.{time.Minutes:00}:{time.Seconds:00}.{(time.Milliseconds / 10):00}";
            }
            else if (time.Minutes > 0)
            {
                return $"{time.Minutes:00}:{time.Seconds:00}.{(time.Milliseconds / 10):00}";
            }
            else
            {
                return $"{time.Seconds:0}.{(time.Milliseconds / 10):00}";
            }
        }

        TimeSpan computeAverage(int averageOfNumber, TimeSpan withTime = default(TimeSpan))
        {
            TimeSpan returnedAverage = TimeSpan.Zero;
            bool isWithTime = withTime != TimeSpan.Zero;

            // Check if there is enough times to compute the average (take withTime in consideration)
            if ((!isWithTime && currentTimes.Count < averageOfNumber) || (isWithTime && currentTimes.Count < averageOfNumber - 1))
            {
                return returnedAverage;
            }

            // Take last averageOfNumber times
            List<TimeSpan> times = currentTimes.Select(x => x.SingleTime.Time).ToList();
            if (isWithTime)
            {
                times.Add(withTime);
            }
            
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

        bool isBestTime(TimeSpan time, RecordType recordType)
        {
            bool isBest = false;

            if (time.Equal(TimeSpan.Zero))
            {
                return isBest;
            }

            //List<RecordData> times = currentTimes.Where(x => !x.SingleTime.Time.Equal(TimeSpan.Zero)).ToList();
            
            switch (recordType)
            {
                case RecordType.Single:
                    isBest = currentTimes.Where(x => !x.SingleTime.Time.Equal(TimeSpan.Zero))
                                            .All(x => x.SingleTime.Time > time);
                    break;
                case RecordType.AO5:
                    isBest = currentTimes.Where(x => !x.AverageOfFive.Time.Equal(TimeSpan.Zero))
                                            .All(x => x.AverageOfFive.Time > time);
                    break;
                case RecordType.AO12:
                    isBest = currentTimes.Where(x => !x.AverageOfTwelve.Time.Equal(TimeSpan.Zero))
                                            .All(x => x.AverageOfTwelve.Time > time);
                    break;
                default:
                    isBest = false;
                    break;
            }

            return isBest;
        }
    }
}
