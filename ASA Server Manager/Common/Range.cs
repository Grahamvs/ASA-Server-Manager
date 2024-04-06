namespace ASA_Server_Manager.Common;

public static class Range
{
    #region Public Methods

    public static T? SetInRange<T>(T? value, T min, T max)
        where T : struct, IComparable<T> =>
        new Range<T>(min, max).SetInRange(value);

    public static T SetInRange<T>(T value, T min, T max)
        where T : struct, IComparable<T> =>
        new Range<T>(min, max).SetInRange(value);

    #endregion
}

public class Range<T>
    where T : struct, IComparable<T>
{
    #region Public Constructors

    public Range(T min, T max)
    {
        if (min.CompareTo(max) > 0)
        {
            // Swap by deconstruction using tuples.
            (Min, Max) = (max, min);
        }
        else
        {
            Min = min;
            Max = max;
        }
    }

    #endregion

    #region Public Properties

    public T Max { get; }

    public T Min { get; }

    #endregion

    #region Public Methods

    public T? SetInRange(T? value)
    {
        if (!value.HasValue)
            return null;

        return SetInRange(value.Value);
    }

    public T SetInRange(T value)
    {
        if (value is double and (double.NaN or double.NegativeInfinity or double.PositiveInfinity))
            return value;

        if (value.CompareTo(Min) < 0)
            return Min;

        if (value.CompareTo(Max) > 0)
            return Max;

        return value;
    }

    #endregion
}