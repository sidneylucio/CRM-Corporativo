namespace CRM.Corporativo.Infra.CrossCutting.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Adds the given number of business days to the <see cref="DateTime"/>.
    /// </summary>
    /// <param name="current">The date to be changed.</param>
    /// <param name="days">Number of business days to be added.</param>
    /// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
    public static DateTime AddBusinessDays(this DateTime current, int days)
    {
        var sign = Math.Sign(days);
        var unsignedDays = Math.Abs(days);
        for (var i = 0; i < unsignedDays; i++)
        {
            current = current.AddDays(sign);
            if (current.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                i--;
            }
        }
        return current;
    }
}