namespace Application.Common.CQRS;

/// <summary>
/// Marca una consulta que produce una respuesta de tipo TResponse.
/// </summary>
public interface IQuery<TResponse> { }

/// <summary>
/// Manejador de una consulta.
/// </summary>
public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}

/// <summary>
/// Despachador de consultas. Resuelve el handler correspondiente
/// desde el contenedor de DI y lo ejecuta.
/// </summary>
public interface IQueryDispatcher
{
    Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
