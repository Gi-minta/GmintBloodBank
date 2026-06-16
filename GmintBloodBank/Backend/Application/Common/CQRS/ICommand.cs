namespace Application.Common.CQRS;

/// <summary>
/// Marca un comando que produce una respuesta de tipo TResponse.
/// Reemplaza IRequest&lt;T&gt; de MediatR con una abstracción propia.
/// </summary>
public interface ICommand<TResponse> { }

/// <summary>
/// Manejador de un comando. Reemplaza IRequestHandler de MediatR.
/// </summary>
public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Despachador de comandos. Resuelve el handler correspondiente
/// desde el contenedor de DI y lo ejecuta.
/// </summary>
public interface ICommandDispatcher
{
    Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
}
