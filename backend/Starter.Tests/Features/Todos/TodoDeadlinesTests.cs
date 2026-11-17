using Microsoft.Extensions.Time.Testing;
using Starter.Api.Domain;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>Enhetstester för regeln. Ingen databas, ingen HTTP, ingen riktig klocka.</summary>
public sealed class TodoDeadlinesTests
{
    private static readonly DateOnly Today = new(2026, 11, 16);

    [Fact]
    [Trait("Krav", "AK-2.1")]
    public void A_todo_due_today_is_not_overdue()
    {
        var todo = new Todo { DueDate = Today };

        Assert.False(TodoDeadlines.IsOverdue(todo, Today));
    }

    [Fact]
    [Trait("Krav", "AK-2.2")]
    public void A_todo_due_yesterday_is_overdue()
    {
        var todo = new Todo { DueDate = Today.AddDays(-1) };

        Assert.True(TodoDeadlines.IsOverdue(todo, Today));
    }

    [Fact]
    [Trait("Krav", "AK-2.3")]
    public void A_done_todo_is_never_overdue()
    {
        var todo = new Todo { DueDate = Today.AddDays(-1), IsDone = true };

        Assert.False(TodoDeadlines.IsOverdue(todo, Today));
    }

    [Fact]
    public void A_todo_without_due_date_is_never_overdue()
    {
        var todo = new Todo { DueDate = null };

        Assert.False(TodoDeadlines.IsOverdue(todo, Today));
    }

    // 23:30 UTC den 16:e är redan 00:30 den 17:e i Stockholm (UTC+1 på vintern).
    [Fact]
    [Trait("Krav", "AK-2.1")]
    public void Today_is_the_swedish_date_not_the_server_date()
    {
        var clock = new FakeTimeProvider(new DateTimeOffset(2026, 11, 16, 23, 30, 0, TimeSpan.Zero));

        Assert.Equal(new DateOnly(2026, 11, 17), TodoDeadlines.Today(clock));
    }

    [Fact]
    [Trait("Krav", "AK-2.1")]
    public void A_todo_due_today_is_still_not_overdue_at_23_59_swedish_time()
    {
        // 22:59 UTC = 23:59 i Stockholm, fortfarande den 16:e.
        var clock = new FakeTimeProvider(new DateTimeOffset(2026, 11, 16, 22, 59, 0, TimeSpan.Zero));
        var todo = new Todo { DueDate = Today };

        Assert.False(TodoDeadlines.IsOverdue(todo, TodoDeadlines.Today(clock)));

        clock.Advance(TimeSpan.FromMinutes(2)); // nu är det 00:01 den 17:e i Stockholm

        Assert.True(TodoDeadlines.IsOverdue(todo, TodoDeadlines.Today(clock)));
    }

    [Fact]
    public void Days_late_counts_whole_days()
    {
        Assert.Equal(3, TodoDeadlines.DaysLate(Today.AddDays(-3), Today));
    }
}
