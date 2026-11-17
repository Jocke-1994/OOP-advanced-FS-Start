namespace Starter.Api.Features.Todos;

/// <summary>What a client sends to create a todo.</summary>
public sealed record CreateTodoRequest(string Title, DateTime? DueDate = null);

/// <summary>What a client gets back. This is the shape the frontend depends on.</summary>
public sealed record TodoResponse(int Id, string Title, bool IsDone, DateTimeOffset CreatedAt, DateTime? DueDate);

/// <summary>En försenad uppgift. Samma fält som Kims anonyma objekt, men nu med en riktig typ.</summary>
public sealed record OverdueTodoResponse(int Id, string Title, DateTime DueDate, int DaysLate);
