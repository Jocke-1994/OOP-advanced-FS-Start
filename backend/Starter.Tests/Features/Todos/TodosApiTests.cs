using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>Starts the real API in memory, with its own throwaway SQLite file, and talks to it over HTTP.</summary>
public sealed class TodosApiTests : IClassFixture<TodosApiTests.ApiFactory>
{
    private readonly HttpClient _client;

    private readonly FakeTimeProvider _clock;

    public TodosApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
        _clock = factory.Clock;
    }

    [Fact]
    public async Task Post_then_get_returns_the_created_todo()
    {
        var created = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Write a test"));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var todos = await _client.GetFromJsonAsync<List<TodoResponse>>("/api/todos");

        Assert.Contains(todos!, t => t.Title == "Write a test");
    }

    [Fact]
    public async Task Post_with_empty_title_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest(""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Saras buggrapport: "Klockan nio på morgonen står det att Skicka offert är en dag sen,
    // fast den ska vara klar idag." 08:00 UTC är 09:00 i Stockholm i november.
    [Fact]
    [Trait("Krav", "AK-2.1")]
    public async Task A_todo_due_today_is_not_listed_as_overdue_at_nine_in_the_morning()
    {
        _clock.SetUtcNow(new DateTimeOffset(2026, 11, 16, 8, 0, 0, TimeSpan.Zero));
        await _client.PostAsJsonAsync("/api/todos", new { title = "Skicka offert (AK-2.1)", dueDate = "2026-11-16" });

        var overdue = await _client.GetFromJsonAsync<List<OverdueTodoResponse>>("/api/todos/overdue");

        Assert.DoesNotContain(overdue!, t => t.Title == "Skicka offert (AK-2.1)");
    }

    public sealed class ApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"starter-test-{Guid.NewGuid()}.db");

        /// <summary>En klocka som testerna styr själva. Ersätter TimeProvider.System i appen.
        /// Startar tidigt, eftersom FakeTimeProvider bara kan spolas framåt.</summary>
        public FakeTimeProvider Clock { get; } = new(new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.Zero));

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("ConnectionStrings:Default", $"Data Source={_databasePath}");
            builder.ConfigureTestServices(services => services.AddSingleton<TimeProvider>(Clock));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(_databasePath);
        }
    }
}
