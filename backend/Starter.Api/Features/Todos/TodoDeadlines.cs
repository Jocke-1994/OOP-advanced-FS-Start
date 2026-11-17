using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

/// <summary>
/// Regeln "när är en uppgift försenad?" på ett enda ställe. Metoderna är rena: de frågar inte själva
/// vad klockan är, utan får "idag" som parameter. Därför kan ett test bestämma dagen.
/// </summary>
public static class TodoDeadlines
{
    // Sara: "har jag sagt fredag så får det bli klart fredag kväll". Därför jämförs svensk tid.
    private static readonly TimeZoneInfo Stockholm = TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");

    /// <summary>Vilket datum det är i Sverige just nu, oavsett vilken tidszon servern står i.</summary>
    public static DateOnly Today(TimeProvider clock) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(clock.GetUtcNow(), Stockholm).DateTime);

    /// <summary>Försenad först dagen efter förfallodatumet. Förfallodagen räknas som hel dag.</summary>
    public static bool IsOverdue(Todo todo, DateOnly today) =>
        !todo.IsDone && todo.DueDate is { } due && due < today;

    public static int DaysLate(DateOnly dueDate, DateOnly today) => today.DayNumber - dueDate.DayNumber;
}
