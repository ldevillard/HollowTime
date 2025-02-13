using HollowTime.Data;
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

            // Populate the recordedTime structure with timeToRecord
            RecordData recordedTime = new RecordData
            {
                SolveIndex = currentTimes.Count + 1,
                SingleTime = new TimeRecordData { Type = RecordType.Single, Time = lastTime }
            };
            // Register the recordedTime to the list, we need to add the object in the list before computing the averages (to update the list size)
            currentTimes.Add(recordedTime);
            
            // Compute the averages
            averageOfFive = computeAverage(5);
            averageOfTwelve = computeAverage(12);

            // Update the averages of the recordedTime struct
            recordedTime.AverageOfFive = new TimeRecordData { Type = RecordType.AO5, Time = averageOfFive };
            recordedTime.AverageOfTwelve = new TimeRecordData { Type = RecordType.AO12, Time = averageOfTwelve };

            // Check if the time is the current best or not
            recordedTime.SingleTime.BestTime = isBestTime(lastTime, RecordType.Single);
            recordedTime.AverageOfFive.BestTime = isBestTime(averageOfFive, RecordType.AO5);
            recordedTime.AverageOfTwelve.BestTime = isBestTime(averageOfTwelve, RecordType.AO12);
            
            StateHasChanged();
        }

        string getFormatedTime(TimeSpan timeSpan, bool handleZero = false)
        {
            if (handleZero && timeSpan == TimeSpan.Zero)
            {
                return "-";
            }
            
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
            List<TimeSpan> times = currentTimes.Select(x => x.SingleTime.Time).ToList();
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

            if (time == TimeSpan.Zero)
            {
                return isBest;
            }

            switch (recordType)
            {
                case RecordType.Single:
                    isBest = currentTimes.All(x => x.SingleTime.Time > time);
                    break;
                case RecordType.AO5:
                    isBest = currentTimes.All(x => x.AverageOfFive.Time > time);
                    break;
                case RecordType.AO12:
                    isBest = currentTimes.All(x => x.AverageOfTwelve.Time > time);
                    break;
                default:
                    isBest = false;
                    break;
            }

            return isBest;
        }
    }
}
