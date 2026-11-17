using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

/// <summary>Data access for todos. The service depends on this interface, which makes it testable without a database.</summary>
public interface ITodoRepository
{
    Task<IReadOnlyList<Todo>> ListAsync(CancellationToken cancellationToken);

    /// <summary>Uppgifter som är försenade vid tidpunkten <paramref name="now"/>. Filtreras i databasen.</summary>
    Task<IReadOnlyList<Todo>> ListOverdueAsync(DateTime now, CancellationToken cancellationToken);

    Task<Todo?> FindAsync(int id, CancellationToken cancellationToken);

    Task AddAsync(Todo todo, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
