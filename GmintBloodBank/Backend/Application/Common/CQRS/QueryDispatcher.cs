using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.CQRS;

/// <summary>
/// Implementación propia del despachador de consultas.
/// </summary>
public sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));

        var handler = _serviceProvider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod("HandleAsync")
            ?? throw new InvalidOperationException($"No se encontró HandleAsync para {handlerType.Name}");

        var result = method.Invoke(handler, new object[] { query, cancellationToken })
            ?? throw new InvalidOperationException("El handler retornó null.");

        return (Task<TResponse>)result;
    }
}
