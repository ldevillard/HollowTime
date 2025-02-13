namespace HollowTime.Data
{
    public enum RecordType
    {
        Single,
        AO5,
        AO12
    }
    
    public class RecordData
    {
        public int SolveIndex { get; set; }
        public TimeRecordData SingleTime { get; set; }
        public TimeRecordData AverageOfFive { get; set; }
        public TimeRecordData AverageOfTwelve { get; set; }
    }

    public class TimeRecordData
    {
        public RecordType Type { get; set; }
        public TimeSpan Time { get; set; }
        public bool BestTime { get; set; }
    }
}
