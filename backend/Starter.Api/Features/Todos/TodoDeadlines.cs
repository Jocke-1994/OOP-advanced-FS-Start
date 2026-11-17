using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

/// <summary>
/// Regeln "när är en uppgift försenad?" på ett enda ställe. Metoden är ren: den frågar inte själv
/// vad klockan är, utan får "nu" som parameter. Därför kan ett test bestämma tiden.
/// </summary>
public static class TodoDeadlines
{
    // Kims regel, oförändrad: försenad om förfallodatumet ligger före "nu".
    public static bool IsOverdue(Todo todo, DateTime now) =>
        !todo.IsDone && todo.DueDate is { } due && due < now;

    public static int DaysLate(DateTime dueDate, DateTime now) => (now - dueDate).Days;
}
