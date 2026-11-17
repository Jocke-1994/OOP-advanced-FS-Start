using Microsoft.EntityFrameworkCore;
using Starter.Api.Data;
using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

public sealed class TodoRepository(AppDbContext db) : ITodoRepository
{
    public async Task<IReadOnlyList<Todo>> ListAsync(CancellationToken cancellationToken) =>
        await db.Todos.AsNoTracking().OrderByDescending(t => t.Id).ToListAsync(cancellationToken);

    // Samma regel som TodoDeadlines.IsOverdue, men som SQL (WHERE), så att databasen bara skickar
    // de försenade i stället för att vi hämtar alla uppgifter till minnet som Kim gjorde.
    public async Task<IReadOnlyList<Todo>> ListOverdueAsync(DateTime now, CancellationToken cancellationToken) =>
        await db.Todos.AsNoTracking()
            .Where(t => !t.IsDone && t.DueDate != null && t.DueDate < now)
            .OrderBy(t => t.DueDate)
            .ToListAsync(cancellationToken);

    public Task<Todo?> FindAsync(int id, CancellationToken cancellationToken) =>
        db.Todos.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddAsync(Todo todo, CancellationToken cancellationToken) =>
        await db.Todos.AddAsync(todo, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
}
