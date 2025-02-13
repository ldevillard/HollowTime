namespace HollowTime.Utility;

public static class Utility
{
    #region Extension Methods

    public static bool Equal(this TimeSpan t1, TimeSpan t2, double epsilon = 0.001)
    {
        return Math.Abs((t1 - t2).TotalSeconds) < epsilon;
    }

    #endregion   
}