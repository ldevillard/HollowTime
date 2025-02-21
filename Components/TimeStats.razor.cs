using HollowTime.Data;
using HollowTime.Utility;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Security;

namespace HollowTime.Components
{
    public partial class TimeStats : ComponentBase
    {
        #region Private Properties

        [Inject] IDialogService? dialogService { get; set; }

        #endregion

        #region Private Members

        List<RecordData> currentTimes = new List<RecordData>();
        List<SummaryTimeData> summaryTimes = new List<SummaryTimeData>();
        
        TimeSpan lastTime;
        TimeSpan averageOfFive;
        TimeSpan averageOfTwelve; 
        TimeSpan mean;
        
        #endregion
        
        #region Protected Methods

        protected override void OnInitialized()
        {
            summaryTimes.Add(new SummaryTimeData { Type = RecordType.Single});
            summaryTimes.Add(new SummaryTimeData { Type = RecordType.AO5});
            summaryTimes.Add(new SummaryTimeData { Type = RecordType.AO12});
        }
        
        #endregion
        
        #region Public Methods
        
        public void RecordTime(TimeSpan timeToRecord)
        {
            lastTime = timeToRecord;
            // Compute the averages
            List<TimeSpan> times = currentTimes.Select(x => x.SingleTime.Time).ToList();
            averageOfFive = getAverageOfNumber(5, times, timeToRecord);
            averageOfTwelve = getAverageOfNumber(12, times, timeToRecord);

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

            // Fill the summary data
            summaryTimes[0].Current = lastTime;
            summaryTimes[0].Best = recordedTime.SingleTime.BestTime ? lastTime : summaryTimes[0].Best;
            summaryTimes[1].Current = averageOfFive;
            summaryTimes[1].Best = recordedTime.AverageOfFive.BestTime ? averageOfFive : summaryTimes[1].Best;
            summaryTimes[2].Current = averageOfTwelve;
            summaryTimes[2].Best = recordedTime.AverageOfTwelve.BestTime ? averageOfTwelve : summaryTimes[2].Best;
            
            computeMean();
            StateHasChanged();
        }

        #endregion

        #region Private Methods

        async void onSingleClicked(int solveIndex) 
        {
            if (dialogService is null)
            {
                return;
            }

            var options = new DialogOptions { CloseOnEscapeKey = true };

            bool? result = await dialogService.ShowMessageBox(
                "Do you want to delete a solve?",
                $"Solve at index: {solveIndex}",
                yesText: "Delete",
                cancelText: "Cancel",
                options: options);

            if (result is not null && result.Value) 
            {
                removeSolveAtIndex(solveIndex);
                StateHasChanged();
            }
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

        TimeSpan getAverageOfNumber(int averageOfNumber, List<TimeSpan> times, TimeSpan withTime = default(TimeSpan))
        {
            TimeSpan returnedAverage = TimeSpan.Zero;
            bool isWithTime = withTime != TimeSpan.Zero;

            // Check if there is enough times to compute the average (take withTime in consideration)
            if ((!isWithTime && times.Count < averageOfNumber) || (isWithTime && times.Count < averageOfNumber - 1))
            {
                return returnedAverage;
            }

            // Take last averageOfNumber times
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
        
        void computeMean()
        {
            if (currentTimes.Count < 1)
            {
                return;
            }
            
            long tickMean = 0;
            foreach (RecordData time in currentTimes)
            {
                tickMean += time.SingleTime.Time.Ticks;
            }
            long meanTicks = tickMean / currentTimes.Count;
            mean = TimeSpan.FromTicks(meanTicks);
        }

        bool isBestTime(TimeSpan time, RecordType recordType, bool withTime = false)
        {
            bool isBest = false;

            if (time.Equal(TimeSpan.Zero))
            {
                return isBest;
            }
            
            switch (recordType)
            {
                case RecordType.Single:
                    isBest = currentTimes.Where(x => !x.SingleTime.Time.Equal(TimeSpan.Zero))
                                            .All(x => withTime ? x.SingleTime.Time >= time : x.SingleTime.Time > time);
                    break;
                case RecordType.AO5:
                    isBest = currentTimes.Where(x => !x.AverageOfFive.Time.Equal(TimeSpan.Zero))
                                            .All(x => withTime? x.AverageOfFive.Time >= time : x.AverageOfFive.Time > time);
                    break;
                case RecordType.AO12:
                    isBest = currentTimes.Where(x => !x.AverageOfTwelve.Time.Equal(TimeSpan.Zero))
                                            .All(x => withTime ? x.AverageOfTwelve.Time >= time : x.AverageOfTwelve.Time > time);
                    break;
                default:
                    isBest = false;
                    break;
            }

            return isBest;
        }

        void removeSolveAtIndex(int solveIndex) 
        {
            // Remove the time element
            currentTimes.RemoveAt(solveIndex - 1);

            List<TimeSpan> encounteredTimes = new List<TimeSpan>();
            
            // Rebuild the list by recomputing the averages and indexes
            for (int i = 0; i < currentTimes.Count; i++) 
            {
                currentTimes[i].SolveIndex = i + 1;

                List<TimeSpan> times = currentTimes.Select(x => x.SingleTime.Time).Take(i + 1).ToList();

                // Because we are using the withTime bool with isBestTime, if we have to same times we don't want to set both as best times
                bool withTime = encounteredTimes.All(x => currentTimes[i].SingleTime.Time != x);
                
                // Recompute Single
                currentTimes[i].SingleTime.BestTime = isBestTime(currentTimes[i].SingleTime.Time, RecordType.Single, withTime);
                
                // Recompute AO5
                currentTimes[i].AverageOfFive.Time = getAverageOfNumber(5, times);
                currentTimes[i].AverageOfFive.BestTime = isBestTime(currentTimes[i].AverageOfFive.Time, RecordType.AO5, withTime);

                // Recompute AO12
                currentTimes[i].AverageOfTwelve.Time = getAverageOfNumber(12, times);
                currentTimes[i].AverageOfTwelve.BestTime = isBestTime(currentTimes[i].AverageOfTwelve.Time, RecordType.AO12, withTime);
                
                encounteredTimes.Add(currentTimes[i].SingleTime.Time);
            }
        }

        #endregion
    }
}
